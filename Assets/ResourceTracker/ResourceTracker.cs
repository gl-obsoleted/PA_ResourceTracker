using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

public enum ResourceRequestType
{
    Ordinary,
    Async,
}

public class ResourceRequestInfo
{
    public int seqID = 0;
    public int rootID = 0;

    public ResourceRequestType requestType;

    public string resourcePath = "";
    public System.Type resourceType = null;

    public string srcFile = "";
    public int srcLineNum = 0;

    public double requestTime = Time.realtimeSinceStartup;

    public string ToString()
    {
        return string.Format("#{0} ({1:0.000}) {2} {3} {4} +{5} +{6} ({7})",
            seqID, requestTime, rootID, resourceType.ToString(), 
            requestType == ResourceRequestType.Async ? "(a)" : "", resourcePath, srcFile, srcLineNum);
    }

    public void RecordObject(UnityEngine.Object obj)
    {
        rootID = obj.GetInstanceID();
        resourceType = obj.GetType();
    }
}

public class ResourceTracker : IDisposable
{
    public static ResourceTracker Instance;

    private StreamWriter _logWriter = null;
    private string _logPath = "";
    private int _reqSeq = 0;

    public ResourceTracker()
    {
        try
        {
            DateTime dt = DateTime.Now;

            string logFile = string.Format("{0}_{1}_alloc.txt", SysUtil.FormatDateAsFileNameString(dt), SysUtil.FormatTimeAsFileNameString(dt));
            string logPath = Path.Combine(Application.persistentDataPath, logFile);

            _logWriter = new FileInfo(logPath).CreateText();
            _logWriter.AutoFlush = true;
            _logPath = logPath;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("[ResourceTracker.ctor] error: {0} ", ex.Message);

            if (_logWriter != null)
            {
                _logWriter.Close();
                _logWriter = null;
            }

            _logPath = "";
        }
    }

    public void Dispose()
    {
        if (_logWriter != null)
        {
            _logWriter.WriteLine("--------- unfinished request: {0} --------- ", InProgressAsyncObjects.Count);
            foreach (KeyValuePair<System.Object, ResourceRequestInfo> p in InProgressAsyncObjects)
            {
                _logWriter.WriteLine("  + {0}", p.Value.ToString());
            }
            _logWriter.Close();
            _logWriter = null;
        }
    }

    public void TrackSyncRequest(UnityEngine.Object spawned, string path)
    {
        var sf = new System.Diagnostics.StackFrame(2, true);
        var request = NewRequest(path, sf);
        request.requestType = ResourceRequestType.Ordinary;
        TrackRequestWithObject(request, spawned);
    }

    public void TrackResourcesDotLoad(UnityEngine.Object loaded, string path)
    {
        var sf = new System.Diagnostics.StackFrame(1, true);
        var request = NewRequest(path, sf);
        request.requestType = ResourceRequestType.Ordinary;
        TrackRequestWithObject(request, loaded);
    }

    private ResourceRequestInfo NewRequest(string path, StackFrame sf)
    {
        ResourceRequestInfo reqInfo = new ResourceRequestInfo();
        reqInfo.resourcePath = path;
        reqInfo.srcFile = sf.GetFileName();
        reqInfo.srcLineNum = sf.GetFileLineNumber();
        reqInfo.seqID = _reqSeq++;

        return reqInfo;
    }

    private void ExtractObjectResources(UnityEngine.Object obj, int reqSeqID)
    {
        SceneGraphExtractor sge = new SceneGraphExtractor(obj);

        for (int i = 0; i < sge.GameObjectIDs.Count; i++)
        {
            if (!TrackedGameObjects.ContainsKey(sge.GameObjectIDs[i]))
            {
                TrackedGameObjects[sge.GameObjectIDs[i]] = reqSeqID;
            }
        }

        foreach (var p in sge.MemObjectIDs)
        {
            foreach (var item in p.Value)
            {
                if (!TrackedMemObjects.ContainsKey(item))
                {
                    TrackedMemObjects[item] = reqSeqID;
                }
            }
        }
    }

    private void TrackRequestWithObject(ResourceRequestInfo req, UnityEngine.Object obj)
    {
        try
        {
            req.RecordObject(obj);

            TrackedAllocInfo[req.seqID] = req;
            ExtractObjectResources(obj, req.seqID);

            if (_logWriter != null)
                _logWriter.WriteLine(req.ToString());
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogErrorFormat("[ResourceTracker.TrackAsyncDone] error: {0} \n {1} \n {2}",
                ex.Message, req != null ? req.ToString() : "", ex.StackTrace);
        }
    }

    public void TrackAsyncRequest(System.Object handle, string path)
    {
        var sf = new System.Diagnostics.StackFrame(2, true);
        if (sf.GetMethod().Name.Contains("SpawnAsyncOldVer"))
        {
            sf = new System.Diagnostics.StackFrame(3, true);
        }

        InProgressAsyncObjects[handle] = NewRequest(path, sf);
    }

    public void TrackAsyncDone(System.Object handle, UnityEngine.Object target)
    {
        ResourceRequestInfo request;
        if (!InProgressAsyncObjects.TryGetValue(handle, out request))
            return;

        request.requestType = ResourceRequestType.Async;
        TrackRequestWithObject(request, target);
        InProgressAsyncObjects.Remove(handle);
    }

    public void TrackSceneLoaded(string sceneName)
    {
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene == null)
            return;

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            TrackSyncRequest(roots[i], "[scene]: " + sceneName);
        }
    }

    public void TrackObjectInstantiation(UnityEngine.Object src, UnityEngine.Object instantiated)
    {
        int allocSeqID = -1;
        if (!TrackedGameObjects.TryGetValue(src.GetInstanceID(), out allocSeqID))
            return;

        ExtractObjectResources(instantiated, allocSeqID);
    }

    public ResourceRequestInfo GetAllocInfo(int instID, string className)
    {
        int allocSeqID = -1;
        if (className == "GameObject")
        {
            if (!TrackedGameObjects.TryGetValue(instID, out allocSeqID))
                return null;
        }
        else if (SceneGraphExtractor.MemCategories.Contains(className))
        {
            if (!TrackedMemObjects.TryGetValue(instID, out allocSeqID))
                return null;
        }
        
        ResourceRequestInfo requestInfo = null;
        if (!TrackedAllocInfo.TryGetValue(allocSeqID, out requestInfo))
            return null;

        return requestInfo;
    }

    Dictionary<System.Object, ResourceRequestInfo> InProgressAsyncObjects = new Dictionary<System.Object, ResourceRequestInfo>();
    Dictionary<int, ResourceRequestInfo> TrackedAllocInfo = new Dictionary<int, ResourceRequestInfo>();

    Dictionary<int, int> TrackedGameObjects = new Dictionary<int, int>();
    Dictionary<int, int> TrackedMemObjects = new Dictionary<int, int>();
}

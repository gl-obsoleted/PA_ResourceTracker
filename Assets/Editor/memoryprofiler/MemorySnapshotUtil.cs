using UnityEngine;
using System.Collections;
using UnityEditor.MemoryProfiler;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MemorySnapshotUtil 
{
    public static string SnapshotsDir = string.Format("{0}/mem_snapshots", Application.persistentDataPath);

    public static string GetFullpath(string filename)
    {
        return string.IsNullOrEmpty(filename) ? "" : string.Format("{0}/{1}", SnapshotsDir, filename);
    }

    public static string[] GetFiles()
    {
        try
        {
            string[] files = Directory.GetFiles(SnapshotsDir);
            for (int i = 0; i < files.Length; i++)
            {
                int begin = files[i].LastIndexOfAny(new char[] { '\\', '/' });
                if (begin != -1)
                {
                    files[i] = files[i].Substring(begin + 1);
                }
            }
            return files;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return new string[] { };
        }

    }

    public static PackedMemorySnapshot Load(string filename)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
                throw new Exception("bad_load: filename is empty.");

            string fullpath = GetFullpath(filename);
            if (!File.Exists(fullpath))
                throw new Exception(string.Format("bad_load: file not found. ({0})", fullpath));

            using (Stream stream = File.Open(fullpath, FileMode.Open))
            {
                return new BinaryFormatter().Deserialize(stream) as PackedMemorySnapshot;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogErrorFormat("bad_load: exception occurs while loading '{0}'.", filename);
            Debug.LogException(ex);
            return null;
        }
    }

    public static string Save(PackedMemorySnapshot snapshot)
    {
        try
        {
            string filename = GetFullpath(string.Format("{0}-{1}.memsnap",
                SysUtil.FormatDateAsFileNameString(DateTime.Now),
                SysUtil.FormatTimeAsFileNameString(DateTime.Now)));

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                bf.Serialize(stream, snapshot);
            }
            return filename;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return "";
        }
    }

}

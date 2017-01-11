using UnityEditor;
using UnityEngine;

public class ServerAndTrackerLauncher : MonoBehaviour 
{
    public bool LogRemotely = true;
    public bool LogIntoFile = false;
    public bool InGameGui = false;

	void Start() 
    {
        _usmooth = new UsMain(LogRemotely, LogIntoFile, InGameGui);

#if UNITY_EDITOR
        EditorWindow w = EditorWindow.GetWindow<EditorWindow>("MemoryProfilerWindow");
        if (w.GetType().Name == "MemoryProfilerWindow")
        {
            w.SendEvent(EditorGUIUtility.CommandEvent("AppStarted"));
        }
#endif
        ResourceTracker.Instance = new ResourceTracker(true);
    }

    void Update()
    {
        if (_usmooth != null)
		    _usmooth.Update();
	}

    void OnDestroy()
    {
        if (_usmooth != null)
            _usmooth.Dispose();
    }

    void OnGUI() 
    {
        if (_usmooth != null)
            _usmooth.OnGUI();
	}

    void OnLevelWasLoaded()
    {
        if (_usmooth != null)
            _usmooth.OnLevelWasLoaded();
    }

    private UsMain _usmooth;
}

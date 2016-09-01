using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planter : MonoBehaviour {

    Dictionary<string, string> _plantInfo = new Dictionary<string, string> {
        { "PlantPos_00", "trees/baum ld0(enhanced)"},
        { "PlantPos_01", "trees/baum ld1(enhanced)"},
        { "PlantPos_02", "trees/baum ld2(enhanced)"},
    };

    Dictionary<string, GameObject> _plantPrototypes = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> _plantAnchors = new Dictionary<string, GameObject>();

    bool _init = false;

	void Start () {
        foreach (var name in _plantInfo.Keys)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                _plantAnchors[name] = go;
            }
        }

        // error handling omitted
        if (_plantAnchors.Count != _plantInfo.Count)
            return;

        _init = true;
	}
	
    void OnGUI()
    {
        GUILayout.BeginVertical();

        foreach (var name in _plantInfo.Keys)
        {
            if (GUILayout.Button(name))
            {
                PlantAt(name);
            }
        }
        if (GUILayout.Button("Resources Cleanup"))
        {
            Resources.UnloadUnusedAssets();
        }
        GUILayout.EndVertical();
    }

    void PlantAt(string name)
    {
        if (!_init)
            return;

        GameObject proto = null;
        if (!_plantPrototypes.TryGetValue(name, out proto))
        {
            string path = _plantInfo[name];
            var r = Resources.Load(path, typeof(GameObject)) as GameObject;
            ResourceTracker.Instance.TrackResourcesDotLoad(r, path);
            if (r == null)
                return;

            proto = _plantPrototypes[name] = r;
        }

        GameObject anchor = _plantAnchors[name];
        GameObject plant = Instantiate(proto) as GameObject;
        if (anchor != null && plant != null)
        {
            ResourceTracker.Instance.TrackObjectInstantiation(proto, plant);
            plant.transform.parent = anchor.transform;
            plant.transform.localPosition = Vector3.zero;
            plant.transform.localScale = Vector3.one * 0.3f;
        }
    }
}

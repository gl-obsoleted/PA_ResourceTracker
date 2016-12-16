using UnityEngine;
using System.Collections;

public class DemoMain : MonoBehaviour {

	void Start () {
        ResourceTracker.Instance = new ResourceTracker(true);
	}
	
	void Update () {
	
	}
}

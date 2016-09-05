using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ResourceTracker.Instance = new ResourceTracker(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

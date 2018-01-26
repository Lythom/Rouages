using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSizeFromRatio : MonoBehaviour {

	public float height = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Camera.main.orthographicSize = height / Camera.main.aspect;
		Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, - Camera.main.orthographicSize, Camera.main.transform.position.z);
	}
}

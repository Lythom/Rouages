using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSizeFromRatio : MonoBehaviour {

	public float width = 8f;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void LateUpdate () {
		if (Camera.main.orthographic) {
			Camera.main.orthographicSize = width / Camera.main.aspect;
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		} else {
			Camera.main.fieldOfView = 2 * Mathf.Atan(Mathf.Tan((width * 9f) * Mathf.Deg2Rad / 2) / Camera.main.aspect) * Mathf.Rad2Deg;
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
			
		}
	}
}
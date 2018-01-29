using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Track : MonoBehaviour {

	public Transform target;
	public Vector3 offset;

	// Update is called once per frame
	void Update () {
		if (target != null) {
			this.transform.position = target.position + offset;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public bool alsoWave = false;

	private float i = 0;
	// Update is called once per frame
	void Update () {
		i+=0.1f;
		transform.Rotate (0, 0, -4f + (alsoWave ? 2f : 0));
		if (alsoWave) {
			transform.localPosition = new Vector3 (0, Mathf.Cos (i) * 0.4f + 0.2f, 0);
		}
	}
}
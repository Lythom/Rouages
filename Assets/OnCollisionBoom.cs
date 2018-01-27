using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionBoom : MonoBehaviour {
    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter2D(Collision2D other) {
		audioSource.Play();
		audioSource.pitch = Random.Range(2, 3);
	}
}

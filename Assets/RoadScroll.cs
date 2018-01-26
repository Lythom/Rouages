using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoadScroll : MonoBehaviour {

	public float speed = 0.2f;
	
	public GameObject[] tracks;

	private float offset = 0f;

	private GameObject currentTrack = null;
	private GameObject nextTrack = null;

	private Vector3 startPosCurrent = new Vector3(8, 0, 0);
	private Vector3 startPosNext = new Vector3(8 + 16, 0, 0);


	// Use this for initialization
	void Start ()
    {
        currentTrack = Instantiate(tracks[Random.Range(0, tracks.Length)], this.transform);
        currentTrack.transform.position = startPosCurrent;
        nextTrack = instantiateNextTrack();
    }

    private GameObject instantiateNextTrack()
    {
        var n = Instantiate(tracks[Random.Range(0, tracks.Length)]);
        n.transform.position = startPosNext;
		return n;
    }

    // Update is called once per frame
    void FixedUpdate() {
		offset = offset + speed;
		if(offset > 16) {
			offset -= 16;
			Destroy(currentTrack);
			currentTrack = nextTrack;
			nextTrack = instantiateNextTrack();
		}
		currentTrack.transform.position = new Vector3(startPosCurrent.x - offset, currentTrack.transform.position.y, currentTrack.transform.position.z);
		nextTrack.transform.position = new Vector3(startPosNext.x - offset, nextTrack.transform.position.y, nextTrack.transform.position.z);
		
	}
}

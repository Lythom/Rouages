using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public class RoadScroll : MonoBehaviour {

    public static float TRACK_WIDTH = 32f;

    public double speed = 0.15f;

    public GameObject[] tracks;

    public GameObject tutoTrack;

    [Tooltip ("Piloté automatiquement, ne pas modifier")]
    public double offset = 0f;
    [Tooltip ("Piloté automatiquement, ne pas modifier")]
    // TODO : move to Party and use A delegate event to inform of offsetChange
    public float trackFinishedCount = 0f;
    public bool tutorial;

    private GameObject[] compatibleTracks;
    private GameObject currentTrack = null;
    private GameObject nextTrack = null;

    private Vector3 startPosCurrent = new Vector3 (TRACK_WIDTH / 4, 0, 0);
    private Vector3 startPosNext = new Vector3 (TRACK_WIDTH / 4 + TRACK_WIDTH, 0, 0);

    // Use this for initialization
    void Start () {
        currentTrack = Instantiate (tutoTrack);
        currentTrack.transform.position = startPosCurrent;
        nextTrack = instantiateNextTrack ();
    }

    private int getOut (string trackName) {
        return int.Parse (trackName[trackName.Length - 1].ToString ());
    }
    private int getIn (string trackName) {
        return int.Parse (trackName[trackName.Length - 2].ToString ());
    }

    private GameObject instantiateNextTrack () {
        if (!tutorial) {
            if (currentTrack.name == tutoTrack.name) {
                // Starting tile is the first of the array
                var n = Instantiate (tracks[0], this.transform);
                n.name = n.name.Replace ("(Clone)", "");
                n.transform.position = startPosNext;
                return n;
            } else {
                int output = getOut (currentTrack.name);
                compatibleTracks = tracks.Where (item => {
                    var input = getIn (item.name);
                    return (input & output) == output; // input must cover at least all the outputs
                }).ToArray ();

                GameObject randomTrack = compatibleTracks[Random.Range (0, compatibleTracks.Length)];
                var n = Instantiate (randomTrack);
                n.name = n.name.Replace ("(Clone)", "");
                n.transform.position = startPosNext;
                return n;
            }
        } else {
            var n = Instantiate (tutoTrack);
            n.name = n.name.Replace ("(Clone)", "");
            n.transform.position = startPosNext;
            return n;
        }

    }

    // Update is called once per frame
    void FixedUpdate () {
        offset = offset + speed;
        if (offset > TRACK_WIDTH) {
            offset -= TRACK_WIDTH;
            Destroy (currentTrack);
            currentTrack = nextTrack;
            nextTrack = instantiateNextTrack ();
            if (!tutorial)
                trackFinishedCount++;
        }
        currentTrack.transform.position = new Vector3 (startPosCurrent.x - (float) offset, currentTrack.transform.position.y, currentTrack.transform.position.z);
        nextTrack.transform.position = new Vector3 (startPosNext.x - (float) offset, nextTrack.transform.position.y, nextTrack.transform.position.z);

    }
}
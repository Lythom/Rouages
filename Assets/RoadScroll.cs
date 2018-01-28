﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
public class RoadScroll : MonoBehaviour
{

	public static float TRACK_WIDTH = 32f;

    public double speed = 0.15f;

    public GameObject[] tracks;

    public GameObject tutoTrack;
	
	[Tooltip("Piloté automatiquement, ne pas modifier")]
	public double offset = 0f;
	[Tooltip("Piloté automatiquement, ne pas modifier")]
	// TODO : move to Party and use A delegate event to inform of offsetChange
	public float trackFinishedCount = 0f;
    public bool tutorial;

    private GameObject[] compatibleTracks;
    private GameObject currentTrack = null;
    private GameObject nextTrack = null;

    private Vector3 startPosCurrent = new Vector3(TRACK_WIDTH / 4, 0, 0);
    private Vector3 startPosNext = new Vector3(TRACK_WIDTH / 4 + TRACK_WIDTH, 0, 0);


    // Use this for initialization
    void Start()
    {   
        currentTrack = Instantiate(tutoTrack);
        currentTrack.transform.position = startPosCurrent;
        nextTrack = instantiateNextTrack();
    }

    private GameObject instantiateNextTrack()
    {
        if (! tutorial)
        {
            var currentTrackNumber = Regex.Match(currentTrack.name, @"\d+").Value;            
            if (int.Parse(currentTrackNumber) == 1)
            {
                // Starting tile is the first of the array
                var n = Instantiate(tracks[0], this.transform);
                n.transform.position = startPosNext;
                return n;
            } else
            {
                var currentTrackNumberArray = currentTrackNumber.ToCharArray();

                compatibleTracks = tracks.Where(item => {
                    var itemNumber = Regex.Match(item.name, @"\d+").Value;
                    var itemNumberArray = itemNumber.ToCharArray();
                    var output = int.Parse(currentTrackNumberArray[1].ToString());
                    var input = int.Parse(itemNumberArray[0].ToString());

                    return output == input;
                }).ToArray();
                
                var n = Instantiate(compatibleTracks[Random.Range(0, compatibleTracks.Length)]);
                n.transform.position = startPosNext;
                return n;
            }
        } else
        {
            var n = Instantiate(tutoTrack);
            n.transform.position = startPosNext;
            return n; 
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        offset = offset + speed;
        if (offset > TRACK_WIDTH)
        {
            offset -= TRACK_WIDTH;
            Destroy(currentTrack);
            currentTrack = nextTrack;
            nextTrack = instantiateNextTrack();
            if(! tutorial)
			    trackFinishedCount ++;
        }
        currentTrack.transform.position = new Vector3(startPosCurrent.x - (float)offset, currentTrack.transform.position.y, currentTrack.transform.position.z);
        nextTrack.transform.position = new Vector3(startPosNext.x - (float)offset, nextTrack.transform.position.y, nextTrack.transform.position.z);

    }
}

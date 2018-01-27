using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Party : MonoBehaviour
{

    public List<Move> cars;
    public Transform losePosition;
    public Transform winPosition;

    public RoadScroll roadScroll;

    [Tooltip("How much cars are far from each other at start")]
    public float initialSpacing = 2f;

    [Tooltip("Number of tracks before the game ends and the scores are reset.")]
    public int partyLength = 20;

    private Dictionary<int, int> scores = new Dictionary<int, int>();

    // Use this for initialization
    void Start()
    {
        placeCars();
        initScores();
        roadScroll.trackFinishedCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cars.ForEach(c =>
        {
            if (c.transform.position.x > winPosition.position.x)
            {
                scores[c.playerId] = scores[c.playerId] + 1;
                c.transform.position = getStartPosition(c);
            }
            if (c.transform.position.x < losePosition.position.x)
            {
                scores[c.playerId] = System.Math.Max(0, scores[c.playerId] - 1);
                c.transform.position = getStartPosition(c);
            }
        });

        if (roadScroll.trackFinishedCount >= partyLength)
        {
            roadScroll.enabled = false;
        }
    }

    private void placeCars()
    {
        cars.ForEach(car => car.transform.position = getStartPosition(car));
    }

    private Vector2 getStartPosition(Move car)
    {
        return new Vector2(GetStartX(), -(initialSpacing * cars.Count) / 2 + initialSpacing * car.playerId);
    }

    private void initScores()
    {
        cars.ForEach(car => scores[car.playerId] = 0);
    }

    private float GetStartX()
    {
        return (losePosition.position.x + winPosition.position.x) / 2;
    }
}

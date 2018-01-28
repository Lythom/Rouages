using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Party : MonoBehaviour
{

    public static int INITIAL_GEAR_AMOUNT = 3;

    public List<Move> cars;
    public Transform losePosition;
    public Transform winPosition;

    public Transform canvas;
    public GameObject playerScorePrefab;

    public RoadScroll roadScroll;
    public Tuto tuto;

    [Tooltip("How much cars are far from each other at start")]
    public float initialSpacing = 2f;

    [Tooltip("Number of tracks before the game ends and the scores are reset.")]
    public int partyLength = 20;

    private Dictionary<int, int> scores = new Dictionary<int, int>();
    private Dictionary<int, Text> scoreTexts = new Dictionary<int, Text>();
    private AudioSource audioSource;

    private GameObject tutoText;

    // Use this for initialization
    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        placeCars();
        initScores();
        roadScroll.trackFinishedCount = 0;
        tutoText = GameObject.Find("PlayAgain");
        tutoText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        cars.ForEach(c =>
        {
            if (c.transform.position.x > winPosition.position.x)
            {
                scores[c.playerId] = scores[c.playerId] + 1;
                scoreTexts[c.playerId].text = "Player " + c.playerId + "\n" + scores[c.playerId];
                c.moveTo(getStartPosition(c));
                c.GearAmount = INITIAL_GEAR_AMOUNT;
                audioSource.Stop();
                audioSource.Play();
            }
            if (c.transform.position.x < losePosition.position.x)
            {
                scores[c.playerId] = System.Math.Max(0, scores[c.playerId] - 1);
                scoreTexts[c.playerId].text = "Player " + c.playerId + "\n" + scores[c.playerId];
                c.moveTo(getStartPosition(c));
                c.GearAmount = INITIAL_GEAR_AMOUNT;
            }
        });

        if (roadScroll.trackFinishedCount >= partyLength)
        {   
            // Go to tuto and set RoadScroll in tuto mode
            roadScroll.tutorial = true;
            roadScroll.trackFinishedCount = 0;
            // roadScroll.currentTrack = Instantiate(roadScroll.tutoTrack);
            // roadScroll.nextTrack = Instantiate(roadScroll.tutoTrack);
            roadScroll.offset = 64;

            tuto.enabled = true;
            tuto.placeCars();
            tutoText.SetActive(true);

            this.enabled = false;     
        }
    }

    private void placeCars()
    {
        cars.ForEach(c =>
        {
            c.moveTo(getStartPosition(c));
        });
    }

    private Vector3 getStartPosition(Move car)
    {
        return new Vector3(GetStartX(), -(initialSpacing * cars.Count) / 2 + initialSpacing * car.playerId, -3);
    }

    private void initScores()
    {
        cars.ForEach(c =>
        {
            scores[c.playerId] = 0;
            scoreTexts[c.playerId] = Instantiate(playerScorePrefab, canvas).GetComponent<Text>();
            RectTransform t = scoreTexts[c.playerId].transform as RectTransform;
            t.anchoredPosition = new Vector2(-150 + 450 * c.playerId, -110f);
            scoreTexts[c.playerId].text = "Player " + c.playerId + "\n" + scores[c.playerId];

        });
    }

    private float GetStartX()
    {
        return (losePosition.position.x + winPosition.position.x) / 2;
    }
}

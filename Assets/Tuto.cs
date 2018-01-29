using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tuto : MonoBehaviour {
    public List<Move> cars;
    public Transform losePosition;
    public Transform winPosition;

    [Tooltip ("How much cars are far from each other at start")]
    public float initialSpacing = 2f;

    public GameObject helpText;

    public RoadScroll roadScroll;
    public Party party;

    private Move[] movedCars;
    private bool starting;
    private float carPlacedTime = 0;

    // Use this for initialization
    void Awake() {
        if(helpText != null) helpText.GetComponent<MeshRenderer> ().sortingLayerName = "Flying";     
    }

    void Start () {
        OnEnable ();
    }

    void OnEnable () {
        // audioSource = GetComponent<AudioSource>();
        if (this.enabled) {
            placeCars ();
            if(helpText != null) helpText.GetComponent<TextMesh> ().text = "Move here \n to start";
            starting = false;
        }
    }

    void LateUpdate () {
        movedCars = cars.Where (c => {
            return c.transform.position.y > 0.2f;
        }).ToArray ();

        if (movedCars.Length == 4 && !starting && Time.time > carPlacedTime) {
            // Tuto over
            StartCoroutine (StartGame ());
            starting = true;
        }

        if (!starting) {
            cars.ForEach (c => {
                var target = getCarStart (c.playerId);
                var move = (target - c.transform.position.x) * 0.4f;
                float yPos = Mathf.Max(Mathf.Min(c.transform.position.y, 4f), -4f);
                c.transform.position = new Vector3 (c.transform.position.x + move, yPos, c.transform.position.z);
                c.tutoVisual.SetActive(c.transform.position.y <= 0.2f);
            });
        }

    }

    IEnumerator StartGame () {
        helpText.GetComponent<TextMesh> ().text = "Starting...";
        party.placeCars ();
        yield return new WaitForSeconds (3);
        party.enabled = true;
        roadScroll.tutorial = false;
        this.enabled = false;
    }

    private float getCarStart (int playerId) {
        return GetStartX () + playerId * 2.5f - 6.5f;
    }

    public void placeCars () {
        carPlacedTime = Time.time + 3f;
        // Set position depending on car id
        cars.ForEach (car => {
            car.moveTo (new Vector3 (getCarStart (car.playerId), -2, -3));
            car.GearAmount = 3;
            car.tutoVisual.SetActive(true);
        });
    }

    private float GetStartX () {
        return (losePosition.position.x + winPosition.position.x) / 2;
    }
}
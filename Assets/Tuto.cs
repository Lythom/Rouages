using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tuto : MonoBehaviour
{
    public List<Move> cars;
    public Transform losePosition;
    public Transform winPosition;

    [Tooltip("How much cars are far from each other at start")]
    public float initialSpacing = 2f;

    public RoadScroll roadScroll;
    public Party party;

    private Move[] movedCars;


    // Use this for initialization
    void Start()
    {
        OnEnable();
    }

    void OnEnable()
    {
        // audioSource = GetComponent<AudioSource>();
        if (this.enabled)
        {
            placeCars();
        }
    }

    void Update()
    {
        movedCars = cars.Where(c =>
        {
            return c.transform.position.y > 0.2f;
        }).ToArray();

        if (movedCars.Length == 3)
        {
            // Tuto over
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        GameObject.Find("PlayAgain").GetComponent<TextMesh>().text = "Starting...";
        party.placeCars();
        yield return new WaitForSeconds(3);
        party.enabled = true;
        roadScroll.tutorial = false;
        this.enabled = false;
    }


    public void placeCars()
    {
        // Set position depending on car id
        cars.ForEach(car => {
            car.moveTo(new Vector3(GetStartX() + car.playerId * 2.5f - 6.5f, -2, -3));
            car.GearAmount = 3;
        });
    }

    private float GetStartX()
    {
        return (losePosition.position.x + winPosition.position.x) / 2;
    }
}

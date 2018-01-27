using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public int playerId;
    public float acceleration = 40f;
    public float maxVerticalSpeed = 99f;
    public float rotationAmount = 0.5f;
    private Rigidbody2D rb;
    private Vector2 movingVector;

    private float gearAmount = 3;
    private float horizontalSpeed;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D> ();
        horizontalSpeed = (gearAmount - 3) * 0.1f;
        int i = 0;
        foreach (Transform item in this.transform) {
            i++;
            if (gearAmount < i) {
                item.gameObject.SetActive (false);
            }
        }
    }

    private void OnCollisionEnter2D (Collision2D other) {
        if (other.gameObject.CompareTag ("Collectible")) {
            if (gearAmount < 7) {
                gearAmount++;
                horizontalSpeed = (gearAmount - 3) * 0.1f;
                int i = 0;
                foreach (Transform item in this.transform) {
                    i++;
                    if (gearAmount >= i) {
                        item.gameObject.SetActive (true);
                    }
                }
            }
            Destroy (other.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        if (rb == null) return;

        movingVector = new Vector2 (horizontalSpeed, Input.GetAxis ("Vertical"));
        if (Mathf.Abs (rb.velocity.y) < maxVerticalSpeed) {
            rb.AddForce (movingVector * acceleration);
        }
        rb.velocity = new Vector2 (horizontalSpeed, rb.velocity.y * 0.9f);

        this.transform.rotation = new Quaternion (0, 0, rb.velocity.y * rotationAmount, 1);
    }
}
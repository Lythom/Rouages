using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	public float playerId;	
	private Rigidbody2D rb;
	private Vector2 movingVector;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		movingVector = new Vector2(0, Input.GetAxis("Vertical"));				
		rb.velocity = movingVector * 5;
	}
}

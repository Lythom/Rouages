﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{


    public SpriteRenderer rope;
    public Transform thrower;

    public Transform Hooked
    {
        get { return target; }
    }

    private Transform target;
    private Vector3 hookedRelativePosition;
    private Vector3 shootDirection;

    private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            // do hook
            hook(other.transform);
        }
        else
        {
            dehook();
            // TODO play sound here
        }
    }

    private void hook(Transform otherCar)
    {
        if (target == null) {
            target = otherCar.transform;
            hookedRelativePosition = this.transform.position - target.transform.position;
            target.GetComponent<Move>().GearAmount++;
            thrower.GetComponent<Move>().GearAmount--;
        }
        // TODO play sound here

    }

    public void fire()
    {
        rb.position = this.transform.position = thrower.position + Vector3.right;
        var initialVelocity = thrower.GetComponent<Rigidbody2D>().velocity;
        shootDirection = thrower.rotation * Vector3.right + new Vector3(0, initialVelocity.y * 0.6f, 0);
        this.gameObject.SetActive(true);
        Update();
    }

    public void dehook()
    {
        // TODO: Get Nothing back if no-one was hooked
        if (target != null) target.GetComponent<Move>().GearAmount -= 2;
        if (thrower != null && target != null) thrower.GetComponent<Move>().GearAmount += 2;
        this.gameObject.SetActive(false);
        target = null;
    }


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 direction = (thrower.transform.position - this.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion qDirection = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
        this.transform.rotation = qDirection;

        if (target == null)
        {
            rb.velocity = shootDirection * 4;
        }
        else
        {
            rb.position = this.transform.position = target.position + hookedRelativePosition;
            rb.velocity = Vector2.zero;
            if(target.position.x < thrower.position.x) {
                dehook();
            }
        }

        rope.size = new Vector2(Vector3.Distance(this.transform.position, thrower.transform.position), rope.size.y);
        rope.transform.localPosition = new Vector2(-rope.size.x / 2, rope.transform.localPosition.y);

    }
}
using System;
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
    private Vector3 hookedRelativePosition = Vector3.zero;
    private Vector3 shootDirection;

    private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            // do hook
            hook(other.transform);
        }
        else if (target == null)
        {
            dehook();
            // TODO play sound here
        }
    }

    private void hook(Transform otherCar)
    {
        if (target == null)
        {
            target = otherCar.transform;
            //hookedRelativePosition = this.transform.position - target.transform.position;
            this.GetComponentInChildren<SpriteRenderer>().enabled = false;
            target.GetComponent<Move>().GearAmount++;
        }
        // TODO play sound here

    }

    public void fire()
    {
        rb.position = this.transform.position = thrower.position + Vector3.right;
        var initialVelocity = thrower.GetComponent<Rigidbody2D>().velocity;
        shootDirection = thrower.rotation * Vector3.right + new Vector3(0, initialVelocity.y * 0.2f, 0);
        thrower.GetComponent<Move>().GearAmount--;
        this.gameObject.SetActive(true);
        FixedUpdate();
    }

    public void dehook()
    {
        // TODO: Get Nothing back if no-one was hooked
        if (target != null) target.GetComponent<Move>().GearAmount -= 2;
        if (thrower != null && target != null) thrower.GetComponent<Move>().GearAmount += 2;
        this.GetComponentInChildren<SpriteRenderer>().enabled = true;
        this.gameObject.SetActive(false);
        target = null;
    }


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 direction = (thrower.transform.position - (target == null ? this.transform.position : target.position)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion qDirection = Quaternion.AngleAxis(angle + 180f, Vector3.forward) * Quaternion.AngleAxis((thrower.position.z - (target == null ? 0 : target.position.z)) * 17, Vector3.up);
        this.transform.rotation = qDirection;

        if (target == null)
        {
            rb.velocity = shootDirection * 5.5f;
        }
        else
        {
            rb.position = this.transform.position = target.position + hookedRelativePosition;
            rb.velocity = Vector2.zero;
            if (target.position.x < thrower.position.x)
            {
                dehook();
            }
        }

        rope.size = new Vector2(Vector3.Distance(this.transform.position, thrower.transform.position), rope.size.y);
        rope.transform.localPosition = new Vector2(-rope.size.x / 2, rope.transform.localPosition.y);

    }
}

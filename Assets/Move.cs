﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour {

    public static float MAX_VOLUME = 0.33f;
    public static float MIN_VOLUME = 0.11f;

    public int playerId;
    public float verticalAcceleration = 25f;
    public float horizontalAcceleration = 0.1f;
    public float horizontalMaxSpeedPerGear = 0.17f;
    public float maxVerticalSpeed = 99f;
    public float rotationAmount = 0.05f;

    public ParticleSystem groundTrail;
    public ParticleSystem flyTrail;

    public Hook hook;

    private bool isHookingButtonReleased = true;
    private bool Hooking {
        get { return hook != null && hook.gameObject.activeSelf; }
    }

    public SpriteRenderer wings;
    public SpriteRenderer shadow;
    public Text playerText;
    public GameObject tutoVisual;
    public AudioClip[] engineSounds;
    public AudioClip[] levelUpSounds;
    public AudioClip[] pickupSounds;

    private Rigidbody2D rb;

    private int gearAmount = 3;
    private AudioSource audioSource;
    private bool flying = false;
    private bool autoDriving;

    public int GearAmount {
        get { return gearAmount; }
        set {
            int amount = Math.Max (Math.Min (value, 7), 0);
            if (audioSource != null) {
                audioSource.Stop ();
                if (gearAmount < amount) {
                    PlayAudio (levelUpSounds[System.Math.Min (amount - 1, levelUpSounds.Length - 1)], false);
                    audioSource.volume = MAX_VOLUME;
                }
            }
            gearAmount = amount;
            updateGearVisuals ();
        }
    }

    private float HorizontalSpeed {
        get { return (gearAmount - 3) * horizontalMaxSpeedPerGear; }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D> ();
        audioSource = GetComponents<AudioSource> ().Where (a => a.clip == null).First ();
        updateGearVisuals ();
        playerText.text = "P" + playerId;
    }

    private void updateGearVisuals () {
        int i = 0;
        foreach (Transform item in this.transform) {
            if (item.CompareTag ("cargear")) {
                i++;
                item.gameObject.SetActive (GearAmount >= i);
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.CompareTag ("Collectible")) {
            GearAmount++;
            Destroy (other.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        if (rb == null) return;

        if (Mathf.Abs (rb.velocity.y) < maxVerticalSpeed) {
            rb.AddForce (new Vector2 (0, Input.GetAxis ("Vertical" + playerId) * verticalAcceleration));
        }

        // HSpeed (hooked vs. geared)
        if (Hooking && hook.Hooked != null) {
            rb.AddForce (new Vector2 (0, (hook.Hooked.position.y - this.transform.position.y) * 20f));
            rb.velocity = new Vector2 (hook.Hooked.GetComponent<Rigidbody2D> ().velocity.x, rb.velocity.y * 0.9f);
        } else {
            float targetVelocity = HorizontalSpeed;
            float currentVelocity = rb.velocity.x;
            float step = (targetVelocity - currentVelocity) * horizontalAcceleration;
            rb.velocity = new Vector2 (rb.velocity.x + step, rb.velocity.y * 0.9f);
        }

        this.transform.rotation = new Quaternion (0, 0, Input.GetAxis ("Vertical" + playerId) * rotationAmount, 1);

        if (!audioSource.isPlaying) {
            PlayAudio (engineSounds[System.Math.Min (GearAmount, engineSounds.Length - 1)], true);
            audioSource.volume = MIN_VOLUME;

        }

        if (Input.GetButtonDown ("Fire" + playerId) || Input.GetAxis ("Fire" + playerId) != 0) {
            if (!Hooking && isHookingButtonReleased) {
                // TODO: lose 1 gear here and not when hook worked !
                hook.fire ();
                isHookingButtonReleased = false;
            }
        } else {
            if (Hooking) {
                hook.dehook ();
            }
            isHookingButtonReleased = true;

        }

        foreach (Transform item in this.transform) {
            if (item.CompareTag ("cargear")) {
                item.gameObject.transform.Rotate (0, 0, -gearAmount * 1.2f);
            }
        }

        var p = this.transform.position;
        if (p.z < -0.1) {
            displayWings ();
            fly ();
            shadow.transform.position = new Vector3 (p.x, p.y, 0);
            shadow.transform.localScale = new Vector3 ((9 + p.z) / 9, (9 + p.z) / 9, 1);
        } else {
            land ();
            // out = sent left to lose
            if (Math.Abs (this.transform.position.y) > 5) {
                rb.velocity = new Vector2 (-12, 0);
            }
        }
        float yPos = Mathf.Max(Mathf.Min(p.y, 4.8f), -4.8f);
        if (this.hook.Hooked != null) {
            displayWings ();
            this.transform.position = new Vector3 (p.x, yPos, p.z <= -3 ? -3f : p.z - 0.07f);
        } else {
            this.transform.position = new Vector3 (p.x, yPos, p.z >= 0 ? 0f : p.z + 0.05f - GearAmount * 0.005f);
            if (this.transform.position.z > -0.1) {
                hideWings ();
            }
        }
    }

    public void moveTo (Vector3 position, bool freeY = false) {
        if (!autoDriving) {
            autoDriving = true;
            if (rb != null) rb.velocity = new Vector2 (0, 0);
            StartCoroutine (autoDrive (position, freeY));
        }
    }

    private IEnumerator autoDrive (Vector3 dest, bool freeY = false) {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < 3) {
            float yPos = transform.position.y;
            transform.position = Vector3.Lerp (startingPos, dest, (elapsedTime / 3));
            if (freeY) {
                transform.position = new Vector3 (transform.position.x, yPos, transform.position.z);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        autoDriving = false;
        yield return null;
    }

    private void hideWings () {
        wings.enabled = false;
        flyTrail.gameObject.SetActive(false);
        groundTrail.gameObject.SetActive(true);
        //shadow.enabled = false;
    }

    private void displayWings () {
        wings.enabled = true;
        flyTrail.gameObject.SetActive(true);
        groundTrail.gameObject.SetActive(false);
        
        //shadow.enabled = true;
    }

    private List<SpriteRenderer> GetAllSpriteRenderers () {
        return this.GetComponentsInChildren<SpriteRenderer> ()
            .Concat (this.GetComponents<SpriteRenderer> ())
            .ToList ();
    }
    private void land () {
        if (this.flying) {
            hideWings ();
            this.gameObject.layer = LayerMask.NameToLayer ("Default");
            GetAllSpriteRenderers ().ForEach (sr => sr.sortingLayerName = "Default");
            this.flying = false;
        }
    }

    private void fly () {
        if (!this.flying) {
            this.gameObject.layer = LayerMask.NameToLayer ("Flying");
            GetAllSpriteRenderers ().ForEach (sr => sr.sortingLayerName = "Flying");
            this.flying = true;
        }
    }

    public void PlayAudio (AudioClip clip, bool loop) {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.playOnAwake = true;
        audioSource.Play ();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : MonoBehaviour
{

    public static float MAX_VOLUME = 0.35f;
    public static float MIN_VOLUME = 0.15f;

    public int playerId;
    public float verticalAcceleration = 25f;
    public float horizontalAcceleration = 0.1f;
    public float horizontalMaxSpeedPerGear = 0.17f;
    public float maxVerticalSpeed = 99f;
    public float rotationAmount = 0.05f;

    private bool hooking = false;

    public AudioClip[] engineSounds;
    public AudioClip[] levelUpSounds;

    private Rigidbody2D rb;

    private int gearAmount = 3;
    private AudioSource audioSource;

    public int GearAmount
    {
        get { return gearAmount; }
        set
        {
            audioSource.Stop();
            if (gearAmount < value) PlayAudio(levelUpSounds[System.Math.Min(value - 1, levelUpSounds.Length - 1)], false);
            audioSource.volume = MAX_VOLUME;
            gearAmount = value;
            updateGearVisuals();
        }
    }

    private float HorizontalSpeed
    {
        get { return (gearAmount - 3) * horizontalMaxSpeedPerGear; }
    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponents<AudioSource>().Where(a => a.clip == null).First();
        updateGearVisuals();
    }

    private void updateGearVisuals()
    {
        int i = 0;
        foreach (Transform item in this.transform)
        {
            i++;
            item.gameObject.SetActive(GearAmount >= i);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            if (GearAmount < 7)
            {
                GearAmount++;
            }
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rb == null) return;

        if (Mathf.Abs(rb.velocity.y) < maxVerticalSpeed)
        {
            rb.AddForce(new Vector2(0, Input.GetAxis("Vertical" + playerId) * verticalAcceleration));
        }
        float targetVelocity = HorizontalSpeed;
        float currentVelocity = rb.velocity.x;
        float step = (targetVelocity - currentVelocity) * horizontalAcceleration;
        rb.velocity = new Vector2(rb.velocity.x + step, rb.velocity.y * 0.9f);

        this.transform.rotation = new Quaternion(0, 0, rb.velocity.y * rotationAmount, 1);

        if (!audioSource.isPlaying)
        {
            PlayAudio(engineSounds[System.Math.Min(GearAmount, engineSounds.Length - 1)], true);
        }
        audioSource.volume = audioSource.volume + (MIN_VOLUME - audioSource.volume) * 0.15f;

        if (Input.GetButtonDown("Fire" + playerId) || Input.GetAxis("Fire" + playerId) != 0)
        {
            if(!hooking) {
                Debug.Log("Fire" + playerId);
                hooking = true;
            }
        } else if (hooking) {
            Debug.Log("Dehooking " + playerId);            
            hooking = false;
        }
    }

    public void PlayAudio(AudioClip clip, bool loop)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
}

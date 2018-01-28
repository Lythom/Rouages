using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : MonoBehaviour
{

    public static float MAX_VOLUME = 0.35f;
    public static float MIN_VOLUME = 0.17f;

    public int playerId;
    public float verticalAcceleration = 25f;
    public float horizontalAcceleration = 0.1f;
    public float horizontalMaxSpeedPerGear = 0.17f;
    public float maxVerticalSpeed = 99f;
    public float rotationAmount = 0.05f;

    public Hook hook;

    private bool Hooking
    {
        get { return hook != null && hook.gameObject.activeSelf; }
    }

    public AudioClip[] engineSounds;
    public AudioClip[] levelUpSounds;
    public AudioClip[] pickupSounds;

    private Rigidbody2D rb;

    private int gearAmount = 3;
    private AudioSource audioSource;

    public int GearAmount
    {
        get { return gearAmount; }
        set
        {
            if (value <= 7 && value >= 0) {
                audioSource.Stop();
                if (gearAmount < value) PlayAudio(levelUpSounds[System.Math.Min(value - 1, levelUpSounds.Length - 1)], false);
                audioSource.volume = MAX_VOLUME;
                gearAmount = value;
                updateGearVisuals();
            }
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
            GearAmount++;
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
        

        if (Hooking && hook.Hooked != null) {
            rb.AddForce(new Vector2(0, (hook.Hooked.position.y - this.transform.position.y) * 20f));
            rb.velocity = new Vector2(hook.Hooked.GetComponent<Rigidbody2D>().velocity.x, rb.velocity.y * 0.9f);
        } else {
            float targetVelocity = HorizontalSpeed;
            float currentVelocity = rb.velocity.x;
            float step = (targetVelocity - currentVelocity) * horizontalAcceleration;
            rb.velocity = new Vector2(rb.velocity.x + step, rb.velocity.y * 0.9f);
        }

        this.transform.rotation = new Quaternion(0, 0, rb.velocity.y * rotationAmount, 1);

        if (!audioSource.isPlaying)
        {
            PlayAudio(engineSounds[System.Math.Min(GearAmount, engineSounds.Length - 1)], true);
        }
        audioSource.volume = audioSource.volume + (MIN_VOLUME - audioSource.volume) * 0.12f;

        if (Input.GetButtonDown("Fire" + playerId) || Input.GetAxis("Fire" + playerId) != 0)
        {
            if (!Hooking)
            {
                hook.fire();
            }
        }
        else if (Hooking)
        {
            hook.dehook();
        }

        // if(this.transform.z > )
        // this.gameObject.layer = LayerMask.NameToLayer("Flying");
        
    }

    public void PlayAudio(AudioClip clip, bool loop)
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
}

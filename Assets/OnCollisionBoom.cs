using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnCollisionBoom : MonoBehaviour
{

	public AudioClip boomClip;
	public AudioClip[] mechanicClips;
    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponents<AudioSource>().Where(a => a.clip != null).First();
    }

    // Update is called once per frame
    void Update()
    {

    }
	private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
			audioSource.Stop();
            audioSource.clip = mechanicClips[Random.Range(0, mechanicClips.Length)];			
			audioSource.pitch = Random.Range(1, 2);
			audioSource.volume = 0.25f;
			audioSource.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
		if (other.relativeVelocity.magnitude < 1.5) return;

        audioSource.Stop();
		audioSource.clip = boomClip;		
        audioSource.pitch = Random.Range(2, 3.3f);
		audioSource.volume = Mathf.Min(other.relativeVelocity.magnitude / 15, 0.35f);
        audioSource.Play();
    }
}

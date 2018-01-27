using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnCollisionBoom : MonoBehaviour
{
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

    private void OnCollisionEnter2D(Collision2D other)
    {
		if (other.relativeVelocity.magnitude < 1.5) return;

        audioSource.Stop();
        audioSource.pitch = Random.Range(2, 3.3f);
		audioSource.volume = Mathf.Min(other.relativeVelocity.magnitude / 20, 0.35f);
        audioSource.Play();
    }
}

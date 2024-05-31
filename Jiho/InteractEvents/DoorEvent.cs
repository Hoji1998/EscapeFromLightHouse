using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvent : InteractEvent
{
    [Header("Door Sound")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    [SerializeField] private float soundScale = 0.1f;

    [Header("Guide Line")]
    public LineRenderer guideLine;

    private Animator Door;
    private AudioSource audioSource;

    private void Awake()
    {
        Door = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        Door.SetBool("isOpen", false);

        base.SetGuideLine(guideLine);
    }

    public override void StartInteractEvent()
    {
        Door.SetBool("isOpen", true);
        audioSource.Stop();
        audioSource.PlayOneShot(doorOpenSound, soundScale);
    }

    public override void StopInteractEvent()
    {
        Door.SetBool("isOpen", false);
        audioSource.Stop();
        audioSource.PlayOneShot(doorCloseSound, soundScale);
    }
}

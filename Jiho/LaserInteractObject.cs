using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInteractObject : MonoBehaviour
{
    [Header("Interact Component")]
    [SerializeField] private List<InteractEvent> events;

    [Header("Interact State")]
    public bool IsInteract = false;

    [Header("Interact Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private float soundScale = 0.1f;

    [HideInInspector] public GameObject InteractLaser;

    private Material material;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (events.Count > 0)
        {
            foreach (InteractEvent eventObject in events)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, eventObject.transform.position);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(eventObject.transform.position, 0.3f);
            }
        }
    }

    private void Start()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
        material.color = Color.blue;

        //foreach (InteractEvent e in events)
        //{
        //    e.GetGuideLine().startColor = Color.blue;
        //    e.GetGuideLine().endColor = Color.blue;

        //    e.GetGuideLine().SetPosition(0, transform.position);
        //    e.GetGuideLine().SetPosition(1, e.transform.position);
        //    e.GetGuideLine().gameObject.SetActive(true);
        //}
    }

    private void Update()
    {
        ProcessAbility();
    }

    private void ProcessAbility()
    {
        if (!AuthorizedInteract())
        {
            InteractOff();
        }
    }
    private bool AuthorizedInteract()
    {
        if (InteractLaser == null)
            return false;

        if (!InteractLaser.activeSelf)
            return false;

        return true;
    }

    public void InteractOn()
    {
        if (!IsInteract)
        {
            foreach (InteractEvent e in events)
            {
                e.StartInteractEvent();
                //e.GetGuideLine().startColor = Color.red;
                //e.GetGuideLine().endColor = Color.red;
            }

            material.color = Color.red;
            audioSource.Stop();
            audioSource.PlayOneShot(interactSound, soundScale);
        }

        IsInteract = true;
    }

    public void InteractOff()
    {
        if (IsInteract)
        {
            foreach (InteractEvent e in events)
            {
                e.StopInteractEvent();
                //e.GetGuideLine().startColor = Color.blue;
                //e.GetGuideLine().endColor = Color.blue;
            }
            material.color = Color.blue;
        }

        IsInteract = false;
    }
}

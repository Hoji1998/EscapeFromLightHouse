using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TreadInteractObject : MonoBehaviour
{
    [Header("Interact Component")]
    [SerializeField] private List<InteractEvent> events;
    [SerializeField] private bool isButtonDown = false;
    [SerializeField] private GameObject buttonOnPosition;
    [SerializeField] private GameObject buttonOffPosition;

    [Header("Button Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonOnSound;
    [SerializeField] private AudioClip buttonOffSound;
    [SerializeField] private float soundScale = 0.1f;

    [Header("Change Materials")]
    [SerializeField] private MeshRenderer[] meshRenderers;

    private Material[] materials;
    private float emmisionIntensity = 40f;
    private float pushTime = 0.3f;
    private List<GameObject> currentCollideObject;
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

    private void Awake()
    {
        currentCollideObject = new List<GameObject>();
        materials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            materials[i] = meshRenderers[i].material;
        }

        ChangeColor(Color.cyan);
    }
    private void Start()
    {
        //foreach (InteractEvent e in events)
        //{
        //    e.GetGuideLine().startColor = Color.blue;
        //    e.GetGuideLine().endColor = Color.blue;

        //    e.GetGuideLine().SetPosition(0, transform.position);
        //    e.GetGuideLine().SetPosition(1, e.transform.position);
        //    e.GetGuideLine().gameObject.SetActive(true);
        //}
    }
    private void ChangeColor(Color color)
    {
        foreach (Material material in materials)
        {
            material.SetColor("_EmissiveColor", color * emmisionIntensity);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigid = collision.gameObject.GetComponent<Rigidbody>();

        if (rigid != null)
        {
            currentCollideObject.Add(collision.gameObject);
            InteractButton();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rigid = collision.gameObject.GetComponent<Rigidbody>();

        if (rigid != null)
        {
            currentCollideObject.Remove(collision.gameObject);
            if (currentCollideObject.Count == 0)
            {
                ButtonReset();
            }
        }
    }

    private void Update()
    {
        if (currentCollideObject.Count == 0)
            return;

        foreach (GameObject collideObject in currentCollideObject)
        {
            if (!collideObject.activeSelf)
            {
                currentCollideObject.Remove(collideObject);
                if (currentCollideObject.Count == 0)
                {
                    ButtonReset();
                }
                return;
            }
        }
    }
    private void InteractButton()
    {
        if (!AuthorizedInteract())
            return;

        ButtonOn();
    }

    private bool AuthorizedInteract()
    {
        if (events.Count <= 0)
        {
            return false;
        }

        if (isButtonDown)
        {
            return false;
        }

        return true;
    }

    private void ButtonOn()
    {
        isButtonDown = true;
        ChangeColor(Color.red);
        transform.DOLocalMove(buttonOnPosition.transform.localPosition, pushTime);
        audioSource.Stop();
        audioSource.PlayOneShot(buttonOnSound, soundScale);

        foreach (InteractEvent e in events)
        {
            e.StartInteractEvent();
            //e.GetGuideLine().startColor = Color.red;
            //e.GetGuideLine().endColor = Color.red;
        }
    }

    private void ButtonReset()
    {
        isButtonDown = false;
        ChangeColor(Color.cyan);
        transform.DOLocalMove(buttonOffPosition.transform.localPosition, pushTime);
        audioSource.Stop();
        audioSource.PlayOneShot(buttonOffSound, soundScale);

        foreach (InteractEvent e in events)
        {
            e.StopInteractEvent();
            //e.GetGuideLine().startColor = Color.blue;
            //e.GetGuideLine().endColor = Color.blue;
        }
    }
}

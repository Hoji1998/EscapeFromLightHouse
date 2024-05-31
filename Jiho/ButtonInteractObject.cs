using DG.Tweening;
using HoJin.GameScene;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractObject : InteractObjectController
{
    [Header("Interact Component")]
    [SerializeField] private List<InteractEvent> events;
    [SerializeField] private bool isButtonDown = false;
    [SerializeField] private float resetTime = 1.5f;
    [SerializeField] private GameObject buttonOnPosition;
    [SerializeField] private GameObject buttonOffPosition;

    [Header("Additional Button")]
    [SerializeField] private ButtonInteractObject additionalButton;

    [Header("Button Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonOnSound;
    [SerializeField] private AudioClip buttonOffSound;
    [SerializeField] private float soundScale = 0.1f;

    private Material material;
    private float pushTime = 0.3f;
    private GameManager gameManager;
    private GUIManager guiManager;

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

        if (additionalButton != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, additionalButton.transform.position);
        }
    }

    private new void Awake()
    {
        base.Awake();
        OnPointer = new Action(InteractButton);
        material = GetComponent<MeshRenderer>().material;

        material.SetColor("_EmissiveColor", Color.cyan * 40f);

        if (isButtonDown)
        {
            material.SetColor("_EmissiveColor", Color.red * 40f);
            transform.DOLocalMove(buttonOnPosition.transform.localPosition, pushTime);
            if (events.Count > 0)
            {
                foreach (InteractEvent e in events)
                {
                    e.StartInteractEvent();
                    //e.GetGuideLine().startColor = Color.red;
                    //e.GetGuideLine().endColor = Color.red;
                }
            }
        }
    }
    private new void Start()
    {
        base.Start();
        //foreach (InteractEvent e in events)
        //{
        //    e.GetGuideLine().startColor = Color.blue;
        //    e.GetGuideLine().endColor = Color.blue;

        //    e.GetGuideLine().SetPosition(0, transform.position);
        //    e.GetGuideLine().SetPosition(1, e.transform.position);
        //    e.GetGuideLine().gameObject.SetActive(true);
        //}
    }
    private void InteractButton()
    {
        if (!AuthorizedInteract())
            return;

        if (gameManager == null)
            gameManager = GameManager.Instance;
        if (guiManager == null)
            guiManager = GUIManager.Instance;

        if (guiManager != null)
            guiManager.OnInteractButtonGuideText();

        if (Input.GetKeyDown(KeyCode.F))
        {
            ButtonOn();
        }
    }

    private bool AuthorizedInteract()
    {
        if (isButtonDown)
        {
            return false;
        }

        return true;
    }

    private void ButtonOn()
    {
        isButtonDown = true;
        material.SetColor("_EmissiveColor", Color.red * 40f);
        transform.DOLocalMove(buttonOnPosition.transform.localPosition, pushTime);
        audioSource.PlayOneShot(buttonOnSound, soundScale);

        if (events.Count > 0)
        {
            foreach (InteractEvent e in events)
            {
                e.StartInteractEvent();
                //e.GetGuideLine().startColor = Color.red;
                //e.GetGuideLine().endColor = Color.red;
            }
        }

        if (additionalButton != null)
        {
            additionalButton.ButtonReset();
        }

        if (resetTime == 0)
            return;

        if (resetTime > pushTime)
        {
            Invoke("ButtonReset", resetTime);
        }
        else
        {
            Invoke("ButtonReset", pushTime);
        }
    }

    public void ButtonReset()
    {
        isButtonDown = false;
        material.SetColor("_EmissiveColor", Color.cyan * 40f);
        transform.DOLocalMove(buttonOffPosition.transform.localPosition, pushTime);
        audioSource.PlayOneShot(buttonOffSound, soundScale);

        if (events.Count == 0)
            return;

        foreach (InteractEvent e in events)
        {
            e.StopInteractEvent();
            //e.GetGuideLine().startColor = Color.blue;
            //e.GetGuideLine().endColor = Color.blue;
        }
    }
}

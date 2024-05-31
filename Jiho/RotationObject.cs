using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoJin.GameScene;
using DG.Tweening;

public class RotationObject : InteractObjectController
{
    [Header("Rotate Component")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private RotationObject targetRotateLogic;
    [SerializeField] private int rotateAngle = 0;
    [SerializeField] private float bufferTime = 0.3f;
    [Header("Rotate Axis")]
    [SerializeField] bool xAxisRotate = false;
    [Header("Rotation Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rotateSound;
    [SerializeField] private float soundScale = 0.1f;

    [HideInInspector] public bool IsRotate = false;

    private Vector3 initRotation;
    private float currentBufferTime = 0f;
    private GameManager gameManager;
    private GUIManager guiManager;

    private new void Awake()
    {
        base.Awake();
        OnPointer = new Action(Rotate);
    }

    private void Rotate()
    {
        if (!AuthorizedRotate())
            return;

        if (gameManager == null)
            gameManager = GameManager.Instance;
        if (guiManager == null)
            guiManager = GUIManager.Instance;

        if (guiManager != null)
            guiManager.OnRotateButtonGuideText();

        

        if (Input.GetKeyDown(KeyCode.Q))
        {
            initRotation = targetObject.transform.localRotation.eulerAngles;
            if (xAxisRotate)
            {
                initRotation.x += rotateAngle;
            }
            else
            {
                initRotation.y += rotateAngle;
            }
            
            targetObject.transform.DOLocalRotate(initRotation, bufferTime * 0.9f).OnStart(RotateStart).OnComplete(RotateEnd);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            initRotation = targetObject.transform.localRotation.eulerAngles;
            if (xAxisRotate)
            {
                initRotation.x -= rotateAngle;
            }
            else
            {
                initRotation.y -= rotateAngle;
            }

            targetObject.transform.DOLocalRotate(initRotation, bufferTime * 0.9f).OnStart(RotateStart).OnComplete(RotateEnd);
        }
    }

    private bool AuthorizedRotate()
    {
        if (targetObject == null)
            return false;

        if (targetRotateLogic.IsRotate)
        {
            return false;
        }

        return true;
    }

    private void RotateSoundPlay()
    {
        audioSource.PlayOneShot(rotateSound, soundScale);
    }
    private void RotateStart()
    {
        targetRotateLogic.IsRotate = true;
        RotateSoundPlay();
    }
    private void RotateEnd()
    {
        targetRotateLogic.IsRotate = false;
    }
}

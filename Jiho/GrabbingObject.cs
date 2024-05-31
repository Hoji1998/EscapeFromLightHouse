using HoJin.GameScene;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using AmazingAssets.AdvancedDissolve;

public class GrabbingObject : InteractObjectController
{
    [SerializeField] private float GrabDelayTime = 0.2f;
    public Collider coll;

    [Header("Collide Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collideSound;
    [SerializeField] private float soundScale = 0.1f;

    [HideInInspector] public Rigidbody rigid;
    private float hitSoundDelayTime = 0.2f;
    private GameManager gameManager;
    private GUIManager guiManager;
    private bool GrabOn = false;
    private MeshRenderer meshRenderer;
    private Material material;
    private Coroutine coroutine;
    private new void Awake()
    {
        base.Awake();
        OnPointer = new Action(Grab);
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    private void OnEnable()
    {
        coll.enabled = true;
        rigid.useGravity = true;
        StopAllCoroutines();
        coroutine = StartCoroutine(DissolveOn(true));
    }

    private IEnumerator DissolveOn(bool active) 
    {
        float clip;
        if (active)
        {
            clip = 0.7f;
        }
        else
        {
            clip = 0f;
        }

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (active)
            {
                clip -= Time.deltaTime * 0.5f;
                if (clip <= 0f)
                {
                    AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material,
                    AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 0f);
                    StopCoroutine(coroutine);
                    break;
                }
            }
            else
            {
                clip += Time.deltaTime * 0.5f;
                if (clip <= 0f)
                {
                    AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material,
                    AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 0.8f);
                    gameObject.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }
            }
            AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material,
                AdvancedDissolveProperties.Cutout.Standard.Property.Clip, clip);
        }
    }

    private void Grab()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager.PlayerInteract.GetIsGrabbing() || GrabOn)
        {
            return;
        }

        if (guiManager == null)
        {
            guiManager = GUIManager.Instance;
        }
        if (guiManager != null)
            guiManager.OnInteractButtonGuideText();

        if (Input.GetKeyDown(KeyCode.F))
        {
            gameManager.PlayerInteract.GrabbingObject = this;
            gameManager.PlayerInteract.SetIsGrabbing(true);
            rigid.useGravity = false;

            GrabOn = true;
        }
    }
    public void StopObject()
    {
        gameObject.transform.rotation = gameManager.PlayerInteract.GetExaminePointTransform().rotation;
    }
    public void Drop()
    {
        ReturnGravity();
        Invoke("GrabDelaying", GrabDelayTime);
        gameManager.PlayerInteract.GrabbingObject = null;
        rigid.velocity = Vector3.zero;
    }

    public void DissolveEnd()
    {
        coll.enabled = false;
        rigid.useGravity = false;
        StopAllCoroutines();
        coroutine = StartCoroutine(DissolveOn(false));
    }

    private void ReturnGravity()
    {
        rigid.useGravity = true;
    }

    private void GrabDelaying()
    {
        GrabOn = false;
    }

    #region MovingPlatform
    private GameObject contactPlatform;
    private MovingPlatformEvent movingPlatform;
    private Vector3 distance;

    private void Update()
    {
        if (contactPlatform == null)
            return;

        if (!movingPlatform.IsMove)
            return;

        transform.position = contactPlatform.transform.position - distance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(collideSound, soundScale);
        //플랫폼이 45도 이내의 기울기일 때에만 바닥으로 판정
        if (collision.contacts[0].normal.y > 0.7f)
        {
            //접촉한 오브젝트의 태그가 platform 일 때,
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                contactPlatform = collision.gameObject;
                movingPlatform = contactPlatform.GetComponent<MovingPlatformEvent>();
                distance = contactPlatform.transform.position - transform.position;
            }
            else
            {
                contactPlatform = null;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            contactPlatform = null;
        }
    }
    #endregion
}

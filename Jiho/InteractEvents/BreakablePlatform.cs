using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BreakablePlatform : InteractEvent
{
    [Header("BreakablePlatform")]
    [SerializeField] public GameObject ShakeBox;
    [SerializeField] private float limitTime = 0f;

    [Header("Breakable Sound")]
    [SerializeField] private AudioSource audioSource;

    [HideInInspector] public Collider coll;
    private Rigidbody rigid;
    private SelfDestroy selfDestroy;
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        selfDestroy = ShakeBox.GetComponent<SelfDestroy>();
        ShakeBox.transform.parent = null;
    }
    public override void StartInteractEvent()
    {
        StartCoroutine(BreakOn());
    }
    private IEnumerator BreakOn()
    {
        audioSource.Play();
        yield return new WaitForSeconds(limitTime);
        rigid.useGravity = true;
        rigid.isKinematic = false;
        selfDestroy.SelfDestroyOn();
        gameObject.GetComponent<SelfDestroy>().SelfDestroyOn();

        audioSource.Play();
    }

    public override void StopInteractEvent()
    {
        rigid.useGravity = false;
        rigid.isKinematic = true;
    }
}
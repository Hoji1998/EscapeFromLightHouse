using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;

public class Water : MonoBehaviour
{
    [Header("IceBlock")]
    public GameObject IceBlock;

    [Header("GuideObject")]
    public GameObject GuideObject;

    [Header("Material Intensity Component")]
    [SerializeField] private float maximumIntensity = 200f;
    [SerializeField] private float minimumIntensity = 4f;
    [SerializeField] private float changeIntensitySpeed = 6f;

    [Header("floating Component")]
    [SerializeField] private Vector3 waterDirection;
    [SerializeField] private float flowRate = 1f;
    [SerializeField] private float buoyancy = 5f;
    [SerializeField] private float maximumBuoyancy = 5f;

    [Header("Ice Sound")]
    public AudioSource audioSource;
    [SerializeField] private AudioClip IceSound;
    [SerializeField] private float soundScale = 0.1f;

    private float materialIntensity = 200f;
    private Material iceMaterial;
    private Vector2 iceTileValue;
    private MeshRenderer waterMeshRenderer;
    private Collider waterCollider;
    private Rigidbody floatingObjectRigid;

    private void Start()
    {
        iceMaterial = IceBlock.GetComponent<MeshRenderer>().material;
        waterCollider = GetComponent<Collider>();
        waterMeshRenderer = GetComponent<MeshRenderer>();
        GuideObject.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.5f, 1f, 0.3f);

        iceTileValue = new Vector2(1f / (transform.localScale.z / transform.localScale.x), 1f);

        iceMaterial.SetTextureScale("_BaseColorMap", iceTileValue);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, waterDirection + transform.position);
        Gizmos.DrawSphere(transform.position, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(waterDirection + transform.position, 0.3f);
    }

    public void ReturnWater()
    {
        StartCoroutine(MeltingIce());
    }

    public void FreezingWater()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(IceSound, soundScale);

        StartCoroutine(Freezing());
    }

    IEnumerator MeltingIce()
    {
        waterCollider.enabled = true;
        waterMeshRenderer.enabled = true;
        materialIntensity = maximumIntensity;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (materialIntensity > minimumIntensity + changeIntensitySpeed)
            {
                materialIntensity -= changeIntensitySpeed;
            }
            else
            {
                materialIntensity = minimumIntensity;
                break;
            }

            iceMaterial.SetColor("_EmissiveColor", Color.white * materialIntensity);
        }
        IceBlock.SetActive(false);
        StopCoroutine(MeltingIce());
    }
    IEnumerator Freezing()
    {
        IceBlock.SetActive(true);
        materialIntensity = maximumIntensity;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (materialIntensity > minimumIntensity + changeIntensitySpeed)
            {
                materialIntensity -= changeIntensitySpeed;
            }
            else
            {
                materialIntensity = minimumIntensity;
                break;
            }

            iceMaterial.SetColor("_EmissiveColor", Color.white * materialIntensity);
        }
        StopCoroutine(Freezing());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovingController>() != null)
        {
            GUIManager.Instance.ScreenFadeIn(LevelManager.Instance.currentStage.StageName);
            GUIManager.Instance.failedCanvas.gameObject.SetActive(true);
            LevelManager.Instance.player.GetComponent<PlayerMovingController>().isStunned = true;
            return;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        floatingObjectRigid = other.GetComponentInParent<Rigidbody>();

        if (floatingObjectRigid == null)
            return;

        floatingObjectRigid.AddForce(new Vector3(waterDirection.x, 1f, waterDirection.z) * buoyancy);

        if (floatingObjectRigid.velocity.x >= flowRate)
        {
            floatingObjectRigid.velocity = new Vector3(flowRate, floatingObjectRigid.velocity.y, floatingObjectRigid.velocity.z);
        }
        else if (floatingObjectRigid.velocity.x <= -flowRate)
        {
            floatingObjectRigid.velocity = new Vector3(-flowRate, floatingObjectRigid.velocity.y, floatingObjectRigid.velocity.z);
        }

        if (floatingObjectRigid.velocity.z >= flowRate)
        {
            floatingObjectRigid.velocity = new Vector3(floatingObjectRigid.velocity.x, floatingObjectRigid.velocity.y, flowRate);
        }
        else if (floatingObjectRigid.velocity.z <= -flowRate)
        {
            floatingObjectRigid.velocity = new Vector3(floatingObjectRigid.velocity.x, floatingObjectRigid.velocity.y, -flowRate);
        }

        if (floatingObjectRigid.velocity.y >= maximumBuoyancy)
        {
            floatingObjectRigid.velocity = new Vector3(floatingObjectRigid.velocity.x, maximumBuoyancy, floatingObjectRigid.velocity.z);
        }
    }
}

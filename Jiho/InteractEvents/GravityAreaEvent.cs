using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;

public class GravityAreaEvent : InteractEvent
{
    [Header("WallGravity")]
    [SerializeField] private bool IsWallGravity = false;
    [Header("force Direction")]
    [SerializeField] private Vector3 forceVector;
    [Header("Gravity Plane")]
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject area;
    [Header("Gravity Particle")]
    [SerializeField] private ParticleSystem particle;

    private bool gravityOn = false;
    private float materialIntensity = 40f;
    private MeshRenderer planeMeshRenderer;
    private MeshRenderer areaMeshRenderer;
    private ParticleSystemRenderer particleMeshRenderer;
    private ParticleSystem.ForceOverLifetimeModule particleforceOverLifetime;
    private CharacterController characterController;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, forceVector + transform.position);
        Gizmos.DrawSphere(transform.position, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(forceVector + transform.position, 0.3f);
    }
    private void Start()
    {
        planeMeshRenderer = plane.GetComponent<MeshRenderer>();
        areaMeshRenderer = area.GetComponent<MeshRenderer>();
        particleMeshRenderer = particle.GetComponent<ParticleSystemRenderer>();

        ParticleSystem.EmissionModule particleEmission = particle.emission;
        particleEmission.rateOverTime = gameObject.transform.localScale.x * gameObject.transform.localScale.y * gameObject.transform.localScale.z;

        particleforceOverLifetime= particle.forceOverLifetime;
        particleforceOverLifetime.z = 0.1f * (Mathf.Abs(forceVector.x) + Mathf.Abs(forceVector.y) + Mathf.Abs(forceVector.z));
        ChangeColor();
    }

    public override void StartInteractEvent()
    {
        gravityOn = true;
        ChangeColor();
    }
    public override void StopInteractEvent()
    {
        gravityOn = false;
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (gravityOn)
        {
            planeMeshRenderer.material.SetColor("_EmissiveColor", Color.red);
            areaMeshRenderer.material.SetColor("_EmissiveColor", Color.red);
            particleMeshRenderer.material.color = Color.red;
            particleMeshRenderer.material.SetColor("_EmissiveColor", Color.red * materialIntensity);
        }
        else
        {
            planeMeshRenderer.material.SetColor("_EmissiveColor", Color.blue);
            areaMeshRenderer.material.SetColor("_EmissiveColor", Color.blue);
            particleMeshRenderer.material.color = Color.cyan;
            particleMeshRenderer.material.SetColor("_EmissiveColor", Color.cyan * materialIntensity);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        ConstantForce constantForce = other.GetComponent<ConstantForce>();
        characterController = other.GetComponent<CharacterController>();
        PlayerMovingController characterMovingController = other.GetComponent<PlayerMovingController>();

        if (!gravityOn)
        {
            if (characterController != null)
            {
                characterMovingController.InitGravityDirection(IsWallGravity);
            }

            if (rigid != null && constantForce != null)
            {
                rigid.useGravity = true;
                constantForce.force = Vector3.zero;
            }
            return;
        }

        if (characterController != null)
        {
            characterMovingController.StopCoroutine(characterMovingController.LerpGravityDirection());
            characterMovingController.gravityForce = forceVector * 10f;
            if (IsWallGravity)
            {
                characterController.transform.localRotation = Quaternion.Slerp(characterController.transform.localRotation,
               transform.localRotation,
                5f * Time.fixedDeltaTime);
            }
        }

        if (rigid != null && constantForce != null)
        {
            rigid.useGravity = false;
            constantForce.force = forceVector * 10f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        ConstantForce constantForce = other.GetComponent<ConstantForce>();
        characterController = other.GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.GetComponent<PlayerMovingController>().InitGravityDirection(IsWallGravity);
        }

        if (rigid != null && constantForce != null)
        {
            rigid.useGravity = true;
            constantForce.force = Vector3.zero;
        }
    }
}

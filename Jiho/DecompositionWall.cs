using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;
public class DecompositionWall : MonoBehaviour
{
    [SerializeField] private float dissolvingTime = 1f;
    private GrabbingObject grabbingObject;
    private CharacterController characterController;
    private PlayerInteractController playerInteractController;
    private PostProcessingControl postProcessingControl;
    private GameManager gameManager;

    private void Start()
    {
        playerInteractController = Camera.main.GetComponent<PlayerInteractController>();
        postProcessingControl = Camera.main.GetComponent<PostProcessingControl>();
        gameManager = GameManager.Instance;
    }

    private void OnTriggerStay(Collider other)
    {
        characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            postProcessingControl.DecompositionScreenEffect();
            gameManager.PlayerWeapon.CancleBullet();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        grabbingObject = other.GetComponent<GrabbingObject>();
        if (grabbingObject != null && grabbingObject.coll.enabled)
        {
            playerInteractController.ProcessDrop();
            grabbingObject.DissolveEnd();
            playerInteractController.SetIsGrabbing(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class FixedCameraObject : InteractObjectController
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] protected FlashLightState lightState;
        private readonly string cameraTransformName = "CameraTransform";



        protected new void Reset()
        {
            base.Reset();
            Transform cameraTransform = transform.Find(cameraTransformName);
            if (cameraTransform == null)
            {
                cameraTransform = new GameObject(cameraTransformName).transform;
                
            }
            cameraTransform.SetParent(transform);
            this.cameraTransform = cameraTransform;
        }



        public override void Interact()
        {
            GameManager.Instance.PushControlKeyLockState(GameManager.Instance.GetQuizControlKeyLock());
            GameManager.Instance.PlayerInteract.SetIsSelect(true);
            FixCamera(cameraTransform, lightState);
            TurnOffEmission();
        }

        public override void GetInputAfterInteract()
        {
            if (GameManager.Instance.InputRightClick())
            {
                Unselect();
            }
        }

        public virtual void Unselect()
        {
            GameManager.Instance.PopControlKeyLockState();
            GameManager.Instance.PlayerInteract.SetIsSelect(false);
            UnfixCamera();
            GameManager.Instance.PlayerInteract.SetWatchingInteractObject();
        }

        public static void FixCamera(Transform cameraTransform, FlashLightState lightState)
        {
            GameManager.Instance.UIController.GetSelectPoint().SetActive(false);
            GameManager.Instance.UIController.DisappearInteractUIsAndItemUsingUIs();

            GameManager.Instance.PlayerInteract.SetCameraTransform(cameraTransform);
            GameManager.Instance.PlayerInteract.UseExamineLight(lightState);
        }
        public static void UnfixCamera()
        {
            GameManager.Instance.UIController.GetSelectPoint().SetActive(true);

            GameManager.Instance.PlayerInteract.SetCameraOriginalTransform();
            GameManager.Instance.PlayerInteract.UseFlashlight();
        }
    }
}
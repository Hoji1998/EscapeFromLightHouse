using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace HoJin.GameScene
{
    public class PlayerInteractController : GameDirector
    {
        #region 필드
        [SerializeField] private int rayLength = 5;
        [Header("Interact Sound")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip interactSound;
        [SerializeField] private float soundScale = 0.1f;
        private InteractObjectController selectedObject;
        private Transform examinePointTransform;
        private Light examineLight;
        private Camera examineCamera;
        private Camera mainCamera;       
        private int layerMask;
        private bool isSelect; 
        private bool isGrabbing; //2022. 08. 01. jiho
        private GameManager gameManager; //2022. 08. 01. jiho
        private GUIManager guiManager;
        private GrabbingObject grabbingObject; //2022. 08. 01. jiho
        private Animator animator;
        #endregion

        #region 속성
        public InteractObjectController SelectedObject { get => selectedObject; set => selectedObject = value; }
        public GrabbingObject GrabbingObject { get => grabbingObject; set => grabbingObject = value; }
        public Transform GetExaminePointTransform()
        {
            return examinePointTransform;
        }
        private void SetExaminePointTransform(Transform value)
        {
            examinePointTransform = value;
        }

        public Light GetExamineLight()
        {
            return examineLight;
        }
        private void SetExamineLight(Light value)
        {
            examineLight = value;
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }
        private void SetMainCamera(Camera value)
        {
            mainCamera = value;
        }

        public bool GetIsSelect()
        {
            return isSelect;
        }
        public void SetIsSelect(bool value)
        {
            isSelect = value;
        }

        public bool GetIsGrabbing()  //2022. 08. 01. jiho
        {
            return isGrabbing;
        }

        public void SetIsGrabbing(bool value)  //2022. 08. 01. jiho
        {
            isGrabbing = value;
        }

        public Animator Animator { get => animator; }
        #endregion



        protected void Awake()
        {
            selectedObject = null;
            SetExaminePointTransform(transform.GetChild(0).GetChild(0));
            SetMainCamera(GetComponent<Camera>());
            SetIsSelect(false);
            SetExamineLight(transform.GetChild(0).GetChild(1).GetComponent<Light>());
            transform.GetChild(0).GetChild(2).TryGetComponent(out examineCamera);
            TryGetComponent(out animator);
            layerMask = 1 << LayerMask.NameToLayer(KeyWord.interact);
        }

        private void Update()
        {
            ProcessGrab();  //2022. 08. 01. jiho

            if (!GetIsSelect())
            {
                if (IsLookingAtInteractObject(out InteractObjectController objectLookingAt))
                {
                    if (selectedObject != objectLookingAt)
                    {
                        SetNotWatchingInteractObject();
                        selectedObject = objectLookingAt;
                        SetWatchingInteractObject();
                    }

                    if (selectedObject.InputInteractKey.Invoke())
                    {
                        selectedObject.InteractEvent.Invoke();
                    }
                    selectedObject.OnPointer.Invoke();
                }
                else
                {
                    if (selectedObject != null)
                    {
                        SetNotWatchingInteractObject();
                        selectedObject = null;
                    }
                    if (guiManager == null)
                    {
                        guiManager = GUIManager.Instance;
                    }
                    if (guiManager != null)
                    {
                        guiManager.OffAllGuideText();
                    }
                }
                
            }
            else
            {
                SelectedObject.GetInputAfterInteract();
            }
        }


        private void ProcessGrab()  //2022. 08. 01. jiho
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            if (isGrabbing)
            {
                if (grabbingObject == null)
                    return;

                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                grabbingObject.rigid.velocity = (GetExaminePointTransform().position - grabbingObject.transform.position) * 15f;
                grabbingObject.StopObject();

                if (Input.GetKeyDown(KeyCode.F))
                {
                    ProcessDrop();
                }
            }
        }

        public void ProcessDrop()
        {
            isGrabbing = false;

            if (grabbingObject != null)
            {
                grabbingObject.Drop();
            }
            audioSource.Stop();
        }

        private bool IsLookingAtInteractObject(out InteractObjectController interactObject)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, rayLength, layerMask) &&
                hit.transform.TryGetComponent(out interactObject) == true)
            {
                return true;
            }
            else
            {
                interactObject = null;
                return false;
            }
        }

        public void SetWatchingInteractObject()
        {
            selectedObject.TurnOnEmission();
        }
        private void SetNotWatchingInteractObject()
        {
            selectedObject?.TurnOffEmission();
        }
        public void UseExamineLight()
        {
            examineLight.gameObject.SetActive(true);
            //GameManager.Instance.PlayerInventory.FlashLightController.gameObject.SetActive(false);
        }
        public void UseExamineLight(float intensity)
        {
            UseExamineLight();
            examineLight.intensity = intensity;
        }
        public void UseExamineLight(FlashLightState lightState)
        {
            UseExamineLight();
            examineLight.intensity = lightState.Intensity;
            examineLight.innerSpotAngle = lightState.InAngle;
            examineLight.spotAngle = lightState.OutAngle;
        }
        public void UseFlashlight()
        {
            examineLight.gameObject.SetActive(false);
            //GameManager.Instance.PlayerInventory.FlashLightController.gameObject.SetActive(true);
        }
        public void SetCameraTransform(Transform transform)
        {
            //GameManager.Instance.PlayerMoving.enabled = false;

            this.transform.position = transform.position;
            this.transform.rotation = transform.rotation;

            //GameManager.Instance.PlayerMoving.enabled = true;
        }
        public void SetCameraOriginalTransform()
        {
            transform.localPosition = Vector3.up * 0.8f;
        }
        public void TurnOnExamineCamera()
        {
            examineCamera.gameObject.SetActive(true);
        }
        public void TurnOffExamineCamera()
        {
            examineCamera.gameObject.SetActive(false);
        }
    }
}
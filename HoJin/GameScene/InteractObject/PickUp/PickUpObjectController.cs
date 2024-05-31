using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

namespace HoJin.GameScene
{
    [Serializable]
    public struct ExamineData
    {
        [SerializeField] private bool isComeRightAway;
        [SerializeField] private bool isAppearedExaminePanel;
        [SerializeField] private float examineDistance;
        [SerializeField] private Vector3 examineAngle;
        [SerializeField] private float examineLightIntensity;
        [SerializeField] private float zoomMinDistance;
        [SerializeField] private float zoomMaxDistance;

        public ExamineData(Vector3 examineAngle, bool isComeRightAway = false, float examineDistance = 0.4f, float examineLightIntensity = 1f,
            float zoomMinDistance = 0.3f, float zoomMaxDistance = 0.5f, bool isAppearedExaminePanel = true)
        {
            this.isComeRightAway = isComeRightAway;
            this.examineDistance = examineDistance;
            this.examineAngle = examineAngle;
            this.examineLightIntensity = examineLightIntensity;
            this.zoomMinDistance = zoomMinDistance;
            this.zoomMaxDistance = zoomMaxDistance;
            this.isAppearedExaminePanel = isAppearedExaminePanel;
        }

        public bool IsComeRightAway { get => isComeRightAway; }
        public float ExamineDistance { get => examineDistance; }
        public Vector3 ExamineAngle { get => examineAngle; }
        public float ExamineLightIntensity { get => examineLightIntensity; }
        public float ZoomMinDistance { get => zoomMinDistance; }
        public float ZoomMaxDistance { get => zoomMaxDistance; }
        public bool IsAppearedExaminePanel { get => isAppearedExaminePanel; set => isAppearedExaminePanel = value; }
    }

    public class PickUpObjectController : InteractObjectController
    {
        [Header("Pick up object controller")]
        [SerializeField] protected ExamineData examineData;
        //[SerializeField] protected ItemType itemType;
        //[SerializeField] protected ObjectOnWhichBeUsedItem[] objectsToUse;
        [SerializeField] protected bool isCollectibleObject;
        [SerializeField] protected bool isExaminableObject;
        [SerializeField] protected bool isInsideObject;
        [SerializeField] protected bool isDisposable = true;
        protected Vector3 originalPosition;
        protected Quaternion originalRotation;
        protected bool isCollected;
        protected Coroutine comeCoroutine;
        protected float scrollNum;
        protected new Collider collider;
        protected readonly float comeSpeed = 5f;
        protected readonly float rotationSpeed = 20f;
        public ExamineData ExamineData { get => examineData; }



        protected new void Awake()
        {
            base.Awake();
            scrollNum = examineData.ExamineDistance;
            TryGetComponent(out collider);
        }

        protected new void Start()
        {
            base.Start();
            SetOriginalTransform();
        }



        #region Ʋ
        public override void Interact()
        {
            void RotateThis(Quaternion rotation, Vector3 angle)
            {
                transform.LookAt((transform.position) + (rotation * Vector3.forward), rotation * Vector3.up);
                transform.Rotate(angle);
            }
            IEnumerator ComeToPlayerCoroutine(Vector3 target)
            {
                collider.enabled = false;
                while (Vector3.Distance(transform.position, target) > 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * comeSpeed);
                    yield return null;
                }
                collider.enabled = true;
            }
            void ComeToPlayer()
            {
                if (examineData.IsComeRightAway == true)
                {
                    transform.position = GameManager.Instance.PlayerInteract.GetExaminePointTransform().position;
                }
                else
                {
                    comeCoroutine = StartCoroutine(ComeToPlayerCoroutine(GameManager.Instance.PlayerInteract.GetExaminePointTransform().position));
                }
            }

            interactSound.Play();
            GameManager.Instance.UIController.GetSelectPoint().SetActive(false);
            GameManager.Instance.UIController.DisappearInteractUIsAndItemUsingUIs();
            GameManager.Instance.PlayerInteract.SetIsSelect(true);
            GameManager.Instance.PushControlKeyLockState(GameManager.Instance.ObjectSelectControlKeyLock);
            GameManager.Instance.UIController.SetExaminePanel(this);
            GameManager.Instance.PlayerInteract.GetExaminePointTransform().localPosition = Vector3.forward * examineData.ExamineDistance;
            //if (GameManager.Instance.PlayerInventory.FlashLightController.IsUsable == true)
            //{
            //    GameManager.Instance.PlayerInteract.UseExamineLight(examineData.ExamineLightIntensity);
            //}
            GameManager.Instance.PlayerInteract.TurnOnExamineCamera();
            SetLayer(KeyWord.examine);
            TurnOffEmission();
            SetOriginalTransform();
            ComeToPlayer();
            RotateThis(GameManager.Instance.PlayerInteract.transform.rotation, examineData.ExamineAngle);
        }

        public override void GetInputAfterInteract()
        {
            if (isCollectibleObject)
            {
                if (isCollected)
                {
                    if (GameManager.Instance.InputRightClick())
                    {
                        Unselect();
                    }
                }
                else
                {
                    if (GameManager.Instance.InputInteract() || GameManager.Instance.InputRightClick())
                    {
                        Collect();
                    }
                }
            }
            else
            {
                if (GameManager.Instance.InputRightClick())
                {
                    Unselect();
                }
            }
            

            

            if (isExaminableObject)
            {
                if (GameManager.Instance.InputLeftButton())
                {
                    float h = rotationSpeed * Input.GetAxis("Mouse X");
                    float v = rotationSpeed * Input.GetAxis("Mouse Y");

                    gameObject.transform.RotateAround(transform.position, Quaternion.Euler(GameManager.Instance.PlayerInteract.GetMainCamera().transform.rotation.eulerAngles) * Vector3.up, -h * rotationSpeed * Time.deltaTime);
                    gameObject.transform.RotateAround(transform.position, Quaternion.Euler(GameManager.Instance.PlayerInteract.GetMainCamera().transform.rotation.eulerAngles) * Vector3.right, v * rotationSpeed * Time.deltaTime);
                }

                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    float scroll = Input.GetAxis("Mouse ScrollWheel");
                    if (scrollNum + scroll <= examineData.ZoomMaxDistance && scrollNum + scroll >= examineData.ZoomMinDistance)
                    {
                        scrollNum += scroll;

                        GameManager.Instance.PlayerInteract.GetExaminePointTransform().localPosition = scrollNum * Vector3.forward;

                        transform.position = GameManager.Instance.PlayerInteract.GetExaminePointTransform().position;
                    }
                }
            }
        }

        public override void MakeInteract()
        {
            //transform.SetParent(GameManager.Instance.PlayerInventory.transform.GetChild(0));
            //gameObject.SetActive(false);
            //isCollected = true;
            //transform.localPosition = Vector3.forward;
            //GameManager.Instance.PlayerInventory.Inventory.Add(this);
            //for (int i = 0; i < objectsToUse.Length; i++)
            //{
            //    objectsToUse[i].SetReadyToBeUsed(this);
            //}
        }

        public virtual void Unselect()
        {
            collider.enabled = true;
            GameManager.Instance.UIController.GetSelectPoint().SetActive(true);
            GameManager.Instance.UIController.GetInteractUIsAfterSelect().SetActive(false);
            GameManager.Instance.PlayerInteract.SetIsSelect(false);
            GameManager.Instance.PlayerInteract.UseFlashlight();
            GameManager.Instance.PopControlKeyLockState();
            GameManager.Instance.PlayerInteract.TurnOffExamineCamera();
            SetLayer(KeyWord.interact);
            if (isCollectibleObject)
            {
                if (isCollected)
                {
                    gameObject.SetActive(false);
                }
            }
            if (comeCoroutine != null)
            {
                StopCoroutine(comeCoroutine);
            }
            TurnOnEmission();
            transform.localPosition = originalPosition;
            transform.rotation = originalRotation;
            if (isExaminableObject)
            {
                GameManager.Instance.PlayerInteract.GetExaminePointTransform().localPosition = Vector3.forward;
                scrollNum = examineData.ExamineDistance;
            }
            appearInteractUIs.Invoke();
        }

        public virtual void Collect()
        {
            Unselect();
            MakeInteract();
        }
        #endregion



        public void UseAsDisposableItem()
        {
            //if (isDisposable == true)
            //{
            //    Destroy(gameObject);
            //    GameManager.Instance.PlayerInventory.RemoveItemInInventory(this);
            //    for (int i = 0; i < objectsToUse.Length; i++)
            //    {
            //        objectsToUse[i].SetItemUnusableAtThis(this);
            //    }
            //}
        }

        private void SetLayer(string layerName)
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }
        private void SetOriginalTransform()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.rotation;
        }



        [ContextMenu("Collect this")]
        public void CollectThis()
        {
            Collect();
            Debug.Log("Collect this");
        }
    }
}
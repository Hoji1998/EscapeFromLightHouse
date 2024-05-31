using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using System.Linq;
using System;
using UnityEngine.AI;

namespace HoJin.GameScene
{
    [RequireComponent(typeof(Animator))]
    public class Door : InteractObjectController
    {
        private enum DoorType
        {
            RoomDoor = 0,
            Drawer = 1
        }

//#if UNITY_EDITOR
        [Header("Editor properties")]
        [SerializeField] private DoorType doorType;
        private readonly string enemyAutoOpenersName = "EnemyAutoOpeners";
        private readonly string openTriggerName = "OpenTrigger";
        [Space, Space]
//#endif
        [SerializeField] private bool isLocked;
        [SerializeField] private bool justOpened;
        [SerializeField] private AudioSource openSound;
        [SerializeField] private AudioSource closeSound;
        [SerializeField] private AudioSource rattleSound;
        [SerializeField] private AudioSource unlockSound;
        [SerializeField] private AudioSource handleSound;
        [SerializeField] private AudioSource lockSound;
        [SerializeField] private AudioSource slamSound;
        [SerializeField, HideInInspector] private BoxCollider drawerMembrane;
        private Action open;
        private Action close;
        private float closeAngle;
        private bool isOpened;
        private bool isMoving;
        private Animator animator;
        private Collider[] colliders;
        private readonly string openName = "Open";
        private readonly string closeName = "Close";
        private readonly string rattleName = "Rattle";
        private readonly float roomDoorApproximationValue = 0.2f;
        private readonly float slamSpeed = 30f;
        public AudioSource OpenSound { get => openSound; }
        public AudioSource HandleSound { get => handleSound; }
        public AudioSource LockSound { get => lockSound; }
        public Collider[] Colliders { get => colliders; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public bool IsLocked { get => isLocked; set => isLocked = value; }
        public bool IsOpened { get => isOpened; set => isOpened = value; }
        public bool JustOpened { get => justOpened; }
        public Action Open { get => open; }



        protected new void Reset()
        {
            base.Reset();
            Name = "Door";
            interactType = InteractType.InteractKey;

            if (transform.parent.Find("Sounds") == null)
            {
                Transform sounds = new GameObject("Sounds").transform;
                sounds.SetParent(transform.parent);
                Transform open = new GameObject("Open", typeof(AudioSource)).transform;
                Transform close = new GameObject("Close", typeof(AudioSource)).transform;
                Transform rattle = new GameObject("Rattle", typeof(AudioSource)).transform;
                Transform _lock = new GameObject("Lock", typeof(AudioSource)).transform;
                Transform unlock = new GameObject("Unlock", typeof(AudioSource)).transform;
                Transform slam = new GameObject("Slam", typeof(AudioSource)).transform;
                Transform handle = new GameObject("Handle", typeof(AudioSource)).transform;

                open.SetParent(sounds);
                close.SetParent(sounds);
                rattle.SetParent(sounds);
                _lock.SetParent(sounds);
                unlock.SetParent(sounds);
                slam.SetParent(sounds);
                handle.SetParent(sounds);
            }
            else
            {
                Transform sounds = transform.parent.Find("Sounds");

                if (sounds.Find("Open") != null) openSound = sounds.Find("Open").GetComponent<AudioSource>();
                if (sounds.Find("Close") != null) closeSound = sounds.Find("Close").GetComponent<AudioSource>();
                if (sounds.Find("Rattle") != null) rattleSound = sounds.Find("Rattle").GetComponent<AudioSource>();
                if (sounds.Find("Lock") != null) lockSound = sounds.Find("Lock").GetComponent<AudioSource>();
                if (sounds.Find("Unlock") != null) unlockSound = sounds.Find("Unlock").GetComponent<AudioSource>();
                if (sounds.Find("Slam") != null) slamSound = sounds.Find("Slam").GetComponent<AudioSource>();
                if (sounds.Find("Handle") != null) handleSound = sounds.Find("Handle").GetComponent<AudioSource>();
            }

            if (doorType == DoorType.RoomDoor && TryGetComponent(out NavMeshObstacle navMeshObstacle) == false)
            {
                gameObject.AddComponent<NavMeshObstacle>();
            }
        }

        protected new void Awake()
        {
            base.Awake();
            closeAngle = transform.localEulerAngles.y;
            isOpened = false;
            isMoving = false;
            TryGetComponent(out animator);
            colliders = GetComponents<Collider>();
            animator.GetBehaviours<DoorAnimationState>().ToList().ForEach((x) => x.Door = this);
            open = new Action(OpenDoor);
            close = new Action(CloseDoor);
        }



        public override void Interact()
        {
            if (isMoving == false)
            {
                if (isOpened == true)
                {
                    close.Invoke();
                }
                else
                {
                    if (isLocked == true)
                    {
                        Rattle();
                    }
                    else
                    {
                        open.Invoke();
                    }
                }
            }
        }

        public override void MakeInteract()
        {
            isLocked = false;
        }



        private void OpenDoor()
        {
            animator.Play(openName);
            isOpened = true;
        }
        private void CloseDoor()
        {
            animator.Play(closeName);
            isOpened = false;
            closeSound.Play();
        }
        public void Rattle()
        {
            animator.Play(rattleName);
            rattleSound.PlayOneShot(rattleSound.clip);
            //GameManager.Instance.UIController.AppearLog(GetTextAtJsonFile(KeyWord.interact));
        }
        public void Unlock(bool isPlaySound = true)
        {
            if (isPlaySound == true)
            {
                unlockSound.Play(); 
            }
            isLocked = false;
        }
        public void Lock(bool isPlaySound = true)
        {
            if (isPlaySound == true)
            {
                lockSound.Play();
            }
            isLocked = true;
            isOpened = false;
        }
        public void Slam()
        {
            Vector3 GetLocalEulerAngles(Transform transform)
            {
                if (transform.localEulerAngles.y >= 0 && transform.localEulerAngles.y < 180f)
                {
                    return transform.localEulerAngles;
                }
                else if (transform.localEulerAngles.y >= 180f && transform.localEulerAngles.y < 360f)
                {
                    return new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 360f, transform.localEulerAngles.z);
                }
                else
                {
                    return Vector3.zero;
                }
            }
            IEnumerator CloseRoomDoorCoroutine(float speed)
            {
                isMoving = true;
                while (Mathf.Abs(GetLocalEulerAngles(transform).y - closeAngle) >= roomDoorApproximationValue)
                {
                    transform.localEulerAngles -= Vector3.Lerp(GetLocalEulerAngles(transform), closeAngle * Vector3.up, Time.deltaTime * speed);
                    yield return null;
                }
                transform.localEulerAngles = closeAngle * Vector3.up;
                isMoving = false;
            }

            animator.StopPlayback();
            StartCoroutine(CloseRoomDoorCoroutine(slamSpeed));
            isOpened = false;
            slamSound.Play();
        }
#if UNITY_EDITOR
        public void CreateColliders()
        {
            Collider[] colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                DestroyImmediate(colliders[i]);
            }
            switch (doorType)
            {
                case DoorType.RoomDoor:
                    gameObject.AddComponent<BoxCollider>().center += Vector3.forward * 0.01f;
                    gameObject.AddComponent<BoxCollider>().center += Vector3.back * 0.01f;
                    break;
                case DoorType.Drawer:
                    BoxCollider front = gameObject.AddComponent<BoxCollider>();
                    BoxCollider back = gameObject.AddComponent<BoxCollider>();
                    BoxCollider left = gameObject.AddComponent<BoxCollider>();
                    BoxCollider right = gameObject.AddComponent<BoxCollider>();
                    BoxCollider bottom = gameObject.AddComponent<BoxCollider>();
                    drawerMembrane = gameObject.AddComponent<BoxCollider>();

                    Vector3 originalSize = front.size;
                    front.size += 0.95f * front.size.z * Vector3.back;
                    front.center = (0.5f * originalSize.z * Vector3.forward) - (0.5f * front.size.z * Vector3.forward);

                    originalSize = back.size;
                    back.size += 0.95f * back.size.z * Vector3.back;
                    back.center = (0.5f * originalSize.z * Vector3.back) - (0.5f * back.size.z * Vector3.back);

                    originalSize = left.size;
                    left.size += 0.95f * left.size.x * Vector3.left;
                    left.center = (0.5f * originalSize.x * Vector3.left) - (0.5f * left.size.x * Vector3.left);

                    originalSize = right.size;
                    right.size += 0.95f * right.size.x * Vector3.left;
                    right.center = (0.5f * originalSize.x * Vector3.right) - (0.5f * right.size.x * Vector3.right);

                    originalSize = bottom.size;
                    bottom.size += 0.95f * bottom.size.y * Vector3.down;
                    bottom.center = (0.5f * originalSize.y * Vector3.down) - (0.5f * bottom.size.y * Vector3.down);

                    drawerMembrane.size += 0.95f * drawerMembrane.size.y * Vector3.down;
                    drawerMembrane.enabled = false;
                    break;
                default:
                    break;
            }
        }
        public void CreateEnemyAutoOpener()
        {
            //Transform enemyAutoOpeners = transform.parent.Find(enemyAutoOpenersName);
            //if (enemyAutoOpeners == null)
            //{
            //    enemyAutoOpeners = new GameObject(enemyAutoOpenersName).transform;

            //    //BoxCollider frontTrigger = new GameObject(openTriggerName, typeof(BoxCollider), typeof(EnemyDoorOpener)).GetComponent<BoxCollider>();
            //    //BoxCollider backTrigger = new GameObject(openTriggerName, typeof(BoxCollider), typeof(EnemyDoorOpener)).GetComponent<BoxCollider>();

            //    frontTrigger.transform.SetParent(enemyAutoOpeners);
            //    frontTrigger.transform.localPosition = Vector3.zero + Vector3.forward;
            //    frontTrigger.transform.localRotation = Quaternion.identity;
            //    frontTrigger.transform.localScale = Vector3.one;
            //    frontTrigger.isTrigger = true;
            //    backTrigger.transform.SetParent(enemyAutoOpeners);
            //    backTrigger.transform.localPosition = Vector3.zero - Vector3.forward;
            //    backTrigger.transform.localRotation = Quaternion.identity;
            //    backTrigger.transform.localScale = Vector3.one;
            //    backTrigger.isTrigger = true;
            //}
            //enemyAutoOpeners.SetParent(transform.parent);
            //enemyAutoOpeners.SetSiblingIndex(1);
            //enemyAutoOpeners.localPosition = Vector3.zero;
            //enemyAutoOpeners.localRotation = Quaternion.identity;
        }
        public void CreatePlayerAutoCloser()
        {
            //Transform enemyAutoOpeners = transform.parent.Find(enemyAutoOpenersName);
            //if (enemyAutoOpeners == null)
            //{
            //    enemyAutoOpeners = new GameObject(enemyAutoOpenersName).transform;

            //    BoxCollider frontTrigger = new GameObject(openTriggerName, typeof(BoxCollider), typeof(EnemyDoorOpener)).GetComponent<BoxCollider>();
            //    BoxCollider backTrigger = new GameObject(openTriggerName, typeof(BoxCollider), typeof(EnemyDoorOpener)).GetComponent<BoxCollider>();

            //    frontTrigger.transform.SetParent(enemyAutoOpeners);
            //    frontTrigger.transform.localPosition = Vector3.zero + Vector3.forward;
            //    frontTrigger.transform.localRotation = Quaternion.identity;
            //    frontTrigger.transform.localScale = Vector3.one;
            //    frontTrigger.isTrigger = true;
            //    backTrigger.transform.SetParent(enemyAutoOpeners);
            //    backTrigger.transform.localPosition = Vector3.zero - Vector3.forward;
            //    backTrigger.transform.localRotation = Quaternion.identity;
            //    backTrigger.transform.localScale = Vector3.one;
            //    backTrigger.isTrigger = true;
            //}
            //enemyAutoOpeners.SetParent(transform.parent);
            //enemyAutoOpeners.SetSiblingIndex(1);
            //enemyAutoOpeners.localPosition = Vector3.zero;
            //enemyAutoOpeners.localRotation = Quaternion.identity;
        }
#endif
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.IO;

namespace HoJin.GameScene
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private PlayerMovingController player;
        [Header("Attack")]
        [SerializeField] private GameObject attackVideo;
        [SerializeField] private float attackRange;
        [SerializeField] private float playerHeadApproximationConst;
        [SerializeField] private float playerHeadRotationSpeed;
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private Transform head;
        private bool isReadyToChase;


        private void Reset()
        {
            player = FindObjectOfType<PlayerMovingController>(true);
        }
        private void Awake()
        {
            TryGetComponent(out animator);
            TryGetComponent(out navMeshAgent);
            head = transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0);
            isReadyToChase = default;
        }
        private void Start()
        {
            //StartChase();
        }
        private void Update()
        {
            if (isReadyToChase == true)
            {
                Chase();
                if (Vector3.Distance(transform.position, player.PositionOnFloor) <= attackRange)
                {
                    Attack();
                }
            }
        }
        private void LateUpdate()
        {
            //Vector2 headRotation = new Vector2(head.up.x, head.up.z);
            //head.up = -new Vector3((GameManager.Instance.PlayerInteract.transform.position - head.position).normalized.x, 0f, (GameManager.Instance.PlayerInteract.transform.position - head.position).normalized.z);
            Vector3 headToCamera = (GameManager.Instance.PlayerInteract.transform.position - head.position);
            Debug.DrawLine(head.position, head.position - head.right);
            Debug.DrawLine(head.position, head.position - head.up, Color.red);
            Debug.DrawLine(head.position, GameManager.Instance.PlayerInteract.transform.position, Color.green);
            //head.RotateAround(head.position, -head.right, Vector3.Angle(-head.up, new Vector3(headToCamera.x, head.position.y, headToCamera.z)));
        }



        private void StartChase()
        {
            animator.SetTrigger(KeyWord.chase);
            isReadyToChase = true;
        }
        private void Chase()
        {
            navMeshAgent.destination = player.transform.position;
        }
        private bool IsPlayerInsideAttackRange()
        {
            return true;
        }
        private void Attack()
        {
            GameManager.Instance.PushControlKeyLockState(GameManager.Instance.GetAllControlKeyLock());
            gameObject.SetActive(false);
            attackVideo.SetActive(true);
            isReadyToChase = false;
        }
        [ContextMenu("test")]
        public void Test()
        {
            Debug.Log(transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).name);
            Debug.Log(new Quaternion(Mathf.Sqrt(0.5f), 0f, Mathf.Sqrt(0.5f), 0).eulerAngles);
            Debug.Log(new Quaternion(0f, Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f)).eulerAngles);
        }
    }
}
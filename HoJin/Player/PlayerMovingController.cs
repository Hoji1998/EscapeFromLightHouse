using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace HoJin.GameScene
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovingController : GameDirector
    {
        [Header("Model")]
        public GameObject model;
        [Header("Gravity")]
        [SerializeField] private float gravityReturningForce;
        [Header("RotateLock")]
        public bool rotateLockEvent = false;
        public Vector3 LockRotation = Vector3.zero;
        [Header("Moving Component")]
        [SerializeField] private bool m_IsWalking;
        private bool isRunning;
        public float m_WalkSpeed;
        public float m_RunSpeed;
        public bool isPause;
        public bool isStunned = false;
        public Vector3 gravityForce = new Vector3(0f, -10f, 0f);
        [SerializeField] private AudioSource[] sounds;
        [SerializeField] private float m_RunstepLenghten;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private float m_StepInterval;

        
        private Vector3 initGravityForce = new Vector3(0f, -10f, 0f);
        private Camera m_Camera;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private float m_StepCycle;
        private float m_NextStep;
        public Camera Camera { get => m_Camera; }

        private void Awake()
        {
            isRunning = false;
            isPause = false;
            TryGetComponent(out m_CharacterController);
            m_Camera = Camera.main;
        }

        private void Start()
        {
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_MouseLook.Init(model.transform, m_Camera.transform);
            //LoadPlayer();
        }

        private void FixedUpdate()
        {
            if (isPause)
                return;
            if (!GameManager.Instance.ControlKeyLocks.Peek().IsCameraLocked)
            {
                UpdateCamera();
            }

            float speed;
            if (!isStunned)
            {
                GetInput(out speed);
            }
            else
            {
                speed = 0f;
            }
            
            Vector3 desiredMove = (model.transform.forward * m_Input.y) + (model.transform.right * m_Input.x);
            RaycastHit hitInfo;
            Physics.SphereCast(model.transform.position, m_CharacterController.radius, model.transform.up * -1f, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
            m_MoveDir = desiredMove * speed + gravityForce;

            if (GameManager.Instance.ControlKeyLocks.Peek().IsMovingLocked == false)
            {
                if (contactPlatform != null && m_CharacterController.isGrounded)
                {
                    ContactMovingPlatform(); //2022. 08. 13. jiho
                }
                else 
                {
                    m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
                }
            }

            ProgressStepCycle(speed);
            if (!GameManager.Instance.ControlKeyLocks.Peek().IsCameraLocked)
            {
                UpdateCameraPosition(speed);
            }
        }

        public void InitGravityDirection(bool wallGravity)
        {
            if (wallGravity)
            {
                gravityForce = initGravityForce;
                return;
            }
            m_UseHeadBob = false;
            StopCoroutine(LerpGravityDirection());
            StartCoroutine(LerpGravityDirection());
        }

        public IEnumerator LerpGravityDirection()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                gravityForce = Vector3.Slerp(gravityForce, initGravityForce, gravityReturningForce * Time.fixedDeltaTime);

                if (Vector3.Distance(gravityForce, initGravityForce) < 0.021f)
                {
                    gravityForce = initGravityForce;
                    break;
                }
            }
            m_UseHeadBob = true;
        }

        public Vector3 PositionOnFloor { get { return model.transform.position - (Vector3.up * (m_CharacterController.height - (m_CharacterController.center.y * 2)) * 0.5f); } }
        public void UpdateCamera()
        {
            //m_MouseLook.LookRotation(transform, m_Camera.transform, 데이터 컨트롤러.설정.감도);
            m_MouseLook.LookRotation(transform, model.transform, m_Camera.transform, 2f, gravityForce, rotateLockEvent, LockRotation);
        }

        public void UpdateCameraPosition(float speed, bool isJustUpdate = false)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }

            if ((m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded) || isJustUpdate is true)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)), m_IsWalking);
                newCameraPosition = m_Camera.transform.localPosition;
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }
        public void LookForward(Transform transform)
        {
            m_MouseLook.LookForward(transform);
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }
        private int footSoundCount = 0;
        private void PlayFootStepAudio()
        {
            sounds[footSoundCount].Play();
            footSoundCount = footSoundCount == sounds.Length - 1 ? footSoundCount = 0 : footSoundCount += 1;
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
        }

        private void GetInput(out float speed)
        {
            GameManager.Instance.InputGetMove(out float horizontal, out float vertical);

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            if (horizontal != 0 || vertical != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    m_IsWalking = false;
                    isRunning = true;
                }
                else
                {
                    m_IsWalking = true;
                    isRunning = false;
                }
            }
            else
            {
                m_IsWalking = false;
                isRunning = false;
            }
#endif
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        //private void LoadPlayer()
        //{
        //    if (DataController.Instance.StageData != null)
        //    {
        //        transform.position = DataController.Instance.StageData.SpawnPosition;
        //        transform.eulerAngles = DataController.Instance.StageData.SpawnRotation;
        //        m_MouseLook.Init(transform, m_Camera.transform);
        //        StartCoroutine(GameManager.Instance.PostEffect.Eye_Open_Coroutine());
        //    }
        //}

        //2022. 08. 13. jiho
        #region MovingPlatform
        [HideInInspector] public GameObject contactPlatform;
        private MovingPlatformEvent movingPlatform;
        private Vector3 platformPosition;
        private Vector3 distance;
        private void ContactMovingPlatform()
        {
            //움직이는 발판에서 같이 움직이기
            if (contactPlatform != null)
            {
                //바닥을 밟고 있고, 좌우로 움직이고 있지 않은 경우
                if (!m_IsWalking && !isRunning)
                {
                    //캐릭터의 위치는 밟고 있는 플랫폼과 distance 만큼 떨어진 위치
                    transform.position = contactPlatform.transform.position - distance;
                    if (m_UseHeadBob)
                        m_UseHeadBob = false;
                }
                else if (m_IsWalking || isRunning && movingPlatform.IsMove) //움직이는 발판 위에서 움직임
                {
                    m_MoveDir.x += movingPlatform.currentMoveDirection.normalized.x * movingPlatform.speed;
                    m_MoveDir.z += movingPlatform.currentMoveDirection.normalized.z * movingPlatform.speed;
                    m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                    if (!m_UseHeadBob)
                        m_UseHeadBob = true;
                }
                else //발판 위
                {
                    m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                    if (!m_UseHeadBob)
                        m_UseHeadBob = true;
                }
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //접촉한 오브젝트의 태그가 platform 일 때,
            if (hit.gameObject.CompareTag("MovingPlatform"))
            {
                //접촉한 순간의 오브젝트 위치를 저장
                contactPlatform = hit.gameObject;
                platformPosition = contactPlatform.transform.position;
                movingPlatform = contactPlatform.GetComponent<MovingPlatformEvent>();
                //접촉한 순간의 오브젝트 위치와 캐릭터 위치의 차이를 distance에 저장
                distance = platformPosition - model.transform.position;
            }
            else
            {
                contactPlatform = null;
            }
        }
        #endregion
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using HoJin.GameScene;
using DG.Tweening;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        private float smoothGravityTime = 5f;
        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private Quaternion m_CharacterTargetGravityRot;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            postProcessingControl = Camera.main.GetComponent<PostProcessingControl>();
        }
        bool rotateLock = false;
        private PostProcessingControl postProcessingControl;
        public void LookRotation(Transform character, Transform model, Transform camera, float sensitivity, Vector3 gravityForce, bool rotateLockEvent, Vector3 lockRotation)
        {
            float yRot = GameManager.Instance.InputGetMouseAxis("Mouse X") * sensitivity;
            float xRot = GameManager.Instance.InputGetMouseAxis("Mouse Y") * sensitivity;

            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
            // 카메라 x축 회전 제한 두는거 물어보고 지우기
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
            // 카메라 전환 어떻게 할껀지 물어보고 지우기
            if ((model.localRotation.y > lockRotation.y + 0.5f || model.localRotation.y < lockRotation.y - 0.5f 
                || model.localRotation.x > lockRotation.x + 0.5f || model.localRotation.x < lockRotation.x - 0.5f) 
                && !rotateLock && rotateLockEvent)
            {
                rotateLock = true;
                model.DOLocalRotate(lockRotation, 0.35f).OnComplete(ReturnRotateLock);
            }

            if (!rotateLock)
            {
                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                model.localRotation = Quaternion.Slerp(model.localRotation, m_CharacterTargetRot, smoothTime * Time.fixedDeltaTime);
            }
            else
            {
                postProcessingControl.CameraShakeOn();
                m_CharacterTargetRot = Quaternion.Euler(lockRotation);
            }
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, smoothTime * Time.fixedDeltaTime);

            if (gravityForce.y < 0)
            {
                character.transform.localRotation = Quaternion.Slerp(character.transform.localRotation,
                Quaternion.identity,
                smoothGravityTime * Time.fixedDeltaTime);
            }
        }
        private void ReturnRotateLock()
        {
            rotateLock = false;
        }
        public void Rotate(Vector3 rotation)
        {
            m_CharacterTargetRot.eulerAngles = rotation;
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public void LookForward(Transform transform)
        {
            m_CharacterTargetRot = Quaternion.Euler(transform.eulerAngles);
            m_CameraTargetRot = Quaternion.identity;
        }
    }
}

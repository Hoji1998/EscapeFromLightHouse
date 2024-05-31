using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene.Player
{
    public class PlayerMoving : MonoBehaviour
    {
        #region moving
        [SerializeField] private float movingSpeed;
        [SerializeField] private float movingAcceleration;
        private Vector2 movingDirection;
        private Vector2 movingVelocity;
        private Vector2 currentMovingVector;
        #endregion
        private Vector2 cameraDirection;
        private Quaternion horizontalRotation;
        private Quaternion verticalRotation;
        #region camera rotation
        #endregion
        private CharacterController characterController;
        private new Camera camera;



        private void Awake()
        {
            TryGetComponent(out characterController);
            if (transform.GetChild(0).TryGetComponent(out camera) == false)
            {
                //Debug.LogError("No camera found");
            }
        }
        private void Update()
        {
            currentMovingVector = Vector2.SmoothDamp(currentMovingVector, movingDirection, ref movingVelocity, movingAcceleration);
        }
        private void FixedUpdate()
        {
            characterController.Move(new Vector3(currentMovingVector.x, 0, currentMovingVector.y) * Time.deltaTime * movingSpeed);
            //characterController.Move(new Vector3(0, 0, 1f) * Time.fixedDeltaTime * movingSpeed);
        }



        public void Move(Vector2 vector2)
        {
            movingDirection = transform.rotation * vector2;
        }
        public void RotateCamera(Vector2 vector2)
        {
            cameraDirection = vector2;
        }
    }
}
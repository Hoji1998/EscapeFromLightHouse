using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using HoJin.GameScene.Player;

namespace HoJin.GameScene
{
    public class KeyInput : MonoBehaviour
    {
        public struct KeyLock
        {
            public enum KeyLockType
            {
                Moving = 0,
                Camera = 1,
                Cursor = 2
            }

            private bool isMovingLock;
            private bool isCameraLock;
            private bool isCursorLock;
            public bool IsMovingLock { get => isMovingLock; }
            public bool IsCameraLock { get => isCameraLock; }
            public bool IsCursorLock { get => isCursorLock; }

            public KeyLock(bool isMovingLock, bool isCameraLock, bool isCursorLock)
            {
                this.isMovingLock = isMovingLock;
                this.isCameraLock = isCameraLock;
                this.isCursorLock = isCursorLock;
            }

            public void SetCursor()
            {
                Cursor.visible = isCursorLock;
                if (isCursorLock == true)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            public void ChangeOneLockState(KeyLockType keyLockType, bool value)
            {
                switch (keyLockType)
                {
                    case KeyLockType.Moving:
                        isMovingLock = value;
                        break;
                    case KeyLockType.Camera:
                        isCameraLock = value;
                        break;
                    case KeyLockType.Cursor:
                        isCursorLock = value;
                        break;
                    default:
                        break;
                }
            }
        }

        [SerializeField] private PlayerMoving playerMoving;
        [SerializeField] private PlayerWeapon playerWeapon;
        private EscapeFromLighthouseInputActions inputActions;
        private Stack<KeyLock> keyLockStack;
        public Stack<Action> Move { get; set; }
        public Stack<Action> Look { get; set; }



        private void Awake()
        {
            inputActions = new EscapeFromLighthouseInputActions();
            
            Move = new Stack<Action>(2);
            Look = new Stack<Action>(1);
            Move.Push(() => playerMoving.Move(inputActions.Player.Move.ReadValue<Vector2>()));
            Look.Push(() => playerMoving.RotateCamera(inputActions.Player.Look.ReadValue<Vector2>()));

            keyLockStack = new Stack<KeyLock>(3);
            keyLockStack.Push(new KeyLock(false, false, true));
        }
        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.Player.Enable();
            inputActions.Player.Move.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
            inputActions.Player.Disable();
            inputActions.Player.Move.Disable();
        }



        public void OnMove(InputValue inputValue)
        {
            if (keyLockStack.Peek().IsMovingLock == true)
            {
                Move.Peek().Invoke();
            }
        }
        public void OnLook(InputValue inputValue)
        {
            if (keyLockStack.Peek().IsCameraLock == true)
            {
                Look.Peek().Invoke();
            }
        }
        public void OnFire(InputValue inputValue)
        {
            if (GUIManager.Instance.pauseManager.isPause)
                return;

            playerWeapon.Shoot();
        }
        public void OnZoom(InputValue inputValue)
        {
            GUIManager.Instance.ZoomCrossHair();
            
        }
        public void OnPause(InputValue inputValue)
        {
            GUIManager.Instance.pauseManager.SetPause();
        }

        public void PushKeyLockStack(KeyLock keyLock)
        {
            keyLockStack.Push(keyLock);
            keyLockStack.Peek().SetCursor();
        }
        public void PopKeyLockStack()
        {
            keyLockStack.Pop();
            keyLockStack.Peek().SetCursor();
        }
    }
}
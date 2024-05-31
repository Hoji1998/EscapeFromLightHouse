using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class GameManager : GameDirector
    {
        public delegate bool InputEvent();

        public struct KeyLock
        {
            #region 필드
            private bool isCameraLocked;
            private bool isMovingLocked;
            private bool isInteractLocked;
            private bool isFlashlightLocked;
            private bool isInventoryLocked;
            private bool isLeftClickLocked;
            private bool isRightClickLocked;
            private bool isSettingLocked;
            private bool isCloseUILocked;
            private bool isCursorLocked;
            #endregion

            #region 속성
            public bool IsCameraLocked { get => isCameraLocked; }
            public bool IsMovingLocked  { get => isMovingLocked; }
            public bool IsInteractLocked { get => isInteractLocked; }
            public bool IsFlashLightLocked { get => isFlashlightLocked; }
            public bool IsInventoryLocked { get => isInventoryLocked; }
            public bool IsLeftButtonLocked { get => isLeftClickLocked; }
            public bool IsRightClickLocked { get => isRightClickLocked; }
            public bool IsSettingLocked { get => isSettingLocked; }
            public bool IsCloseUILocked { get => isCloseUILocked; }
            public bool IsCursorLocked { get => isCursorLocked; }
            #endregion



            public KeyLock(bool isCameraLocked, bool isMovingLocked, bool isInteractLocked, bool isFlashLightLocked,
                bool isInventoryLocked, bool isLeftClickLocked, bool isRightClickLocked, bool isSettingLocked, bool isCloseUILocked, bool isCursorLocked)
            {
                this.isCameraLocked = isCameraLocked;
                this.isMovingLocked = isMovingLocked;
                this.isInteractLocked = isInteractLocked;
                this.isFlashlightLocked = isFlashLightLocked;
                this.isInventoryLocked = isInventoryLocked;
                this.isLeftClickLocked = isLeftClickLocked;
                this.isRightClickLocked = isRightClickLocked;
                this.isSettingLocked = isSettingLocked;
                this.isCloseUILocked = isCloseUILocked;
                this.isCursorLocked = isCursorLocked;
            }

            public void Print()
            {
                Debug.Log("isCameraLocked : " + isCameraLocked);
                Debug.Log("isMovingLocked : " + isMovingLocked);
                Debug.Log("isInteractLocked : " + isInteractLocked);
                Debug.Log("isFlashLightLocked : " + isFlashlightLocked);
                Debug.Log("isInventoryLocked : " + isInventoryLocked);
                Debug.Log("isLeftButtonLocked : " + isLeftClickLocked);
                Debug.Log("isRightClickLocked : " + isRightClickLocked);
                Debug.Log("isSettingLocked : " + isSettingLocked);
                Debug.Log("isCloseUILocked : " + isCloseUILocked);
                Debug.Log("isCursorLocked : " + isCursorLocked);
            }

            public bool IsEqual(KeyLock controlKeyLock)
            {
                if (isCameraLocked == controlKeyLock.isCameraLocked &&
                    isMovingLocked == controlKeyLock.isMovingLocked &&
                    isInteractLocked == controlKeyLock.isInteractLocked &&
                    isFlashlightLocked == controlKeyLock.isFlashlightLocked &&
                    isInventoryLocked == controlKeyLock.isInventoryLocked &&
                    isLeftClickLocked == controlKeyLock.isLeftClickLocked &&
                    isRightClickLocked == controlKeyLock.isRightClickLocked &&
                    isSettingLocked == controlKeyLock.isSettingLocked &&
                    isCloseUILocked == controlKeyLock.isCloseUILocked &&
                    isCursorLocked == controlKeyLock.isCursorLocked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region 필드
        [SerializeField] private PlayerMovingController playerMoving;
        [SerializeField] private PlayerInteractController playerInteract;
        [SerializeField] private PlayerWeapon playerWeapon; //2022. 08. 01. jiho
        //[SerializeField] private PlayerInventoryController playerInventory;
        [SerializeField] private UIController uIController;
        //[SerializeField] private PostEffectControl postEffect;
        //[SerializeField] private SettingController settingController;
        private Stack<KeyLock> keyLockStack;
        private KeyLock inventoryControlKeyLock;
        private KeyLock objectSelectControlKeyLock;
        private KeyLock settingControlKeyLock;
        private KeyLock allControlKeyLock;
        private KeyLock quizControlKeyLock;
        private KeyLock onlyCursor;
        #endregion

        #region 속성
        public PlayerMovingController PlayerMoving { get => playerMoving; }
        public PlayerInteractController PlayerInteract { get => playerInteract; }
        public PlayerWeapon PlayerWeapon { get => playerWeapon; } //2022. 08. 01. jiho
        //public PlayerInventoryController PlayerInventory { get => playerInventory; }
        public UIController UIController { get => uIController; }
        //public PostEffectControl PostEffect { get => postEffect; }
        //public SettingController SettingController { get => settingController; }
        public Stack<KeyLock> ControlKeyLocks { get => keyLockStack; }
        public static GameManager Instance { get; set; }
        public KeyLock InventoryControlKeyLock { get => inventoryControlKeyLock; }
        public KeyLock ObjectSelectControlKeyLock { get => objectSelectControlKeyLock; }

        public KeyLock GetSettingControlKeyLock()
        {
            return settingControlKeyLock;
        }

        public KeyLock GetAllControlKeyLock()
        {
            return allControlKeyLock;
        }

        public KeyLock GetQuizControlKeyLock()
        {
            return quizControlKeyLock;
        }
        #endregion



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            keyLockStack = new Stack<KeyLock>();
            PushControlKeyLockState(new KeyLock(false, false, false, false, false, false, false, false, false, true));

                                                 // 카메라  이동   상호작용  손전등  인벤토리  좌클릭  우클릭  setting  UI닫기    커서
            inventoryControlKeyLock = new KeyLock(   true,  true,  true,     true,   false,    true,   true,   true,    false,    false);
            objectSelectControlKeyLock = new KeyLock(true,  true,  false,    true,   true,     false,  false,  false,   false,    false);
            settingControlKeyLock = new KeyLock(     true,  true,  true,     true,   true,     true,   true,   false,   true,     false);
            allControlKeyLock = new KeyLock(         true,  true,  true,     true,   true,     true,   true,   true,    true,     true);
            quizControlKeyLock = new KeyLock(        true,  true,  true,     true,   true,     false,  false,  false,   true,     false);
            onlyCursor = new KeyLock(                true,  true,  true,     true,   true,     true,   true,   true,    true,     false);
        }



        public void PushControlKeyLockState(KeyLock controlKeyLock)
        {
            ControlKeyLocks.Push(controlKeyLock);
            SetCursorState(ControlKeyLocks.Peek().IsCursorLocked);
        }

        public void PopControlKeyLockState()
        {
            if (keyLockStack.Count > 1)
            {
                ControlKeyLocks.Pop();
                SetCursorState(ControlKeyLocks.Peek().IsCursorLocked);
            }
        }



        public void PushOnlyCursorControlKeyLock()
        {
            keyLockStack.Push(onlyCursor);
            SetCursorState(keyLockStack.Peek().IsCursorLocked);
        }

        public float InputGetMouseAxis(string mouse)
        {
            if(!ControlKeyLocks.Peek().IsCameraLocked)
            {
                return Input.GetAxis(mouse);
            }
            else
            {
                return 0;
            }
        }

        public void InputGetMove(out float horizontal, out float vertical)
        {
            horizontal = 0;
            vertical = 0;

            if (!ControlKeyLocks.Peek().IsMovingLocked)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    vertical += 0.001f;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    vertical -= 0.001f;
                }

                if (Input.GetKey(KeyCode.D))
                {
                    horizontal += 0.001f;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    horizontal -= 0.001f;
                }
            }
            else
            {
                horizontal = 0;
                vertical = 0;
            }
        }

        public bool InputInteract()
        {
            if (!ControlKeyLocks.Peek().IsInteractLocked)
            {
                return Input.GetKeyDown(KeyCode.E);
            }
            else
            {
                return false;
            }
        }

        public bool InputFlashLight()
        {
            if (!ControlKeyLocks.Peek().IsFlashLightLocked)
            {
                return Input.GetKeyDown(KeyCode.F);
            }
            else
            {
                return false;
            }
        }

        public bool InputInventory()
        {
            if (!ControlKeyLocks.Peek().IsInventoryLocked)
            {
                return Input.GetKeyDown(KeyCode.Tab);
            }
            else
            {
                return false;
            }
        }

        public bool InputLeftClick()
        {
            if (!ControlKeyLocks.Peek().IsLeftButtonLocked)
            {
                return Input.GetMouseButtonDown(0);
            }
            else
            {
                return false;
            }
        }

        public bool InputLeftButton()
        {
            if (!ControlKeyLocks.Peek().IsLeftButtonLocked)
            {
                return Input.GetMouseButton(0);
            }
            else
            {
                return false;
            }
        }

        public bool InputRightClick()
        {
            if (!ControlKeyLocks.Peek().IsRightClickLocked)
            {
                return Input.GetMouseButtonDown(1);
            }
            else
            {
                return false;
            }
        }

        public bool InputItemUsingDown()
        {
            if (!ControlKeyLocks.Peek().IsRightClickLocked)
            {
                return Input.GetMouseButtonDown(1);
            }
            else
            {
                return false;
            }
        }

        public bool InputItemUsingUp()
        {
            if (!ControlKeyLocks.Peek().IsRightClickLocked)
            {
                return Input.GetMouseButtonUp(1);
            }
            else
            {
                return false;
            }
        }

        public bool InputCloseUI()
        {
            if (!ControlKeyLocks.Peek().IsCloseUILocked)
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
            else
            {
                return false;
            }
        }

        public bool InputSetting()
        {
            if (!keyLockStack.Peek().IsSettingLocked)
            {
                return Input.GetKeyDown(KeyCode.Escape);
            }
            else
            {
                return false;
            }
        }



        [ContextMenu("Print control key lock")]
        public void PrintControlKeyLock()
        {
            Debug.Log(keyLockStack.Count);
            keyLockStack.Peek().Print();
        }
    }
}
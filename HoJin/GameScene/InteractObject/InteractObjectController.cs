using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace HoJin.GameScene
{
    [Serializable]
    public class InteractData : UnityEngine.Object  { }
    [Serializable]
    public enum InteractType
    {
        LeftMouse = 0,
        InteractKey = 1,
    }
    public class InteractObjectController : InteractObject
    {
        #region �ʵ�
        [SerializeField] private new string name;
        [SerializeField] private AudioSource[] sounds;
        [SerializeField] protected AudioSource interactSound;
        [SerializeField] protected InteractType interactType;
        [SerializeField, HideInInspector] protected int serialNumber;
        [SerializeField, HideInInspector] private bool isUseSerialNumber;

        private GameManager.InputEvent inputInteractKey;
        private Action onPointer;
        private Action interactEvent;
        protected Action appearInteractUIs;
        protected Action disappearInteractUIs;
        protected readonly int interactIndex = 0;
        #endregion

        #region �Ӽ�
        public string Name { get => name; set => name = value; }
        public Action OnPointer { get => onPointer; set => onPointer = value; }
        public Action AppearInteractUIs { get => appearInteractUIs; }
        public Action DisappearInteractUIs { get => disappearInteractUIs; }
        public bool IsUseSerialNumber { get => isUseSerialNumber; }
        public Action InteractEvent { get => interactEvent; }
        public GameManager.InputEvent InputInteractKey { get => inputInteractKey; }
        public AudioSource[] GetSounds()
        {
            return sounds;
        }

        public int SerialNumber => serialNumber;
        public void SetSerialNumber(int value)
        {
            serialNumber = value;
        }
        #endregion



        protected void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer(KeyWord.interact);
        }
        protected void Start()
        {
            switch (interactType)
            {
                case InteractType.LeftMouse:
                    inputInteractKey = new GameManager.InputEvent(GameManager.Instance.InputLeftClick);
                    meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
                    break;
                case InteractType.InteractKey:
                    inputInteractKey = new GameManager.InputEvent(GameManager.Instance.InputInteract);
                    meshRenderers = null;
                    break;
                default:
                    break;
            }
            interactEvent = new Action(Interact);
        }



        #region Ʋ
        public virtual void Interact()
        {
            return;
        }

        public virtual void GetInputAfterInteract()
        {
            return;
        }

        public virtual void MakeInteract()
        {
            gameObject.SetActive(false);
        }
        #endregion



        public void SetExamineUIImage_Before_Select()
        {
            appearInteractUIs.Invoke();
        }

        

        public void SetCollider(bool value)
        {
            GetComponent<Collider>().enabled = value;
        }

        private void PlayAudio(int index)
        {
            sounds[index].Play();
        }



        [ContextMenu("Warp player to this")]
        public void WarpPlayerToThis()
        {
            GameObject.Find("Player").transform.position = transform.position;
        }
    }
}
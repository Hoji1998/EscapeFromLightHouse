using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;

namespace HoJin.Test
{
    public class InteractObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private PlayerInteractController player;
        [SerializeField, ReadOnly] private MeshRenderer[] meshRenderers;
        private readonly string emissiveColor = "_EmissiveColor";
        private readonly Color interactColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        private readonly Color noInteractColor = Color.black;

        private void Reset()
        {
            player = FindObjectOfType<PlayerInteractController>(true);
            meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        }
        private void OnMouseEnter()
        {
            //Debug.Log("enter");
            //Debug.Log(Vector3.Distance(transform.position, GameManager.Instance.PlayerInteract.transform.position));
            SetInteractState();
        }
        private void OnMouseExit()
        {
            //Debug.Log("exit");
            SetNoInteractState();
        }

        private void SetInteractState()
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.SetColor(emissiveColor, interactColor);
            }
        }
        private void SetNoInteractState()
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.SetColor(emissiveColor, noInteractColor);
            }
        }
    }
}
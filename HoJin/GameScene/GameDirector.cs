using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

namespace HoJin.GameScene
{
    public class GameDirector : MonoBehaviour
    {
        protected void SetCursorState(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        protected void SetLayer(GameObject selectedObject, string layerName)
        {
            selectedObject.layer = LayerMask.NameToLayer(layerName);
            if (selectedObject.transform.childCount != 0)
            {
                foreach (var item in selectedObject.transform.GetComponentsInChildren<Transform>())
                {
                    item.gameObject.layer = LayerMask.NameToLayer(layerName);
                }
            }
        }
    }
}
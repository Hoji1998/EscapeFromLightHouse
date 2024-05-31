using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoJin.GameScene
{
    public class ObjectThatInteractUsingMouse : InteractObject
    {
        public virtual void OnMouseEnter()
        {
            TurnOnEmission();
        }
        public virtual void OnMouseDown()
        {
            
        }
        public virtual void OnMouseUp()
        {
            
        }
        public virtual void OnMouseExit()
        {
            TurnOffEmission();
        }
    }
}
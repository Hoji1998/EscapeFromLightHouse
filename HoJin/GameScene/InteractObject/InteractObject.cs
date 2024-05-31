using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class InteractObject : GameDirector
    {
        protected MeshRenderer[] meshRenderers;



        protected void Awake()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        }



        public void TurnOnEmission()
        {
            for (int i = 0; i < meshRenderers?.Length; i++)
            {
                meshRenderers[i].material.EnableKeyword("_EMISSION");
            }
        }
        public void TurnOffEmission()
        {
            for (int i = 0; i < meshRenderers?.Length; i++)
            {
                meshRenderers[i].material.DisableKeyword("_EMISSION");
            }
        }
    }
}
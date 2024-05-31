using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class Open : DoorAnimationState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Door.OpenSound.Play();
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (Door.JustOpened == true)
            {
                Destroy(Door);
            }
        }
    }
}
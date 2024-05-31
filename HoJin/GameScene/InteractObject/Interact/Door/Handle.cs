using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class Handle : DoorAnimationState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Door.IsMoving = true;
            Door.HandleSound.Play();
        }
    }
}
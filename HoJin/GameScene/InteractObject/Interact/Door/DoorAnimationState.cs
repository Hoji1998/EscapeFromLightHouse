using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace HoJin.GameScene
{
    public class DoorAnimationState : StateMachineBehaviour
    {
        public Door Door { get; set; }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Door.IsMoving = true;
            for (int i = 0; i < Door.Colliders.Length; i++)
            {
                Door.Colliders[i].enabled = false;
            }
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Door.IsMoving = false;
            for (int i = 0; i < Door.Colliders.Length; i++)
            {
                Door.Colliders[i].enabled = true;
            }
        }
    }
}
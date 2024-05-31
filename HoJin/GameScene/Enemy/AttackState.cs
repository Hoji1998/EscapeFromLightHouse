using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoJin.GameScene
{
    public class AttackState : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(KeyWord.isAttacked, true);
            animator.transform.TryGetComponent(out Enemy enemy);
            //animator.transform.LookAt(enemy.Player.PositionOnFloor);
        }
    }
}
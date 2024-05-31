using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMover : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController == null)
        {
            return;
        }

        LevelManager.Instance.StageClear();
    }
}

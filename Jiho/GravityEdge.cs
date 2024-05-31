using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEdge : MonoBehaviour
{
    [Header("EdgeArea")]
    [SerializeField] private Collider anotherEdgeColl;
    [SerializeField] private Collider prepareEdgeColl;
    private void OnDrawGizmos()
    {
        if (anotherEdgeColl == null || prepareEdgeColl == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, anotherEdgeColl.transform.position);
        Gizmos.DrawLine(transform.position, prepareEdgeColl.transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();

        if (characterController != null && prepareEdgeColl != null)
        {
            prepareEdgeColl.enabled = true;
            anotherEdgeColl.enabled = false;
        }
    }
}

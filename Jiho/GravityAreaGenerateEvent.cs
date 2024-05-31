using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAreaGenerateEvent : InteractEvent
{
    [Header("Guide Line")]
    public LineRenderer guideLine;
    [HideInInspector] public List<GameObject> gravityAreasList;
    private GravityAreaEvent[] gravityAreas; 
    private void Awake()
    {
        gravityAreas = GetComponentsInChildren<GravityAreaEvent>();
        base.SetGuideLine(guideLine);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.3f, 0.3f, 0.3f));
    }

    public override void StartInteractEvent()
    {
        foreach (GravityAreaEvent area in gravityAreas)
        {
            area.StartInteractEvent();
        }
    }
    public override void StopInteractEvent()
    {
        foreach (GravityAreaEvent area in gravityAreas)
        {
            area.StopInteractEvent();
        }
    }
}

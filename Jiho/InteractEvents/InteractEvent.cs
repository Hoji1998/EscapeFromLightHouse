using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractEvent : MonoBehaviour
{
    private LineRenderer guideLine;
    public virtual void SetGuideLine(LineRenderer line)
    {
        guideLine = line;
    }
    public LineRenderer GetGuideLine()
    {
        return guideLine;
    }
    public virtual void StartInteractEvent()
    {
        return;
    }

    public virtual void StopInteractEvent()
    {
        return;
    }

    public virtual void ResetInteractEvent()
    {
        return;
    }
}

using UnityEngine;

public class ReflectObject : MonoBehaviour
{
    [Header("Reflect Component")]
    public GameObject ReflectLaser;
    public ReflectReceiveObject reflectReceiveObject;
    public LaserControl reflectLaserScript;
    public LaserControl LaserReceiveObject;

    [Header("Refelct State")]
    public bool isReflect = false;

    [Header("Reflect Direction")]
    public Vector3 LaserDirection_Receive;
    public GameObject HitPoint;
    public void ProcessAbility()
    {
        if (!AuthorizedRefelct())
            return;

        UpdateReflect();
    }

    #region Reflect Interaction
    private bool AuthorizedRefelct()
    {
        if (!isReflect)
            return false;

        if (reflectLaserScript.StartLaser.reflectObject == null && reflectLaserScript.StartLaser != null)
        {
            reflectReceiveObject.FindStopReflectObject(this);
            return false;
        }

        if (LaserReceiveObject == null)
        {
            reflectReceiveObject.FindStopReflectObject(this);
            return false;
        }

        if (LaserReceiveObject.ParentReflectObject != null)
        {
            if (!LaserReceiveObject.ParentReflectObject.ReflectLaser.activeSelf)
            {
                reflectReceiveObject.FindStopReflectObject(this);
                return false;
            }
        }

        if (LaserReceiveObject.reflectObject != this && LaserReceiveObject != null)
        {
            reflectReceiveObject.FindStopReflectObject(this);
            return false;
        }

        if (LaserDirection_Receive == null || HitPoint == null)
            return false;

        return true;
    }

    private void UpdateReflect()
    {
        reflectLaserScript.LaserDirection = Vector3.Reflect(LaserDirection_Receive, HitPoint.transform.forward); //레이저 방향 벡터 계산

        ReflectLaser.transform.SetPositionAndRotation(HitPoint.transform.position,
            Quaternion.LookRotation(Vector3.Reflect(LaserDirection_Receive * -1f, HitPoint.transform.forward))); //레이저 위치 지정
    }
    
    #endregion
}

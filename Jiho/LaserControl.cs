using UnityEngine;
using HoJin.GameScene;

public class LaserControl : MonoBehaviour
{
    [Header("Laser Components")]
    [SerializeField] private GameObject changingScaleObject;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject HitPointSFX;
    public LaserControl StartLaser;

    [Header("Raycast Option")]
    [SerializeField] private float maximumLaycastDistance = 8.5f;

    [Header("Reflect Laser Components")]
    [SerializeField] private bool IsReflectLaser = false;
    public ReflectObject ParentReflectObject;
    public Vector3 LaserDirection;

    [Header("Receive Object")]
    public ReflectObject reflectObject;
    public LaserInteractObject laserInteractObject;

    [Header("3D Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float soundRadius = 1.5f;
    [SerializeField] private float maxVolume = 0.05f;
    private float initScale = 0.01f;
    private float minimumDistance = 0f;
    private int minimumPos = 0;
    private RaycastHit[] hit;
    private RaycastHit[] soundHit;

    private void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        changingScaleObject.transform.localScale = new Vector3(1f, 1f, initScale);
        LaserDirection = startPoint.transform.forward * -1f;
    }

    private void Update()
    {
        ProcessAbility();
    }

    private void ProcessAbility()
    {
        if (!AuthorizedIncreaseScale())
        {
            return;
        }
        UpdateLaserScale();
    }

    #region Laser interaction
    private bool AuthorizedIncreaseScale()
    {
        if (changingScaleObject == null || startPoint == null || LaserDirection == null)
        {
            return false;
        }

        if (StartLaser.reflectObject == null && StartLaser != null && IsReflectLaser)
        {
            reflectObject = null;
            return false;
        }

        return true;
    }

    private bool IsInfiniteLaser()
    {
        if (hit.Length < 1) //무한히 뻗어나갈때
        {
            changingScaleObject.transform.localScale = new Vector3(1f, 1f, maximumLaycastDistance * 0.5f);
            reflectObject = null;
            laserInteractObject = null;
            HitPointSFX.SetActive(false);
            return false;
        }

        return true;
    }

    private void UpdateLaserScale()
    {
        if (!IsReflectLaser) //반사 레이저가 아닌 경우
        {
            LaserDirection = startPoint.transform.forward * -1f;
        }
        hit = Physics.RaycastAll(startPoint.transform.position, LaserDirection, maximumLaycastDistance);
        if (!IsInfiniteLaser()) //무한히 뻗어나갈때
            return;

        ChangeLaserScale();
    }

    private void ChangeLaserScale()
    {
        minimumDistance = hit[0].distance;
        minimumPos = 0;

        for (int i = 1; i < hit.Length; i++)
        {
            // 최솟값 => min보다 numbers가 작으면 numbers가 최솟값이므로 min값으로 변경.
            if (minimumDistance > hit[i].distance)
            {
                minimumDistance = hit[i].distance;
                minimumPos = i;
            }
        }
        changingScaleObject.transform.localScale = new Vector3(1f, 1f, minimumDistance * 0.5f);

        //LaserSoundHit(minimumPos);
        LaserInteract(minimumPos);

        HitPointSFX.transform.SetPositionAndRotation(hit[minimumPos].point, Quaternion.LookRotation(LaserDirection));
    }
    private void LaserSoundHit(int Pos)
    {
        soundHit = Physics.SphereCastAll(startPoint.transform.position, soundRadius, LaserDirection, minimumDistance);
        for (int i = 1; i < soundHit.Length; i++)
        {
            if (soundHit[i].collider.gameObject.CompareTag("Player"))
            {
                audioSource.volume = maxVolume;
                if (audioSource.isPlaying)
                    return;

                audioSource.Play();
                return;
            }
        }

        audioSource.volume -= 0.002f;
        if (audioSource.volume <= 0)
        {
            audioSource.Stop();
        }
    }
    private void LaserInteract(int Pos)
    {
        if (hit[Pos].collider.gameObject.GetComponent<LaserInteractObject>() != null)
        {
            if (laserInteractObject == null || laserInteractObject != hit[Pos].collider.gameObject.GetComponent<LaserInteractObject>())
            {
                laserInteractObject = hit[Pos].collider.gameObject.GetComponent<LaserInteractObject>();
            }

            if (ParentReflectObject != null)
            {
                laserInteractObject.InteractLaser = ParentReflectObject.ReflectLaser;
            }

            if (!IsReflectLaser)
            {
                laserInteractObject.InteractLaser = changingScaleObject;
            }

            laserInteractObject.InteractOn();
            reflectObject = null;
            HitPointSFX.SetActive(true);
        }
        else
        {
            if (laserInteractObject != null)
            {
                laserInteractObject.InteractOff();
                laserInteractObject = null;
            }

            LaserReflection(Pos);
        }
    }
    #endregion

    #region Laser Reflection
    private void LaserReflection(int Pos)
    {
        if (hit[Pos].collider.gameObject.GetComponent<ReflectReceiveObject>() != null) //반사 시작
        {
            foreach (ReflectObject r in hit[Pos].collider.gameObject.GetComponent<ReflectReceiveObject>().reflectObjects)
            {
                if (r == ParentReflectObject && IsReflectLaser)
                {
                    reflectObject = null;
                    return;
                }
            }

            TurnOnLaserReflection(Pos);
            HitPointSFX.SetActive(false);
        }
        else
        {
            reflectObject = null;
            HitPointSFX.SetActive(true);
        }
    }

    private void TurnOnLaserReflection(int Pos)
    {
        ReflectReceiveObject reflectReceiveObject = hit[Pos].collider.gameObject.GetComponent<ReflectReceiveObject>();
        int count = 0;
        foreach (ReflectObject r in reflectReceiveObject.reflectObjects)
        {
            if (r == reflectObject)
                break;

            if (!r.isReflect)
            {
                reflectObject = r;
                break;
            }
            count++;
        }

        if (reflectReceiveObject.reflectObjects.Count == count)
            return;

        reflectObject.LaserDirection_Receive = hit[Pos].point - startPoint.transform.position;
        reflectObject.HitPoint.transform.position = hit[Pos].point;
        reflectObject.LaserReceiveObject = this;
        reflectObject.reflectLaserScript.StartLaser = StartLaser;

        if (!reflectObject.isReflect)
        {
            reflectReceiveObject.FindStartReflectObject(reflectObject);
        }
    }

    #endregion
}

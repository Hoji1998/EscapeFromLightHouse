using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private RocketLauncherController weapon;
    [Header("Bullet Component")]
    [SerializeField] private float BulletForce = 10f;
    [SerializeField] private float recallTime = 1.5f;
    [SerializeField] private Transform bulletLinePos;
    [SerializeField] private GameObject BulletPos;
    [Header("Bullet Line")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject hitPointVFX;
    [Header("WeaponFire Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip recallSound;
    [SerializeField] private float soundScale = 0.1f;

    private Water freezeWater;
    private Vector3 hitPos;
    private float currentRecallTime = 0f;

    private void Start()
    {
        water = new List<Water>();
    }

    public void Shoot()
    {
        if (weapon.reloading)
            return;

        if (currentWater != null)
        {
            currentWater.FreezingWater();
            freezeWater = currentWater;
        }

        ResetRecallTime();
        weapon.Reload();
        GUIManager.Instance.OnRecallingImage();
        DrawBulletLine();

        audioSource.PlayOneShot(fireSound, soundScale);
    }
    private void DrawBulletLine()
    {
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.SetPosition(0, bulletLinePos.position);
        lineRenderer.SetPosition(1, hitPos);
        hitPointVFX.transform.position = hitPos;

        StartCoroutine(ReduceTheLineWidth());
    }
    private IEnumerator ReduceTheLineWidth()
    {
        float reduceSpeed = 0.3f / 60f;
        lineRenderer.startWidth = 0.05f;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            lineRenderer.startWidth -= reduceSpeed;
            lineRenderer.SetPosition(0, bulletLinePos.position);
            if (lineRenderer.startWidth <= 0)
            {
                break;
            }
        }
        lineRenderer.gameObject.SetActive(false);
        StopCoroutine(ReduceTheLineWidth());
    }
    private void RecallBullet()
    {
        if (currentRecallTime < recallTime)
        {
            currentRecallTime += Time.fixedDeltaTime;
            GUIManager.Instance.defaultRecallingImage.fillAmount += (Time.fixedDeltaTime / recallTime);
            GUIManager.Instance.recallingImage.fillAmount -= (Time.fixedDeltaTime / recallTime);
        }
        else if (!weapon.recalling)
        {
            GUIManager.Instance.OffRecallingImage();
            weapon.recalling = true;
            audioSource.PlayOneShot(recallSound, soundScale);
            weapon.WaitToClose();
            CancleBullet();
        }
    }
    public void CancleBullet()
    {
        if (freezeWater != null)
        {
            freezeWater.ReturnWater();
            GUIManager.Instance.checkGetIceBlockImage.gameObject.SetActive(false);
            freezeWater = null;
        }
        lineRenderer.gameObject.SetActive(false);
    }

    public void ResetRecallTime()
    {
        currentRecallTime = 0f;
    }

    private void Update()
    {
        CheckWaterGuide();

        if (!weapon.reloading)
            return;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            RecallBullet();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ResetRecallTime();
            if (!weapon.recalling)
            {
                GUIManager.Instance.defaultRecallingImage.fillAmount = 0f;
                GUIManager.Instance.recallingImage.fillAmount = 1f;
            }
        }
    }

    #region WaterGuide
    RaycastHit[] hit;
    List<Water> water;
    Water currentWater;
    float minimumDistance;
    int minimumPos;
    private void CheckWaterGuide()
    {
        hit = Physics.RaycastAll(BulletPos.transform.position, BulletPos.transform.forward, 50f);
        if (hit.Length < 1)
            return;

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

        if (water.Count > 0)
        {
            foreach (Water w in water)
            {
                w.GuideObject.SetActive(false);
            }
            water.Clear();
            currentWater = null;
        }

        if (hit[minimumPos].collider.gameObject.GetComponent<Water>() != null)
        {
            currentWater = hit[minimumPos].collider.gameObject.GetComponent<Water>();
            if (!currentWater.IceBlock.activeSelf)
            {
                water.Add(currentWater);
                currentWater.GuideObject.SetActive(true);
            }
        }

        hitPos = hit[minimumPos].point;
    }
    #endregion
}

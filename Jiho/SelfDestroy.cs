using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [Header("SelfDestroyComponent")]
    [SerializeField] private float destroyTime = 8f;
    public bool customDestroy = false;

    private void OnEnable()
    {
        if (!customDestroy)
        {
            SelfDestroyOn();
        }     
    }
    public void SelfDestroyOn()
    {
        StartCoroutine(DestroySequance());
    }
    private IEnumerator DestroySequance()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}

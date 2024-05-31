using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectReceiveObject : MonoBehaviour
{
    [Header("ReflectObjects")]
    public GameObject reflectModule;

    [Header("Laser Pool")]
    [SerializeField] private int poolSize = 10;

    [HideInInspector] public List<ReflectObject> reflectObjects;

    private GameObject[] reflectModules;
    private void Start()
    {
        reflectObjects = new List<ReflectObject>();

        CreatePool();
    }
    private void CreatePool()
    {
        reflectModules = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            reflectModules[i] = Instantiate(reflectModule);
            reflectModules[i].transform.parent = transform.parent;
            reflectModules[i].transform.localPosition = Vector3.zero;
            reflectModules[i].transform.rotation = Quaternion.identity;
            reflectModules[i].SetActive(false);
            reflectObjects.Add(reflectModules[i].GetComponent<ReflectObject>());
        }

        foreach (ReflectObject r in reflectObjects)
        {
            r.reflectReceiveObject = this;
        }
    }
    private void LateUpdate()
    {
        foreach (ReflectObject r in reflectObjects)
        {
            if (r.gameObject.activeSelf)
            {
                r.ProcessAbility();
            }
        }
    }

    public void FindStopReflectObject(ReflectObject reflectObject)
    {
        foreach (ReflectObject r in reflectObjects)
        {
            if (r == reflectObject)
            {
                r.isReflect = false;
                r.ReflectLaser.SetActive(false);

                foreach (GameObject module in reflectModules)
                {
                    if (r.gameObject == module)
                    {
                        module.SetActive(false);
                        break;
                    }
                }
                return;
            }
        }
        
    }

    public void FindStartReflectObject(ReflectObject reflectObject)
    {
        foreach (GameObject module in reflectModules)
        {
            if (module == reflectObject.gameObject)
            {
                module.SetActive(true);

                foreach (ReflectObject r in reflectObjects)
                {
                    if (r == reflectObject)
                    {
                        r.isReflect = true;
                        r.ReflectLaser.SetActive(true);
                        break;
                    }
                }
                return;
            }
        }
    }
}

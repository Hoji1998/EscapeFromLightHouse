using System.Collections;
using UnityEngine;

public class SpawnObjectEvent : InteractEvent
{
    [Header("Spawn Point")]
    [SerializeField] private GameObject spawnPoint;

    [Header("Creatable Object")]
    [SerializeField] private GameObject creatableObject;
    [SerializeField] private int poolSize = 2;

    [Header("Guide Line")]
    public LineRenderer guideLine;

    private GameObject[] spawnObjects;
    private void Start()
    {
        CreatePool();
    }
    private void Awake()
    {
        base.SetGuideLine(guideLine);
    }
    private void CreatePool()
    {
        spawnObjects = new GameObject[poolSize];

        for (int i = 0; i < spawnObjects.Length; i++)
        {
            spawnObjects[i] = Instantiate(creatableObject, spawnPoint.transform.position, spawnPoint.transform.rotation);
            spawnObjects[i].SetActive(false);
        }
    }

    public override void StartInteractEvent()
    {
        ResetInteractEvent();

        CancelInvoke("SpawnObject");
        Invoke("SpawnObject", 0.2f);
    }
    private void SpawnObject()
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            if (!spawnObjects[i].activeSelf)
            {
                spawnObjects[i].SetActive(true);
                spawnObjects[i].transform.position = spawnPoint.transform.position;
                spawnObjects[i].transform.rotation = Quaternion.identity;
                Rigidbody rigid = spawnObjects[i].GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;
                return;
            }
        }
    }
    public override void ResetInteractEvent()
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            if (spawnObjects[i].activeSelf)
            {
                if (spawnObjects[i].GetComponent<GrabbingObject>() != null)
                {
                    spawnObjects[i].gameObject.SetActive(false);
                    //spawnObjects[i].GetComponent<GrabbingObject>().DissolveEnd();
                }
                else
                {
                    spawnObjects[i].gameObject.SetActive(false);
                }
                spawnObjects[i].transform.position = spawnPoint.transform.position;
                spawnObjects[i].transform.rotation = Quaternion.identity;
                Rigidbody rigid = spawnObjects[i].GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;
                return;
            }
        }

    }
}

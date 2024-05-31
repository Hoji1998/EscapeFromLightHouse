using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [Header("SpawnPoint")] //checkPoint
    public Transform spawnPoint;
    public Transform respawnPoint;
    [Header("Stage Component")]
    public bool IsClear = false;
    public string StageName = "Level";
    public int stageIndex = 0;
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlinkingLamp : MonoBehaviour
{
    [Header("BlinkingLamp Component")]
    [SerializeField] private float duration = 1.5f;

    private Light blinkingLight;
    private void Start()
    {
        blinkingLight = GetComponent<Light>();
        blinkingLight.DOIntensity(0f, duration).SetLoops(-1, LoopType.Yoyo);
    }
}

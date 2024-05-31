using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IE.RichFX;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using HoJin.GameScene;

public class PostProcessingControl : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [Header("Fuzz Value")]
    [SerializeField] private float fuzzTime = 0.5f;
    [SerializeField] private float fuzzValue = 1f;
    [Header("Vignette Value")]
    [SerializeField] private float vignetteValue = 0.5f;
    [Header("CameraShake Value")]
    [SerializeField] private float radialBlurValue = 0.15f;
    [SerializeField] private float chromaticAberrationValue = 0.175f;
    [SerializeField] private float cameraShakeTime = 1f;
    [Header("Gass Value")]
    [SerializeField] private float gassTime = 7f;

    private Underwater underwater;
    private Glitch glitch;
    private GaussianBlur gaussianBlur;
    private ScreenFuzz fuzz;
    private Vignette vignette;
    private RadialBlur radialBlur;
    private ChromaticAberration chromaticAberration;
    private Coroutine coroutine;

    private void Start()
    {
        volume.profile.TryGet(out fuzz);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out gaussianBlur);
        volume.profile.TryGet(out radialBlur);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out glitch);
        volume.profile.TryGet(out underwater);
        coroutine = StartCoroutine(DecompositionScreenEffectStart());
        StopCoroutine(coroutine);
        fuzz.active = false;
        vignette.active = false;
        gaussianBlur.active = false;
    }
    public void GassOn()
    {
        StopCoroutine(coroutine);
        coroutine = StartCoroutine(GassEffectStart());
    }
    public void CameraShakeOn()
    {
        StopCoroutine(coroutine);
        coroutine = StartCoroutine(CameraShakeEffectStart());
    }
    public void DecompositionScreenEffect()
    {
        StopCoroutine(coroutine);
        coroutine = StartCoroutine(DecompositionScreenEffectStart());
    }

    public void GaussianBlurOn()
    {
        if (gaussianBlur.active)
            gaussianBlur.active = false;
        else
            gaussianBlur.active = true;
    }

    private void ScreenVignetteOn()
    {
        vignette.active = true;
        vignette.intensity.value = vignetteValue;
    }
    private void ScreenFuzzOn()
    {
        fuzz.active = true;
        fuzz.intensity.value = fuzzValue;
    }
    private IEnumerator GassEffectStart()
    {
        float curTime = 0f;

        vignette.intensity.value = 0.3f;
        vignette.active = true;
        
        gaussianBlur.active = true;
        underwater.active = true;
        glitch.active = true;
        glitch.drift.value = 0f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            curTime += Time.fixedDeltaTime;
            glitch.drift.value += Time.fixedDeltaTime / gassTime;
            vignette.intensity.value += Time.fixedDeltaTime / gassTime;

            if (curTime >= gassTime)
            {
                gaussianBlur.active = false;
                underwater.active = false;
                glitch.active = false;
                vignette.active = false;
                StopCoroutine(coroutine);
            }
        }
    }
    private IEnumerator CameraShakeEffectStart()
    {
        float curTime = 0f;

        radialBlur.active = true;
        chromaticAberration.active = true;
        radialBlur.intensity.value = radialBlurValue;
        chromaticAberration.intensity.value = chromaticAberrationValue;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            curTime += Time.fixedDeltaTime;
            chromaticAberration.intensity.value -= Time.fixedDeltaTime;
            radialBlur.intensity.value -= Time.fixedDeltaTime;

            if (curTime >= cameraShakeTime)
            {
                radialBlur.active = false;
                chromaticAberration.active = false;
                StopCoroutine(coroutine);
            }
        }
    }
    private IEnumerator DecompositionScreenEffectStart()
    {
        float curTime = 0f;
        ScreenFuzzOn();
        ScreenVignetteOn();

        while (true)
        {
            yield return new WaitForFixedUpdate();
            curTime += Time.fixedDeltaTime;
            fuzz.intensity.value -= Time.fixedDeltaTime * 2f;
            vignette.intensity.value -= Time.fixedDeltaTime * 2f;

            if (curTime >= fuzzTime)
            {
                fuzz.active = false;
                StopCoroutine(coroutine);
            }
        }
    }
}

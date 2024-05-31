using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] private AudioMixer backgroundAudioMixer;
    [SerializeField] private AudioMixer SfxAudioMixer;

    [Header("Silders")]
    [SerializeField] private Slider backgroundSlider;
    [SerializeField] private Slider SfxAudioSlider;
    private void Start()
    {
        Initialized();
    }
    public void Initialized()
    {
        backgroundSlider.value = SceneLoadManager.Instance.backgroundVolume;
        SfxAudioSlider.value = SceneLoadManager.Instance.sfxVolume;
        ChangeVolume_BackGround();
        ChangeVolume_Sfx();
    }
    public void ChangeVolume_BackGround()
    {
        backgroundAudioMixer.SetFloat("Master", Mathf.Log10(backgroundSlider.value) * 20);
        SceneLoadManager.Instance.backgroundVolume = backgroundSlider.value;
        DataManager.Instance.data.backgroundVolume = backgroundSlider.value;
    }

    public void ChangeVolume_Sfx()
    {
        SfxAudioMixer.SetFloat("Master", Mathf.Log10(SfxAudioSlider.value) * 20);
        SceneLoadManager.Instance.sfxVolume = SfxAudioSlider.value;
        DataManager.Instance.data.sfxVolume = SfxAudioSlider.value;
    }
}

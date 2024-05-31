using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayControl : MonoBehaviour
{
    [Header("DisplayControl Component")]
    [SerializeField] private List<Vector2> resolutions;
    [SerializeField] private Text displayText;
    [SerializeField] private Toggle onToggle;
    [SerializeField] private Toggle offToggle;

    [Header("TextLanguage")]
    [SerializeField] private Text languageText;
    [SerializeField] public List<LocalizationText> localizationTexts;
    [HideInInspector] public int displayIndex = 0;
    [HideInInspector] public int languageIndex = 0;

    private int width = 0;
    private int height = 0;
    private void Start()
    {
        //SetResolution(1); // 초기에 게임 해상도 고정
        Initialized();
    }
    public void Initialized()
    {
        displayIndex = SceneLoadManager.Instance.resolutionIndex;
        SetResolution();

        switch (SceneLoadManager.Instance.currentLanguage)
        {
            case SceneLoadManager.TextLanguage.Korean:
                languageIndex = 0;
                languageText.text = "한국어";
                break;
            case SceneLoadManager.TextLanguage.English:
                languageIndex = 1;
                languageText.text = "English";
                break;
            case SceneLoadManager.TextLanguage.Chinese_G:
                languageIndex = 2;
                languageText.text = "中文(简体)";
                break;
            case SceneLoadManager.TextLanguage.Chinese_B:
                languageIndex = 3;
                languageText.text = "文字(繁体)";
                break;
            case SceneLoadManager.TextLanguage.Japanese:
                languageIndex = 4;
                languageText.text = "日本語";
                break;
        }

        switch (SceneLoadManager.Instance.screenMode)
        {
            case FullScreenMode.FullScreenWindow:
                onToggle.isOn = true;
                offToggle.isOn = false;
                break;
            case FullScreenMode.Windowed:
                onToggle.isOn = false;
                offToggle.isOn = true;
                break;
        }
        foreach (LocalizationText text in localizationTexts)
        {
            text.LanguageUpdate();
        }
    }
    public void ChangeResolutionIndex(bool value)
    {
        switch (value)
        {
            case false:
                displayIndex = displayIndex - 1 < 0 ? resolutions.Count - 1 : displayIndex - 1;
                break;
            case true:
                displayIndex = displayIndex + 1 > resolutions.Count - 1 ? 0 : displayIndex + 1;
                break;
        }
        SceneLoadManager.Instance.resolutionIndex = displayIndex;
        DataManager.Instance.data.resolutionIndex = displayIndex;
        SetResolution();
    }
    public void SetResolution()
    {
        ChangeResolution();

        int setWidth = width; // 사용자 설정 너비
        int setHeight = height; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장
        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), SceneLoadManager.Instance.screenMode); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }

    }
    public void SetFullScreen(bool value)
    {
        switch (value)
        {
            case false:
                onToggle.isOn = !offToggle.isOn;
                break;
            case true:
                offToggle.isOn = !onToggle.isOn;
                break;
        }
       SceneLoadManager.Instance.screenMode = onToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreenMode = SceneLoadManager.Instance.screenMode;
    }
    public void ChangeLanguge(bool value)
    {
        switch (value)
        {
            case false:
                languageIndex = languageIndex - 1 < 0 ? 4/*max language index*/ : languageIndex - 1;
                break;
            case true:
                languageIndex = languageIndex + 1 > 4/*max language index*/  ? 0 : languageIndex + 1;
                break;
        }
        SetLanguage();
    }
    public void SetLanguage()
    {
        switch (languageIndex)
        {
            case 0:
                SceneLoadManager.Instance.currentLanguage = SceneLoadManager.TextLanguage.Korean;
                languageText.text = "한국어";
                break;
            case 1:
                SceneLoadManager.Instance.currentLanguage = SceneLoadManager.TextLanguage.English;
                languageText.text = "English";
                break;
            case 2:
                SceneLoadManager.Instance.currentLanguage = SceneLoadManager.TextLanguage.Chinese_G;
                languageText.text = "中文(简体)";
                break;
            case 3:
                SceneLoadManager.Instance.currentLanguage = SceneLoadManager.TextLanguage.Chinese_B;
                languageText.text = "文字(繁体)";
                break;
            case 4:
                SceneLoadManager.Instance.currentLanguage = SceneLoadManager.TextLanguage.Japanese;
                languageText.text = "日本語";
                break;

        }
        DataManager.Instance.data.currentLanguage = SceneLoadManager.Instance.currentLanguage;

        foreach (LocalizationText text in localizationTexts)
        {
            text.LanguageUpdate();
        }
    }
    private void ChangeResolution()
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (i == displayIndex)
            {
                width = (int)resolutions[i].x;
                height = (int)resolutions[i].y;
            }
        }
        displayText.text = width + "*" + height;
    }
}

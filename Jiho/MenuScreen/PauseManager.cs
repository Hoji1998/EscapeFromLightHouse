using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Component")]
    public bool isPause = false;
    public GameObject tutorialImage_kor;
    public GameObject tutorialImage_eng;
    public GameObject tutorialImage_Chi_g;
    public GameObject tutorialImage_Chi_b;
    public GameObject tutorialImage_jp;

    private PostProcessingControl postProcessingControl;
    public void Initialized()
    {
        InitializeTutorialImage();

        switch (SceneLoadManager.Instance.currentLanguage)
        {
            case SceneLoadManager.TextLanguage.Korean:
                tutorialImage_kor.SetActive(true);
                break;
            case SceneLoadManager.TextLanguage.English:
                tutorialImage_eng.SetActive(true);
                break;
            case SceneLoadManager.TextLanguage.Chinese_G:
                tutorialImage_Chi_g.SetActive(true);
                break;
            case SceneLoadManager.TextLanguage.Chinese_B:
                tutorialImage_Chi_b.SetActive(true);
                break;
            case SceneLoadManager.TextLanguage.Japanese:
                tutorialImage_jp.SetActive(true);
                break;
        }
    }
    private void InitializeTutorialImage()
    {
        tutorialImage_kor.SetActive(false);
        tutorialImage_eng.SetActive(false);
        tutorialImage_Chi_g.SetActive(false);
        tutorialImage_Chi_b.SetActive(false);
        tutorialImage_jp.SetActive(false);
    }
    public void SetPause()
    {
        if (postProcessingControl == null)
            postProcessingControl = Camera.main.GetComponent<PostProcessingControl>();

        postProcessingControl.GaussianBlurOn();

        if (isPause)
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            isPause = false;
            GUIManager.Instance.audioSource.UnPause();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GUIManager.Instance.HUD.alpha = 1f;
        }
        else
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f;
            isPause = true;
            GUIManager.Instance.audioSource.Pause();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GUIManager.Instance.HUD.alpha = 0f;
            Initialized();
        }
    }
}

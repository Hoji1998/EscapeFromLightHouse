using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartScreen : MonoBehaviour
{
    [Header("StartScene")]
    [SerializeField] private CanvasGroup startScreenPopup;
    [Header("Fader")]
    [SerializeField] private CanvasGroup FadeImage;
    [SerializeField] private float FadeSpeed = 1f;
    [Header("SelectChapter")]
    [SerializeField] private CanvasGroup selectChapterPopup;
    [Header("SelectStage")]
    [SerializeField] private CanvasGroup selectStagePopup;
    [SerializeField] private List<GameObject> selectStagePopupGroup;
    [Header("Option")]
    [SerializeField] private CanvasGroup optionPopup;
    [SerializeField] private DisplayControl displayControl;
    [SerializeField] private VolumeControl volumeControl;
    [Header("UnlockObjects")]
    [SerializeField] private List<UnlockChapter> unlockChapters;

    private Coroutine coroutine;
    private bool IsStartGame = false;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FadeOutScreen();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectStagePopup.gameObject.activeSelf)
            {
                SelectStagePopupOff();
                return;
            }

            if (selectChapterPopup.gameObject.activeSelf)
            {
                SelectChapterPopupOff();
                return;
            }

            if (optionPopup.gameObject.activeSelf)
            {
                OptionPopupOff();
                return;
            }
        }
    }
    public void StartGame(string currentLoadingSceneName)
    {
        SceneLoadManager.Instance.currentLoadingName = currentLoadingSceneName;
        IsStartGame = true;
        FadeInScreen();
    }
    public void SelectChapterPopupOn()
    {
        selectChapterPopup.gameObject.SetActive(true);
        selectChapterPopup.alpha = 0f;
        selectChapterPopup.DOFade(1f, 0.5f);

        startScreenPopup.gameObject.SetActive(false);
        startScreenPopup.alpha = 1f;
        startScreenPopup.DOFade(0f, 0.5f);

        foreach (UnlockChapter unlockChapter in unlockChapters)
        {
            unlockChapter.UnLockCheck();
        }
    }
    public void SelectChapterPopupOff()
    {
        selectChapterPopup.gameObject.SetActive(false);
        selectChapterPopup.alpha = 1f;
        selectChapterPopup.DOFade(0f, 0.5f);

        startScreenPopup.gameObject.SetActive(true);
        startScreenPopup.alpha = 0f;
        startScreenPopup.DOFade(1f, 0.5f);
    }
    public void SelectStagePopupOn(int value)
    {
        selectStagePopup.gameObject.SetActive(true);
        selectStagePopup.alpha = 0f;
        selectStagePopup.DOFade(1f, 0.5f);

        selectChapterPopup.gameObject.SetActive(false);
        selectChapterPopup.alpha = 1f;
        selectChapterPopup.DOFade(0f, 0.5f);

        selectStagePopupGroup[value].SetActive(true);

        foreach (UnlockChapter unlockChapter in unlockChapters)
        {
            unlockChapter.UnLockCheck();
        }
    }
    public void SelectStagePopupOff()
    {
        selectStagePopup.gameObject.SetActive(false);
        selectStagePopup.alpha = 1f;
        selectStagePopup.DOFade(0f, 0.5f);

        selectChapterPopup.gameObject.SetActive(true);
        selectChapterPopup.alpha = 0f;
        selectChapterPopup.DOFade(1f, 0.5f);

        foreach (GameObject popup in selectStagePopupGroup)
        {
            popup.SetActive(false);
        }
    }
    public void OptionPopupOn()
    {
        optionPopup.gameObject.SetActive(true);
        optionPopup.alpha = 0f;
        optionPopup.DOFade(1f, 0.5f);
        displayControl.Initialized();
        volumeControl.Initialized();
    }
    public void OptionPopupOff()
    {
        optionPopup.gameObject.SetActive(false);
        optionPopup.alpha = 1f;
        optionPopup.DOFade(0f, 0.5f);
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }
    public void FadeInScreen()
    {
        coroutine = StartCoroutine(FadeIn());
    }

    
    public void FadeOutScreen()
    {
        coroutine = StartCoroutine(FadeOut());
    }
    private IEnumerator FadeIn()
    {
        FadeImage.alpha = 0f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            FadeImage.alpha += Time.deltaTime * FadeSpeed;
            if (FadeImage.alpha >= 1f)
            {
                break;
            }
        }
        if (IsStartGame)
        {
            SceneLoadManager.Instance.LoadLevel();
        }
        StopCoroutine(coroutine);
    }
    private IEnumerator FadeOut()
    {
        FadeImage.alpha = 1f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            FadeImage.alpha -= Time.deltaTime * FadeSpeed;
            if (FadeImage.alpha <= 0f)
            {
                break;
            }
        }
        StopCoroutine(coroutine);
    }
}

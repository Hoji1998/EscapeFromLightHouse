using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static string nextScene = "";
    [SerializeField] private float spareTime = 2f;
    [SerializeField] private GameObject tutorialImage_kor;
    [SerializeField] private GameObject tutorialImage_eng;
    [SerializeField] private GameObject tutorialImage_Chi_g;
    [SerializeField] private GameObject tutorialImage_Chi_b;
    [SerializeField] private GameObject tutorialImage_jp;

    private void Start()
    {
        nextScene = SceneLoadManager.Instance.currentLoadingName;

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
        StartCoroutine(LoadScene());
    }
    private void InitializeTutorialImage()
    {
        tutorialImage_kor.SetActive(false);
        tutorialImage_eng.SetActive(false);
        tutorialImage_Chi_g.SetActive(false);
        tutorialImage_Chi_b.SetActive(false);
        tutorialImage_jp.SetActive(false);
    }
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress >= 0.9f) 
            {
                timer += Time.deltaTime;
            } 
            if (timer >= spareTime)
            {
                op.allowSceneActivation = true; yield break;
            }
        }
    }
}

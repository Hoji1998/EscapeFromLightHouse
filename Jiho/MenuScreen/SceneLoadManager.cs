using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoadManager : MonoBehaviour
{
    // �ν��Ͻ��� �����ϱ� ���� ������Ƽ
    private static SceneLoadManager instance;

    [Header("Default Setting")]
    [SerializeField] public int resolutionIndex = 3;
    [SerializeField] [Range(0, 1f)] public float sfxVolume = 1f;
    [SerializeField] [Range(0, 1f)] public float backgroundVolume = 1f;
    [SerializeField] public FullScreenMode screenMode = FullScreenMode.FullScreenWindow;

    [HideInInspector] public string currentLoadingName = "";
    public TextLanguage currentLanguage = TextLanguage.Korean;
    public enum TextLanguage { Korean, English, Chinese_B, Chinese_G, Japanese }
    public static SceneLoadManager Instance
    {
        get
        {
            // �ν��Ͻ��� ���� ��쿡 �����Ϸ� �ϸ� �ν��Ͻ��� �Ҵ����ش�.
            if (!instance)
            {
                instance = FindObjectOfType(typeof(SceneLoadManager)) as SceneLoadManager;

                if (instance == null)
                    Debug.Log("no Singleton obj");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // �Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ�Ǵ��� ����Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        DataManager.Instance.LoadGameData();
        currentLanguage = DataManager.Instance.data.currentLanguage;
        backgroundVolume = DataManager.Instance.data.backgroundVolume;
        sfxVolume = DataManager.Instance.data.sfxVolume;
        resolutionIndex = DataManager.Instance.data.resolutionIndex;
    }
    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveGameData();
    }
    public void ChapterUnlock(int chapterNum)
    {
        DataManager.Instance.data.isUnlock[chapterNum] = true;
        DataManager.Instance.SaveGameData();
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene("LoadingScreen");
    }
}

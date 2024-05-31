using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoadManager : MonoBehaviour
{
    // 인스턴스에 접근하기 위한 프로퍼티
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
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
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
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
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

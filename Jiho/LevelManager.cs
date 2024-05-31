using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoJin.GameScene;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("StartScreen")]
    [SerializeField] private string startScreenName = "StartScreen";
    [Header("Stages")]
    public Stage startStage;
    [SerializeField] private string nextStageName;
    [HideInInspector] public GameObject player;
    [HideInInspector] public Stage currentStage;
    [HideInInspector] public EventBox eventBox;
    public static LevelManager Instance { get; set; }

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        InitializedLevel();
    }
    private void InitializedLevel()
    {
        currentStage = startStage;
        gameManager = GameManager.Instance;
        player = gameManager.PlayerMoving.gameObject;

        gameManager.PlayerMoving.isPause = true;
        player.transform.position = currentStage.spawnPoint.position;
        Invoke("SetCompletePosition", 0.5f);
    }
    public void RespawnPlayer()
    {
        gameManager.PlayerMoving.isPause = true;
        player.transform.position = currentStage.respawnPoint.position;
        Invoke("SetCompletePosition", 0.5f);
    }
    public void ReStart()
    {
        GUIManager.Instance.ScreenFadeIn(currentStage.StageName);
        GUIManager.Instance.pauseManager.SetPause();
    }
    public void ExitToMainMenu()
    {
        GUIManager.Instance.pauseManager.SetPause();
        SceneManager.LoadScene(startScreenName);   
    }
    public void ExitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                    Application.Quit(); // 어플리케이션 종료
    #endif
    }
    public void StageClear()
    {
        currentStage.IsClear = true;
        DataManager.Instance.data.isUnlock[currentStage.stageIndex + 2] = true;

        switch (currentStage.stageIndex)
        {
            case 12:
                DataManager.Instance.data.isUnlock[1] = true;
                break;
            case 16:
                DataManager.Instance.data.isUnlock[2] = true;
                break;
            case 20:
                DataManager.Instance.data.isUnlock[3] = true;
                break;
            case 24:
                DataManager.Instance.data.isUnlock[4] = true;
                break;
            case 28:
                DataManager.Instance.data.isUnlock[30] = true;
                break;
        }

        DataManager.Instance.SaveGameData();

        GUIManager.Instance.ScreenFadeIn(nextStageName);
    }
    public void LoadNextStage(string loadNextStage)
    {
        SceneManager.LoadScene(loadNextStage);
    }
    private void SetCompletePosition()
    {
        gameManager.PlayerMoving.isPause = false;
    }

    #region GameEvent
    GUIManager guiManager;
    public void StartSubtitlesEvent()
    {
        guiManager = GUIManager.Instance;
        StartCoroutine(SubtitlesLoad());
    }

    private IEnumerator SubtitlesLoad()
    {
        guiManager.subtitlesText.text = " ";
        int index = 0;
        while (index < guiManager.textData.SubtitlesText.Length)
        {
            if (index == 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                guiManager.audioSource.Stop();
                yield return new WaitForSeconds(1f);
            }

            guiManager.subtitlesGroup.alpha = 1f;
            guiManager.audioSource.Play();

            for (int i = guiManager.textData.SubtitlesText[index].IndexOf(":") + 1; i <= guiManager.textData.SubtitlesText[index].Length; i++)
            {
                guiManager.subtitlesText.text = guiManager.textData.SubtitlesText[index].Substring(0, i);
                
                yield return new WaitForSeconds(0.05f);
            }
            index++;
        }

        guiManager.audioSource.Stop();

        switch (guiManager.textData.ChapterValue)
        {
            case 7:
                eventBox.ElevatorFallSequanceStart();
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(2f);

        guiManager.subtitlesGroup.alpha = 0f;

        StopCoroutine(SubtitlesLoad());
    }
    #endregion
}

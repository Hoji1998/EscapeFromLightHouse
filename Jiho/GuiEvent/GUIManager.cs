using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoJin.GameScene;
using DG.Tweening;

public class GUIManager : MonoBehaviour
{
    [Header("HUD")]
    public CanvasGroup HUD;
    public Image defaultRecallingImage;
    public Image recallingImage;
    public Image checkGetIceBlockImage;
    public Text stageText;
    [Header("PauseCanvas")]
    public PauseManager pauseManager;
    public DisplayControl displayControl;
    public VolumeControl volumeControl;
    [Header("FaliedCanvas")]
    public CanvasGroup failedCanvas;
    [Header("ScreenFade Canvas")]
    [SerializeField] private CanvasGroup screenFadeCanvas;
    [SerializeField] private float fadeDuration = 2f;
    [Header("Cross Hair")]
    public CanvasGroup crossHair;
    [Header("Guide Text")]
    [SerializeField] private CanvasGroup rotateButtonGuideTextGroup;
    [SerializeField] private CanvasGroup interactButtonGuideTextGroup;
    [SerializeField] private Image cursor;
    [Header("Subtitles")]
    public CanvasGroup subtitlesGroup;
    public TextData textData;
    public Text subtitlesText;
    public static GUIManager Instance { get; set; }
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public float zoomSpeed;
    [HideInInspector] public float cameraDefaultSize;
    [HideInInspector] public float cameraMinimumSize;
    [HideInInspector] public bool IsZoomming = false;
    private Coroutine coroutine;
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
        InitializedGuiManager();
    }
    private void InitializedGuiManager()
    {
        zoomSpeed = 5f;
        cameraDefaultSize = 60f;
        cameraMinimumSize = 20f;
        subtitlesText.text = textData.SubtitlesText[0];
        subtitlesGroup.alpha = 0;
        ScreenFadeOut();
        volumeControl.Initialized();
        displayControl.Initialized();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnRecallingImage()
    {
        recallingImage.fillAmount = 1f;
        defaultRecallingImage.fillAmount = 0f;
    }

    public void OffRecallingImage()
    {
        recallingImage.fillAmount = 0f;
        defaultRecallingImage.fillAmount = 1f;
    }
    public void OnRotateButtonGuideText()
    {
        if (rotateButtonGuideTextGroup.alpha == 1f)
            return;

        if (interactButtonGuideTextGroup.alpha == 1f)
            return;

        rotateButtonGuideTextGroup.alpha = 1f;
        cursor.color = Color.cyan;
    }
    public void OffRotateButtonGuideText()
    {
        if (rotateButtonGuideTextGroup.alpha == 0f)
            return;

        rotateButtonGuideTextGroup.alpha = 0f;
        cursor.color = Color.white;
    }
    public void OnInteractButtonGuideText()
    {
        if (interactButtonGuideTextGroup.alpha == 1f)
            return;

        if (rotateButtonGuideTextGroup.alpha == 1f)
            return;

        interactButtonGuideTextGroup.alpha = 1f;
        cursor.color = Color.cyan;
    }
    public void OffInteractButtonGuideText()
    {
        if (interactButtonGuideTextGroup.alpha == 0f)
            return;
        if (gameManager == null)
            gameManager = GameManager.Instance;
        if (gameManager.PlayerInteract.GrabbingObject != null)
            return;

        interactButtonGuideTextGroup.alpha = 0f;
        cursor.color = Color.white;
    }
    public void OffAllGuideText()
    {
        OffInteractButtonGuideText();
        OffRotateButtonGuideText();
    }
    public void ZoomCrossHair()
    {
        if (IsZoomming)
            return;

        if (crossHair.gameObject.activeSelf)
        {
            coroutine = StartCoroutine(ZoomOut());
        }
        else 
        {
            coroutine = StartCoroutine(ZoomIn());
        }
    }
    public void ScreenFadeIn(string loadLevelName)
    {
        StartCoroutine(FadeIn(loadLevelName));
    }
    public void ScreenFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn(string loadLevelName)
    {
        screenFadeCanvas.alpha = 0f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (screenFadeCanvas.alpha < 1f)
            {
                screenFadeCanvas.alpha += 1f / (60f * fadeDuration);
            }
            else
            {
                screenFadeCanvas.alpha = 1f;
                break;
            }
        }
        LevelManager.Instance.LoadNextStage(loadLevelName);
    }

    private IEnumerator FadeOut()
    {
        screenFadeCanvas.alpha = 1f;
        stageText.color = new Color(1f, 1f, 1f, 0f);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (screenFadeCanvas.alpha > 0f)
            {
                screenFadeCanvas.alpha -= 1f / (60f * fadeDuration);
            }
            else
            {
                screenFadeCanvas.alpha = 0f;
                break;
            }
        }
        stageText.text = LevelManager.Instance.currentStage.StageName;
        stageText.DOFade(1f, 0.5f);
        StopCoroutine(FadeOut());
    }
    private IEnumerator ZoomOut()
    {
        IsZoomming = true;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Camera.main.fieldOfView += zoomSpeed;
            if (Camera.main.fieldOfView >= cameraDefaultSize)
            {
                crossHair.gameObject.SetActive(false);
                Camera.main.fieldOfView = cameraDefaultSize;
                break;
            }
        }
        IsZoomming = false;
        StopCoroutine(coroutine);
    }

    private IEnumerator ZoomIn()
    {
        IsZoomming = true;
        crossHair.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Camera.main.fieldOfView -= zoomSpeed;
            if (Camera.main.fieldOfView <= cameraMinimumSize)
            {
                Camera.main.fieldOfView = cameraMinimumSize;
                break;
            }
        }
        IsZoomming = false;
        StopCoroutine(coroutine);
    }
}

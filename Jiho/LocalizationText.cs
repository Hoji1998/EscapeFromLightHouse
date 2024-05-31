using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    [Header("LocalizationText Component")]
    [SerializeField] private Text text;

    [Header("Language")]
    [SerializeField] private string eng;
    [SerializeField] private string kor;
    [SerializeField] private string chi_g;
    [SerializeField] private string chi_b;
    [SerializeField] private string jp;
    private void OnEnable()
    {
        LanguageUpdate();
    }
    public void LanguageUpdate()
    {
        switch (SceneLoadManager.Instance.currentLanguage)
        {
            case SceneLoadManager.TextLanguage.Korean:
                text.text = kor;
                break;
            case SceneLoadManager.TextLanguage.English:
                text.text = eng;
                break;
            case SceneLoadManager.TextLanguage.Chinese_G:
                text.text = chi_g;
                break;
            case SceneLoadManager.TextLanguage.Chinese_B:
                text.text = chi_b;
                break;
            case SceneLoadManager.TextLanguage.Japanese:
                text.text = jp;
                break;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
public class UnlockChapter : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private int index = 0;
    private void Start()
    {
        lockIcon.SetActive(true);
    }
    public void UnLockCheck()
    {
        switch (DataManager.Instance.data.isUnlock[index])
        {
            case true:
                button.interactable = true;
                lockIcon.SetActive(false);
                break;
            case false:
                button.interactable = false;
                lockIcon.SetActive(true);
                break;
        }

        if (index == 30 && !DataManager.Instance.data.isUnlock[index])
        {
            button.gameObject.SetActive(false);
        }
        else { button.gameObject.SetActive(true); }
    }
    private void Update()
    {
        if (button.interactable)
        {
            lockIcon.SetActive(false);
        }
        else
        {
            lockIcon.SetActive(true);
        }
    }
}

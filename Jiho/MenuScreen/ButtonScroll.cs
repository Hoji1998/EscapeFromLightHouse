using UnityEngine;
using UnityEngine.UI;

public class ButtonScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void ScrollValueChange(float value)
    {
        scrollRect.horizontalScrollbar.value += value;
    }
}

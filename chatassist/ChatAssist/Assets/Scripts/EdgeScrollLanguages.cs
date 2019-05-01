using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Following: https://forum.unity.com/threads/how-to-position-scrollrect-to-another-item.268794/
[RequireComponent(typeof(ScrollRect))]
public class EdgeScrollLanguages : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform rectTransform;
    public RectTransform contentRectTransform;
    RectTransform selectedRectTransform;
    public MagicLeap.ControllerSelection contSelect;
    public LanguageScrollList lsl;
    public float scrollSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScrollToSelected();
    }

    void UpdateScrollToSelected()
    {
        if (lsl.toggles.Count == 0) return;
        int index = (int)contSelect.leftIndex;
        selectedRectTransform = lsl.toggles[index].GetComponent<RectTransform>();
        Vector3 selectedDifference = rectTransform.localPosition - selectedRectTransform.localPosition;
        float contentHeightDifference = (contentRectTransform.rect.height - rectTransform.rect.height);
        float selectedPosition = (contentRectTransform.rect.height - selectedDifference.y);
        float currentScrollRectPosition = scrollRect.normalizedPosition.y * contentHeightDifference;
        float above = currentScrollRectPosition - (selectedRectTransform.rect.height / 2) + rectTransform.rect.height;
        float below = currentScrollRectPosition + (selectedRectTransform.rect.height / 2);
        if (selectedPosition > above)
        {
            float step = selectedPosition - above;
            float newY = currentScrollRectPosition + step;
            float newNormalizedY = newY / contentHeightDifference;
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
        }
        else if (selectedPosition < below)
        {
            float step = selectedPosition - below;
            float newY = currentScrollRectPosition + step;
            float newNormalizedY = newY / contentHeightDifference;
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, new Vector2(0, newNormalizedY), scrollSpeed * Time.deltaTime);
        }
    }
}

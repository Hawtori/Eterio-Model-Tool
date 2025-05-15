using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuExtend : MonoBehaviour
{
    public Button mainButton;
    public List<RectTransform> subButtons;
    public float animDuration = 0.3f;

    public bool isExpanded = false;
    private List<Vector2> originalPositions = new List<Vector2>();
    private RectTransform mainButtonRect;

    private void Start()
    {
        mainButtonRect = mainButton.GetComponent<RectTransform>();
        
        foreach(RectTransform btn in subButtons)
        {
            originalPositions.Add(btn.anchoredPosition);
            btn.anchoredPosition = mainButtonRect.anchoredPosition;
            btn.gameObject.SetActive(false);
        }

        mainButton.onClick.AddListener(ToggleMenu);
    }

    private void ToggleMenu()
    {
        if (!isExpanded)
            StartCoroutine(HideButtons());
        else
            StartCoroutine(ShowButtons());

        isExpanded = !isExpanded;
    }

    private IEnumerator ShowButtons()
    {
        for (int i = 0; i < subButtons.Count; i++)
        {
            RectTransform btn = subButtons[i];
            Vector2 targetPos = originalPositions[i];

            btn.gameObject.SetActive(true);
            StartCoroutine(AnimatePosition(btn, mainButtonRect.anchoredPosition, targetPos));
        }

        yield return null;
    }

    private IEnumerator HideButtons()
    {
        for (int i = 0; i < subButtons.Count; i++)
        {
            RectTransform btn = subButtons[i];
            StartCoroutine(AnimatePosition(btn, btn.anchoredPosition, mainButtonRect.anchoredPosition, () =>
            {
                btn.gameObject.SetActive(false);
            }));
        }

        yield return null;
    }

    private IEnumerator AnimatePosition(RectTransform rect, Vector2 start, Vector2 end, System.Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float easeOutT = Mathf.SmoothStep(0, 1, t);

            rect.anchoredPosition = Vector2.Lerp(start, end, easeOutT);
            yield return null;
        }

        rect.anchoredPosition = end;
        onComplete?.Invoke();
    }

}

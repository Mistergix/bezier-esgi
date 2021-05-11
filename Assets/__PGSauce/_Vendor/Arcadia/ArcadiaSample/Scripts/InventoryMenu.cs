using System.Collections;
using GameTroopers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Arcadia.Sample
{
    public class InventoryMenu : Menu, IOnMenuLoaded
{
    public void OnMenuLoaded()
    {
        closeButton.onClick.AddListener(OnCloseButton);

        ShowAction = CustomShowAction;
        HideAction = CustomHideAction;
    }

    /// <summary>
    /// Custom animations override
    /// </summary>
    private IEnumerator CustomShowAction(Menu menu, Vector2 startPosition, Vector2 endPosition, float duration)
    {
        var rectTransform = menu.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPosition;

        while(duration > 0)
        {
            duration -= Time.deltaTime;
            
            rectTransform.anchoredPosition = Vector3.Lerp(endPosition, startPosition, Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, duration)));
            yield return null;
        }
    }

    private IEnumerator CustomHideAction(Menu menu, Vector2 startPosition, Vector2 endPosition, float duration)
    {
        var rectTransform = menu.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPosition;

        float currentTime = 0f;
        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;

            float percent = Mathf.Clamp01(currentTime / duration);

            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, percent * 2);

            yield return null;
        }
    }

    /// <summary>
    /// Custom start and end positions
    /// </summary>
    protected override Vector2 GetMenuOffScreenPosition()
    {
        var canvasSize = m_menuManager.Canvas.GetComponent<RectTransform>().sizeDelta;
        return new Vector2(0, canvasSize.y);
    }

    protected override Vector2 GetMenuOnScreenPosition()
    {
        var canvasSize = m_menuManager.Canvas.GetComponent<RectTransform>().sizeDelta;
        return new Vector2(0, 0);
    }

    /// <summary>
    /// Events
    /// </summary>
    private void OnCloseButton()
    {
        GameManager.Instance.BackEvent();
    }

    /// <summary>
    /// Back event override
    /// </summary>
    protected internal override void HandleGoBack()
    {
        GameManager.Instance.ShowMainMenu();
    }

    [SerializeField] Button closeButton;
}

}

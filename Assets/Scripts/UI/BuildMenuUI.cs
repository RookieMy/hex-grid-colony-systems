using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMenuUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public RectTransform menuPanel;
    public RectTransform toggleButton;

    [Header("Panel Positions")]
    public float panelHiddenY = -150f;
    public float panelShownY = 0f;

    [Header("Toggle Button")]
    public float buttonRestY = 20f;
    public float buttonHoverOffset = 10f;
    public float buttonShownY = 70f;

    [Header("Animation Settings")]
    public float slideSpeed = 10f;

    private bool isMenuOpen = false;
    private float panelTargetY;

    public float buttonTargetY;


    private void Start()
    {
        panelTargetY = panelHiddenY;
        buttonTargetY = buttonRestY;

        if (menuPanel != null)
        {
            var p = menuPanel.anchoredPosition;
            p.y = panelHiddenY;
            menuPanel.anchoredPosition = p;
        }

        if (toggleButton != null)
        {
            var p = toggleButton.anchoredPosition;
            p.y = buttonRestY;
            toggleButton.anchoredPosition = p;
        }
    }

    private void Update()
    {
        if (menuPanel != null)
        {
            Vector2 pos = menuPanel.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, panelTargetY, Time.deltaTime * slideSpeed);
            menuPanel.anchoredPosition = pos;
        }

        if (toggleButton != null)
        {
            Vector2 pos = toggleButton.anchoredPosition;
            pos.y = Mathf.Lerp(pos.y, buttonTargetY, Time.deltaTime * slideSpeed);
            toggleButton.anchoredPosition = pos;
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        panelTargetY = isMenuOpen ? panelShownY : panelHiddenY;
        buttonTargetY = isMenuOpen ? buttonShownY : buttonRestY;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTargetY = isMenuOpen ? buttonShownY : buttonRestY + buttonHoverOffset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonTargetY = isMenuOpen ? buttonShownY : buttonRestY;
    }
}

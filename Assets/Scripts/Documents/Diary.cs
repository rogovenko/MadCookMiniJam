using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class SpreadData
{
    [Header("Левая страница")]
    [TextArea]
    public string leftPageText;
    public Sprite leftPageBackground;
    
    [Header("Правая страница")]
    [TextArea]
    public string rightPageText;
    public Sprite rightPageBackground;
}

public class Diary : Paper
{
    [Header("Развороты дневника")]
    [Tooltip("Список разворотов (левая и правая страница)")]
    public List<SpreadData> spreads = new List<SpreadData>();

    [Header("UI элементы для листания")]
    [Tooltip("Кнопка/область для листания назад (левый нижний угол)")]
    public Button prevSpreadButton;
    [Tooltip("Кнопка/область для листания вперёд (правый нижний угол)")]
    public Button nextSpreadButton;

    [Header("Левая страница")]
    [Tooltip("Text для отображения текста левой страницы")]
    public TextMeshProUGUI paperTextLeft;
    [Tooltip("Image для отображения фона левой страницы")]
    public Image pageBackgroundLeft;

    [Header("Правая страница")]
    [Tooltip("Text для отображения текста правой страницы")]
    public TextMeshProUGUI paperTextRight;
    [Tooltip("Image для отображения фона правой страницы")]
    public Image pageBackgroundRight;

    private int currentSpread = 0;

    protected override void Start()
    {
        base.Start();
        ShowSpread(0);
        if (prevSpreadButton != null)
            prevSpreadButton.onClick.AddListener(PrevSpread);
        if (nextSpreadButton != null)
            nextSpreadButton.onClick.AddListener(NextSpread);
    }

    public void ShowSpread(int spreadIndex)
    {
        if (spreads == null || spreads.Count == 0) return;
        
        currentSpread = Mathf.Clamp(spreadIndex, 0, spreads.Count - 1);
        
        // Обновляем левую страницу
        if (paperTextLeft != null)
            paperTextLeft.text = spreads[currentSpread].leftPageText;
        if (pageBackgroundLeft != null)
            pageBackgroundLeft.sprite = spreads[currentSpread].leftPageBackground;
        
        // Обновляем правую страницу
        if (paperTextRight != null)
            paperTextRight.text = spreads[currentSpread].rightPageText;
        if (pageBackgroundRight != null)
            pageBackgroundRight.sprite = spreads[currentSpread].rightPageBackground;
        
        UpdateButtons();
    }

    public void NextSpread()
    {
        if (currentSpread < spreads.Count - 1)
            ShowSpread(currentSpread + 1);
    }

    public void PrevSpread()
    {
        if (currentSpread > 0)
            ShowSpread(currentSpread - 1);
    }

    private void UpdateButtons()
    {
        if (prevSpreadButton != null)
            prevSpreadButton.interactable = currentSpread > 0;
        if (nextSpreadButton != null)
            nextSpreadButton.interactable = currentSpread < spreads.Count - 1;
    }
    
    // Получить текущий разворот
    public int GetCurrentSpread()
    {
        return currentSpread;
    }
    
    // Получить общее количество разворотов
    public int GetTotalSpreads()
    {
        return spreads.Count;
    }
    
    // Проверить, можно ли листать вперед
    public bool CanGoNext()
    {
        return currentSpread < spreads.Count - 1;
    }
    
    // Проверить, можно ли листать назад
    public bool CanGoPrev()
    {
        return currentSpread > 0;
    }
} 
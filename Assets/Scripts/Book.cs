using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class PageData
{
    [TextArea]
    public string text;
    public Sprite background;
}

public class Book : Paper
{
    [Header("Страницы книги")]
    [Tooltip("Список страниц (текст и фон)")]
    public List<PageData> pages = new List<PageData>();

    [Header("UI элементы для листания")]
    [Tooltip("Кнопка/область для листания назад (левый нижний угол)")]
    public Button prevPageButton;
    [Tooltip("Кнопка/область для листания вперёд (правый нижний угол)")]
    public Button nextPageButton;

    [Header("Фон страницы")]
    [Tooltip("Image для отображения фона страницы")]
    public Image pageBackground;

    private int currentPage = 0;

    protected override void Start()
    {
        base.Start();
        ShowPage(0);
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(PrevPage);
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);
    }

    public void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Count == 0) return;
        currentPage = Mathf.Clamp(pageIndex, 0, pages.Count - 1);
        if (paperText != null)
            Debug.Log(pages[currentPage].text);
            paperText.text = pages[currentPage].text;
        if (pageBackground != null)
            pageBackground.sprite = pages[currentPage].background;
        UpdateButtons();
    }

    public void NextPage()
    {
        if (currentPage < pages.Count - 1)
            ShowPage(currentPage + 1);
    }

    public void PrevPage()
    {
        if (currentPage > 0)
            ShowPage(currentPage - 1);
    }

    private void UpdateButtons()
    {
        if (prevPageButton != null)
            prevPageButton.interactable = currentPage > 0;
        if (nextPageButton != null)
            nextPageButton.interactable = currentPage < pages.Count - 1;
    }
} 
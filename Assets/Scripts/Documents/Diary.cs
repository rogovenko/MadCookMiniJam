using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Diary : Paper
{
    [Header("Развороты дневника")]
    [Tooltip("Список готовых разворотов (объектов)")]
    public List<GameObject> spreadObjects = new List<GameObject>();

    [Header("UI элементы для листания")]
    [Tooltip("Кнопка/область для листания назад (левый нижний угол)")]
    public Button prevSpreadButton;
    [Tooltip("Кнопка/область для листания вперёд (правый нижний угол)")]
    public Button nextSpreadButton;

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
        if (spreadObjects == null || spreadObjects.Count == 0) return;
        
        currentSpread = Mathf.Clamp(spreadIndex, 0, spreadObjects.Count - 1);
        
        // Отключаем все развороты
        for (int i = 0; i < spreadObjects.Count; i++)
        {
            if (spreadObjects[i] != null)
            {
                spreadObjects[i].SetActive(false);
            }
        }
        
        // Включаем только текущий разворот
        if (spreadObjects[currentSpread] != null)
        {
            spreadObjects[currentSpread].SetActive(true);
            Debug.Log($"Diary: Показан разворот {currentSpread + 1} из {spreadObjects.Count}");
        }
        
        UpdateButtons();
    }

    public void NextSpread()
    {
        if (gameManager.isTutorial)
        {
            if(gameManager.tutorial.TutorialStep == 2)
            {
                ShowSpread(currentSpread + 1);
                gameManager.tutorial.NextStep();
            }
            return;
        }

        if (currentSpread < spreadObjects.Count - 1)
            ShowSpread(currentSpread + 1);
    }

    public void PrevSpread()
    {
        if (gameManager.isTutorial)
        {
            return;
        }
        if (currentSpread > 0)
            ShowSpread(currentSpread - 1);
    }

    private void UpdateButtons()
    {
        if (prevSpreadButton != null)
            prevSpreadButton.interactable = currentSpread > 0;
        if (nextSpreadButton != null)
            nextSpreadButton.interactable = currentSpread < spreadObjects.Count - 1;
    }
    
    // Получить текущий разворот
    public int GetCurrentSpread()
    {
        return currentSpread;
    }
    
    // Получить общее количество разворотов
    public int GetTotalSpreads()
    {
        return spreadObjects.Count;
    }
    
    // Проверить, можно ли листать вперед
    public bool CanGoNext()
    {
        return currentSpread < spreadObjects.Count - 1;
    }
    
    // Проверить, можно ли листать назад
    public bool CanGoPrev()
    {
        return currentSpread > 0;
    }
    
    // Получить текущий активный разворот
    public GameObject GetCurrentSpreadObject()
    {
        if (currentSpread >= 0 && currentSpread < spreadObjects.Count)
        {
            return spreadObjects[currentSpread];
        }
        return null;
    }
    
    // Перейти к конкретному развороту по индексу
    public void GoToSpread(int index)
    {
        ShowSpread(index);
    }
    
    // Перейти к первому развороту
    public void GoToFirstSpread()
    {
        ShowSpread(0);
    }
    
    // Перейти к последнему развороту
    public void GoToLastSpread()
    {
        ShowSpread(spreadObjects.Count - 1);
    }
} 
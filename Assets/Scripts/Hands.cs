using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Hands : MonoBehaviour
{
    [Header("Пальцы левой руки")]
    [Tooltip("Изображения пальцев левой руки (5 пальцев)")]
    [SerializeField] private Image[] leftHandFingers = new Image[5];
    
    [Tooltip("Кнопки пальцев левой руки (5 кнопок)")]
    [SerializeField] private Button[] leftHandButtons = new Button[5];
    
    [Tooltip("Разводы крови левой руки (5 разводов)")]
    [SerializeField] private Image[] leftHandBlood = new Image[5];
    
    [Header("Пальцы правой руки")]
    [Tooltip("Изображения пальцев правой руки (5 пальцев)")]
    [SerializeField] private Image[] rightHandFingers = new Image[5];
    
    [Tooltip("Кнопки пальцев правой руки (5 кнопок)")]
    [SerializeField] private Button[] rightHandButtons = new Button[5];
    
    [Tooltip("Разводы крови правой руки (5 разводов)")]
    [SerializeField] private Image[] rightHandBlood = new Image[5];
    
    [Header("Настройки")]
    [Tooltip("Автоматически активировать кнопки при старте")]
    [SerializeField] private bool activateButtonsOnStart = true;
    
    [Header("End Game Manager")]
    [Tooltip("Ссылка на EndGameManager")]
    [SerializeField] private EndGameManager endGameManager;
    
    // Состояние пальцев (true = отрезан, false = целый)
    private bool[] leftHandFingerStates = new bool[5];
    private bool[] rightHandFingerStates = new bool[5];
    
    void Start()
    {
        // Получаем EndGameManager если не назначен
        if (endGameManager == null)
            endGameManager = FindObjectOfType<EndGameManager>();
        
        // Инициализируем состояние пальцев (все целые)
        for (int i = 0; i < 5; i++)
        {
            leftHandFingerStates[i] = false;
            rightHandFingerStates[i] = false;
        }
        
        // Скрываем всю кровь при старте
        HideAllBlood();
        
        // Подключаем обработчики кнопок
        SetupButtonHandlers();

        // Активируем кнопки если нужно
        if (activateButtonsOnStart)
        {
            ActivateAvailableButtons();
        }
    }
    
    // Настроить обработчики кнопок
    private void SetupButtonHandlers()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            int fingerIndex = i; // Замыкание для правильного индекса
            if (leftHandButtons[i] != null)
            {
                leftHandButtons[i].onClick.AddListener(() => OnLeftHandButtonClick(fingerIndex));
            }
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            int fingerIndex = i; // Замыкание для правильного индекса
            if (rightHandButtons[i] != null)
            {
                rightHandButtons[i].onClick.AddListener(() => OnRightHandButtonClick(fingerIndex));
            }
        }
    }
    
    // Обработчик клика кнопки левой руки
    private void OnLeftHandButtonClick(int fingerIndex)
    {
        CutFinger(true, fingerIndex); // true = левая рука
    }
    
    // Обработчик клика кнопки правой руки
    private void OnRightHandButtonClick(int fingerIndex)
    {
        CutFinger(false, fingerIndex); // false = правая рука
    }
    
    // Отрезать палец
    public void CutFinger(bool isLeftHand, int fingerIndex)
    {
        if (fingerIndex < 0 || fingerIndex >= 5)
        {
            Debug.LogError($"Hands: Неверный индекс пальца: {fingerIndex}");
            return;
        }
        
        // Проверяем, не отрезан ли уже палец
        bool[] fingerStates = isLeftHand ? leftHandFingerStates : rightHandFingerStates;
        if (fingerStates[fingerIndex])
        {
            Debug.LogWarning($"Hands: Палец уже отрезан! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
            return;
        }
        
        // Получаем соответствующие массивы
        Image[] fingers = isLeftHand ? leftHandFingers : rightHandFingers;
        Image[] blood = isLeftHand ? leftHandBlood : rightHandBlood;
        Button[] buttons = isLeftHand ? leftHandButtons : rightHandButtons;
        
        // Скрываем палец
        if (fingers[fingerIndex] != null)
        {
            fingers[fingerIndex].enabled = false;
        }
        
        // Показываем кровь
        if (blood[fingerIndex] != null)
        {
            blood[fingerIndex].enabled = true;
        }
        
        // Отключаем кнопку
        if (buttons[fingerIndex] != null)
        {
            buttons[fingerIndex].interactable = false;
        }
        
        // Обновляем состояние
        fingerStates[fingerIndex] = true;
        
        // Уведомляем EndGameManager об отрезанном пальце
        if (endGameManager != null)
        {
            endGameManager.HideFinger(isLeftHand, fingerIndex);
            Debug.Log($"Hands: Уведомлен EndGameManager об отрезанном пальце. Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
        }
        else
        {
            Debug.LogWarning("Hands: EndGameManager не найден!");
        }
        
        // Воспроизводим звук отрезания пальца
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTrash(); // Используем звук trash для отрезания пальца
        }
        
        Debug.Log($"Hands: Отрезан палец! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
    }
    
    // Активировать доступные кнопки (кроме отрезанных пальцев)
    public void ActivateAvailableButtons()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandButtons[i] != null)
            {
                leftHandButtons[i].interactable = !leftHandFingerStates[i];
            }
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandButtons[i] != null)
            {
                rightHandButtons[i].interactable = !rightHandFingerStates[i];
            }
        }
        
        Debug.Log("Hands: Активированы доступные кнопки");
    }
    
    // Деактивировать все кнопки пальцев
    public void DeactivateAllButtons()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandButtons[i] != null)
            {
                leftHandButtons[i].interactable = false;
            }
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandButtons[i] != null)
            {
                rightHandButtons[i].interactable = false;
            }
        }
        
        Debug.Log("Hands: Деактивированы все кнопки пальцев");
    }
    
    // Скрыть всю кровь
    public void HideAllBlood()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandBlood[i] != null)
            {
                leftHandBlood[i].enabled = false;
            }
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandBlood[i] != null)
            {
                rightHandBlood[i].enabled = false;
            }
        }
        
        Debug.Log("Hands: Скрыта вся кровь");
    }
    
    // Показать всю кровь (для отрезанных пальцев)
    public void ShowBloodForCutFingers()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandBlood[i] != null)
            {
                leftHandBlood[i].enabled = leftHandFingerStates[i];
            }
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandBlood[i] != null)
            {
                rightHandBlood[i].enabled = rightHandFingerStates[i];
            }
        }
        
        Debug.Log("Hands: Показана кровь для отрезанных пальцев");
    }
    
    // Сбросить все пальцы (восстановить)
    public void ResetAllFingers()
    {
        // Сбрасываем состояние
        for (int i = 0; i < 5; i++)
        {
            leftHandFingerStates[i] = false;
            rightHandFingerStates[i] = false;
        }
        
        // Показываем все пальцы
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingers[i] != null)
                leftHandFingers[i].enabled = true;
            if (rightHandFingers[i] != null)
                rightHandFingers[i].enabled = true;
        }
        
        // Скрываем всю кровь
        HideAllBlood();
        
        // Активируем все кнопки
        ActivateAvailableButtons();
        
        Debug.Log("Hands: Все пальцы восстановлены");
    }
    
    // Получить количество отрезанных пальцев
    public int GetCutFingersCount()
    {
        int count = 0;
        
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingerStates[i]) count++;
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingerStates[i]) count++;
        }
        
        return count;
    }
    
    // Получить количество отрезанных пальцев на левой руке
    public int GetLeftHandCutFingersCount()
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingerStates[i]) count++;
        }
        return count;
    }
    
    // Получить количество отрезанных пальцев на правой руке
    public int GetRightHandCutFingersCount()
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingerStates[i]) count++;
        }
        return count;
    }
    
    // Проверить, отрезан ли палец
    public bool IsFingerCut(bool isLeftHand, int fingerIndex)
    {
        if (fingerIndex < 0 || fingerIndex >= 5)
            return false;
            
        return isLeftHand ? leftHandFingerStates[fingerIndex] : rightHandFingerStates[fingerIndex];
    }
    
    // Получить информацию о руках
    public string GetHandsInfo()
    {
        return $"Руки:\n" +
               $"Левая рука: {GetLeftHandCutFingersCount()}/5 пальцев отрезано\n" +
               $"Правая рука: {GetRightHandCutFingersCount()}/5 пальцев отрезано\n" +
               $"Всего отрезано: {GetCutFingersCount()}/10 пальцев";
    }
} 
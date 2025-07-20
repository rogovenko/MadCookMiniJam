using UnityEngine;
using UnityEngine.UI;

public class HandsGame : MonoBehaviour
{
    [Header("Пальцы левой руки")]
    [Tooltip("Изображения пальцев левой руки (5 пальцев)")]
    [SerializeField] private Image[] leftHandFingers = new Image[5];
    
    [Header("Пальцы правой руки")]
    [Tooltip("Изображения пальцев правой руки (5 пальцев)")]
    [SerializeField] private Image[] rightHandFingers = new Image[5];
    
    [Header("Настройки")]
    [Tooltip("Автоматически скрывать пальцы при старте")]
    [SerializeField] private bool hideFingersOnStart = false;
    
    // Состояние пальцев (true = скрыт, false = видим)
    private bool[] leftHandFingerStates = new bool[5];
    private bool[] rightHandFingerStates = new bool[5];
    
    void Start()
    {
        // Инициализируем состояние пальцев (все видимы)
        for (int i = 0; i < 5; i++)
        {
            leftHandFingerStates[i] = false;
            rightHandFingerStates[i] = false;
        }
        
        // Скрываем пальцы при старте если нужно
        if (hideFingersOnStart)
        {
            HideAllFingers();
        }
    }
    
    // Скрыть палец
    public void HideFinger(bool isLeftHand, int fingerIndex)
    {
        if (fingerIndex < 0 || fingerIndex >= 5)
        {
            Debug.LogError($"HandsGame: Неверный индекс пальца: {fingerIndex}");
            return;
        }
        
        // Проверяем, не скрыт ли уже палец
        bool[] fingerStates = isLeftHand ? leftHandFingerStates : rightHandFingerStates;
        if (fingerStates[fingerIndex])
        {
            Debug.LogWarning($"HandsGame: Палец уже скрыт! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
            return;
        }
        
        // Получаем соответствующий массив
        Image[] fingers = isLeftHand ? leftHandFingers : rightHandFingers;
        
        // Скрываем палец
        if (fingers[fingerIndex] != null)
        {
            fingers[fingerIndex].enabled = false;
        }
        
        // Обновляем состояние
        fingerStates[fingerIndex] = true;
        
        Debug.Log($"HandsGame: Скрыт палец! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
    }
    
    // Показать палец
    public void ShowFinger(bool isLeftHand, int fingerIndex)
    {
        if (fingerIndex < 0 || fingerIndex >= 5)
        {
            Debug.LogError($"HandsGame: Неверный индекс пальца: {fingerIndex}");
            return;
        }
        
        // Проверяем, не показан ли уже палец
        bool[] fingerStates = isLeftHand ? leftHandFingerStates : rightHandFingerStates;
        if (!fingerStates[fingerIndex])
        {
            Debug.LogWarning($"HandsGame: Палец уже показан! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
            return;
        }
        
        // Получаем соответствующий массив
        Image[] fingers = isLeftHand ? leftHandFingers : rightHandFingers;
        
        // Показываем палец
        if (fingers[fingerIndex] != null)
        {
            fingers[fingerIndex].enabled = true;
        }
        
        // Обновляем состояние
        fingerStates[fingerIndex] = false;
        
        Debug.Log($"HandsGame: Показан палец! Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
    }
    
    // Скрыть все пальцы
    public void HideAllFingers()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingers[i] != null)
            {
                leftHandFingers[i].enabled = false;
            }
            leftHandFingerStates[i] = true;
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingers[i] != null)
            {
                rightHandFingers[i].enabled = false;
            }
            rightHandFingerStates[i] = true;
        }
        
        Debug.Log("HandsGame: Скрыты все пальцы");
    }
    
    // Показать все пальцы
    public void ShowAllFingers()
    {
        // Левая рука
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingers[i] != null)
            {
                leftHandFingers[i].enabled = true;
            }
            leftHandFingerStates[i] = false;
        }
        
        // Правая рука
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingers[i] != null)
            {
                rightHandFingers[i].enabled = true;
            }
            rightHandFingerStates[i] = false;
        }
        
        Debug.Log("HandsGame: Показаны все пальцы");
    }
    
    // Скрыть пальцы левой руки
    public void HideLeftHandFingers()
    {
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingers[i] != null)
            {
                leftHandFingers[i].enabled = false;
            }
            leftHandFingerStates[i] = true;
        }
        
        Debug.Log("HandsGame: Скрыты все пальцы левой руки");
    }
    
    // Показать пальцы левой руки
    public void ShowLeftHandFingers()
    {
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingers[i] != null)
            {
                leftHandFingers[i].enabled = true;
            }
            leftHandFingerStates[i] = false;
        }
        
        Debug.Log("HandsGame: Показаны все пальцы левой руки");
    }
    
    // Скрыть пальцы правой руки
    public void HideRightHandFingers()
    {
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingers[i] != null)
            {
                rightHandFingers[i].enabled = false;
            }
            rightHandFingerStates[i] = true;
        }
        
        Debug.Log("HandsGame: Скрыты все пальцы правой руки");
    }
    
    // Показать пальцы правой руки
    public void ShowRightHandFingers()
    {
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingers[i] != null)
            {
                rightHandFingers[i].enabled = true;
            }
            rightHandFingerStates[i] = false;
        }
        
        Debug.Log("HandsGame: Показаны все пальцы правой руки");
    }
    
    // Скрыть случайные пальцы
    public void HideRandomFingers(int count)
    {
        count = Mathf.Clamp(count, 0, 10); // Максимум 10 пальцев
        
        for (int i = 0; i < count; i++)
        {
            bool isLeftHand = Random.Range(0, 2) == 0;
            int fingerIndex = Random.Range(0, 5);
            
            // Проверяем, не скрыт ли уже палец
            bool[] fingerStates = isLeftHand ? leftHandFingerStates : rightHandFingerStates;
            if (!fingerStates[fingerIndex])
            {
                HideFinger(isLeftHand, fingerIndex);
            }
            else
            {
                i--; // Повторяем попытку
            }
        }
        
        Debug.Log($"HandsGame: Скрыто {count} случайных пальцев");
    }
    
    // Получить количество скрытых пальцев
    public int GetHiddenFingersCount()
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
    
    // Получить количество скрытых пальцев на левой руке
    public int GetLeftHandHiddenFingersCount()
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (leftHandFingerStates[i]) count++;
        }
        return count;
    }
    
    // Получить количество скрытых пальцев на правой руке
    public int GetRightHandHiddenFingersCount()
    {
        int count = 0;
        for (int i = 0; i < 5; i++)
        {
            if (rightHandFingerStates[i]) count++;
        }
        return count;
    }
    
    // Проверить, скрыт ли палец
    public bool IsFingerHidden(bool isLeftHand, int fingerIndex)
    {
        if (fingerIndex < 0 || fingerIndex >= 5)
            return false;
            
        return isLeftHand ? leftHandFingerStates[fingerIndex] : rightHandFingerStates[fingerIndex];
    }
    
    // Сбросить все пальцы (показать все)
    public void ResetAllFingers()
    {
        ShowAllFingers();
        Debug.Log("HandsGame: Все пальцы сброшены");
    }
    
    // Получить информацию о пальцах
    public string GetFingersInfo()
    {
        return $"Пальцы:\n" +
               $"Левая рука: {GetLeftHandHiddenFingersCount()}/5 пальцев скрыто\n" +
               $"Правая рука: {GetRightHandHiddenFingersCount()}/5 пальцев скрыто\n" +
               $"Всего скрыто: {GetHiddenFingersCount()}/10 пальцев";
    }
} 
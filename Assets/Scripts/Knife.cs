using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Knife : MonoBehaviour, IPointerClickHandler
{
    [Header("Компоненты ножа")]
    [Tooltip("Кнопка ножа")]
    [SerializeField] private Button knifeButton;
    
    [Tooltip("RectTransform ножа для позиционирования")]
    [SerializeField] private RectTransform knifeRect;
    
    [Header("Настройки следования")]
    [Tooltip("Скорость следования за курсором")]
    [SerializeField] private float followSpeed = 10f;
    
    [Tooltip("Смещение от курсора")]
    [SerializeField] private Vector2 cursorOffset = Vector2.zero;
    
    [Header("Пальцы")]
    [Tooltip("Компонент Hands для управления пальцами")]
    [SerializeField] private Hands hands;
    
    [Header("Состояние")]
    [Tooltip("Следует ли нож за курсором")]
    [SerializeField] private bool isFollowingCursor = false;
    
    // Сохраненная исходная позиция
    private Vector2 originalPosition;
    private bool hasOriginalPosition = false;
    
    void Start()
    {
        // Получаем компоненты если не назначены
        if (knifeButton == null)
            knifeButton = GetComponent<Button>();
            
        if (knifeRect == null)
            knifeRect = GetComponent<RectTransform>();
        
        // Получаем компонент Hands если не назначен
        if (hands == null)
            hands = FindObjectOfType<Hands>();
        
        // Сохраняем исходную позицию
        if (knifeRect != null)
        {
            originalPosition = knifeRect.anchoredPosition;
            hasOriginalPosition = true;
        }
        
        // Добавляем обработчик клика на кнопку
        if (knifeButton != null)
        {
            knifeButton.onClick.AddListener(OnKnifeButtonClick);
        }
    }
    
    void Update()
    {
        if (isFollowingCursor && knifeRect != null)
        {
            FollowCursor();
        }
    }
    
    // Обработчик клика на нож (IPointerClickHandler)
    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleFollowMode();
    }
    
    // Обработчик клика на кнопку ножа
    public void OnKnifeButtonClick()
    {
        ToggleFollowMode();
    }
    
    // Переключить режим следования
    public void ToggleFollowMode()
    {
        isFollowingCursor = !isFollowingCursor;
        
        if (isFollowingCursor)
        {
            StartFollowing();
        }
        else
        {
            StopFollowing();
        }
        
        Debug.Log($"Knife: Режим следования {(isFollowingCursor ? "включен" : "выключен")}");
    }
    
    // Начать следование за курсором
    public void StartFollowing()
    {
        isFollowingCursor = true;
        
        // Активируем кнопки пальцев
        if (hands != null)
        {
            hands.ActivateAvailableButtons();
            Debug.Log("Knife: Активированы кнопки пальцев");
        }
        
        Debug.Log("Knife: Начинаем следование за курсором");
    }
    
    // Остановить следование и вернуться в исходную позицию
    public void StopFollowing()
    {
        isFollowingCursor = false;
        
        // Деактивируем все кнопки пальцев
        if (hands != null)
        {
            hands.DeactivateAllButtons();
            Debug.Log("Knife: Деактивированы все кнопки пальцев");
        }
        
        if (knifeRect != null && hasOriginalPosition)
        {
            knifeRect.anchoredPosition = originalPosition;
            Debug.Log("Knife: Возвращаемся в исходную позицию");
        }
    }
    
    // Следование за курсором
    private void FollowCursor()
    {
        if (knifeRect == null) return;
        
        // Получаем позицию курсора в экранных координатах
        Vector2 mousePosition = Input.mousePosition;
        
        // Преобразуем в локальные координаты Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePosition, null, out Vector2 localPoint))
            {
                // Добавляем смещение
                Vector2 targetPosition = localPoint + cursorOffset;
                
                // Плавно перемещаем нож к позиции курсора
                Vector2 currentPosition = knifeRect.anchoredPosition;
                Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, followSpeed * Time.deltaTime);
                
                knifeRect.anchoredPosition = newPosition;
            }
        }
    }
    
    // Установить исходную позицию
    public void SetOriginalPosition(Vector2 position)
    {
        originalPosition = position;
        hasOriginalPosition = true;
        
        if (!isFollowingCursor && knifeRect != null)
        {
            knifeRect.anchoredPosition = originalPosition;
        }
    }
    
    // Получить текущую позицию
    public Vector2 GetCurrentPosition()
    {
        return knifeRect != null ? knifeRect.anchoredPosition : Vector2.zero;
    }
    
    // Получить исходную позицию
    public Vector2 GetOriginalPosition()
    {
        return originalPosition;
    }
    
    // Проверить, следует ли нож за курсором
    public bool IsFollowingCursor()
    {
        return isFollowingCursor;
    }
    
    // Установить скорость следования
    public void SetFollowSpeed(float speed)
    {
        followSpeed = Mathf.Max(0.1f, speed);
    }
    
    // Установить смещение от курсора
    public void SetCursorOffset(Vector2 offset)
    {
        cursorOffset = offset;
    }
    
    // Принудительно вернуться в исходную позицию
    public void ReturnToOriginalPosition()
    {
        if (knifeRect != null && hasOriginalPosition)
        {
            knifeRect.anchoredPosition = originalPosition;
            Debug.Log("Knife: Принудительно возвращаемся в исходную позицию");
        }
    }
    
    // Установить активность кнопки
    public void SetButtonInteractable(bool interactable)
    {
        if (knifeButton != null)
        {
            knifeButton.interactable = interactable;
        }
    }
    
    // Получить информацию о ноже
    public string GetKnifeInfo()
    {
        string handsInfo = hands != null ? hands.GetHandsInfo() : "Пальцы не найдены";
        
        return $"Нож: {(isFollowingCursor ? "следует за курсором" : "в исходной позиции")}\n" +
               $"Скорость следования: {followSpeed}\n" +
               $"Смещение: {cursorOffset}\n" +
               $"Текущая позиция: {GetCurrentPosition()}\n" +
               $"Исходная позиция: {originalPosition}\n" +
               $"\n{handsInfo}";
    }
    
    // Методы для управления пальцами из ножа
    
    // Отрезать палец
    public void CutFinger(bool isLeftHand, int fingerIndex)
    {
        if (hands != null)
        {
            hands.CutFinger(isLeftHand, fingerIndex);
        }
        else
        {
            Debug.LogWarning("Knife: Компонент Hands не найден!");
        }
    }
    
    // Активировать доступные кнопки пальцев
    public void ActivateFingerButtons()
    {
        if (hands != null)
        {
            hands.ActivateAvailableButtons();
        }
        else
        {
            Debug.LogWarning("Knife: Компонент Hands не найден!");
        }
    }
    
    // Деактивировать все кнопки пальцев
    public void DeactivateFingerButtons()
    {
        if (hands != null)
        {
            hands.DeactivateAllButtons();
        }
        else
        {
            Debug.LogWarning("Knife: Компонент Hands не найден!");
        }
    }
    
    // Скрыть всю кровь
    public void HideAllBlood()
    {
        if (hands != null)
        {
            hands.HideAllBlood();
        }
        else
        {
            Debug.LogWarning("Knife: Компонент Hands не найден!");
        }
    }
    
    // Сбросить все пальцы
    public void ResetAllFingers()
    {
        if (hands != null)
        {
            hands.ResetAllFingers();
        }
        else
        {
            Debug.LogWarning("Knife: Компонент Hands не найден!");
        }
    }
    
    // Получить количество отрезанных пальцев
    public int GetCutFingersCount()
    {
        return hands != null ? hands.GetCutFingersCount() : 0;
    }
    
    // Установить компонент Hands
    public void SetHands(Hands handsComponent)
    {
        hands = handsComponent;
        Debug.Log("Knife: Компонент Hands установлен");
    }
} 
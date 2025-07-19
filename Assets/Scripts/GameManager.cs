using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Ссылки на менеджеры")]
    [Tooltip("Менеджер персонажей")]
    public CharManager charManager;
    
    [Tooltip("Менеджер очереди")]
    public QueueManager queueManager;
    
    [Tooltip("Таймер игры")]
    public Timer gameTimer;
    
    [Tooltip("Система событий drag'n'drop")]
    public DragEventSystem dragEventSystem;
    
    [Tooltip("Кнопка Tasty")]
    public TastyButton tastyButton;
    
    [Header("Настройки игры")]
    [Tooltip("Создать персонажа при старте игры")]
    [SerializeField] private bool createCharacterOnStart = true;
    
    [Tooltip("Режим выбора заказа")]
    [SerializeField] private bool chooseOrderMode = false;
    
    [Header("Текущий персонаж")]
    [Tooltip("Текущий активный персонаж")]
    [SerializeField] private GameObject currentCharacter;
    
    void Start()
    {
        // Проверяем наличие CharManager
        if (charManager == null)
        {
            charManager = FindObjectOfType<CharManager>();
            if (charManager == null)
            {
                Debug.LogError("GameManager: CharManager не найден на сцене!");
                return;
            }
        }
        
        // Проверяем наличие QueueManager
        if (queueManager == null)
        {
            queueManager = FindObjectOfType<QueueManager>();
            if (queueManager == null)
            {
                Debug.LogError("GameManager: QueueManager не найден на сцене!");
                return;
            }
        }
        
        // Проверяем наличие Timer
        if (gameTimer == null)
        {
            gameTimer = FindObjectOfType<Timer>();
            if (gameTimer == null)
            {
                Debug.LogError("GameManager: Timer не найден на сцене!");
                return;
            }
        }
        
        // Проверяем наличие DragEventSystem
        if (dragEventSystem == null)
        {
            dragEventSystem = FindObjectOfType<DragEventSystem>();
            if (dragEventSystem == null)
            {
                Debug.LogError("GameManager: DragEventSystem не найден на сцене!");
                return;
            }
        }
        
        // Проверяем наличие TastyButton
        if (tastyButton == null)
        {
            tastyButton = FindObjectOfType<TastyButton>();
            if (tastyButton == null)
            {
                Debug.LogError("GameManager: TastyButton не найден на сцене!");
                return;
            }
        }
        
        // Подписываемся на событие готовности очереди
        if (queueManager != null)
        {
            queueManager.OnQueueReady += OnQueueReady;
        }
        
        // Подписываемся на события таймера
        if (gameTimer != null)
        {
            gameTimer.OnTimerFinished += OnGameTimerFinished;
        }
    }
    
    // Вызывается когда очередь готова
    private void OnQueueReady()
    {
        // Создаем персонажа только когда очередь готова
        if (createCharacterOnStart)
        {
            CreateCharacter();
        }
    }
    
    // Вызывается когда таймер истек
    private void OnGameTimerFinished()
    {
        Debug.Log("GameManager: Время игры истекло!");
        // Здесь можно добавить логику окончания игры
        // Например, показать экран Game Over
    }
    
    // Создать персонажа при старте игры
    public void CreateCharacter()
    {
        if (charManager != null && queueManager != null)
        {
            // Проверяем, есть ли персонажи в очереди
            if (queueManager.GetQueueCount() > 0)
            {
                // Получаем первого персонажа из очереди и удаляем его из очереди
                CharacterType characterType = queueManager.RemoveFirstFromQueue();
                GameObject character = charManager.CreateCharacter(characterType);
                if (character != null)
                {
                    SetCurrentCharacter(character);
                    
                    // Запускаем таймер после создания первого персонажа
                    if (gameTimer != null)
                    {
                        gameTimer.StartTimer();
                        Debug.Log("GameManager: Таймер запущен!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("GameManager: Очередь пуста, персонаж не создан!");
            }
        }
        else
        {
            Debug.LogError("GameManager: CharManager или QueueManager не назначены!");
        }
    }
    
    // Установить текущего персонажа
    public void SetCurrentCharacter(GameObject character)
    {
        currentCharacter = character;
    }
    
    // Уничтожить текущего персонажа
    public void DestroyCurrentCharacter()
    {
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
            currentCharacter = null;
            CreateCharacter();
        }
        else
        {
            Debug.LogWarning("GameManager: Нет текущего персонажа для уничтожения!");
        }
    }
    
    // Действие "Trash" - отклонить персонажа
    public void TrashAction()
    {
        DestroyCurrentCharacter();
    }
    
    // Действие "Tasty" - принять персонажа
    public void TastyAction()
    {
        // Переключаем режим выбора заказа
        chooseOrderMode = !chooseOrderMode;
        
        Debug.Log($"GameManager: Режим выбора заказа изменен на {(chooseOrderMode ? "активный" : "неактивный")}");
        
        // Управляем drag'n'drop через DragEventSystem
        if (dragEventSystem != null)
        {
            if (chooseOrderMode)
            {
                dragEventSystem.DisableDragDrop();
            }
            else
            {
                dragEventSystem.EnableDragDrop();
            }
        }
        
        // DestroyCurrentCharacter();
    }
    
    // Получить текущего персонажа
    public GameObject GetCurrentCharacter()
    {
        return currentCharacter;
    }
    
    // Методы для работы с таймером
    public void AddTimeToTimer(float seconds)
    {
        if (gameTimer != null)
        {
            gameTimer.AddTime(seconds);
        }
    }
    
    public void StopGameTimer()
    {
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }
    }
    
    public void StartGameTimer()
    {
        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }
    }
    
    public float GetCurrentGameTime()
    {
        return gameTimer != null ? gameTimer.GetCurrentTime() : 0f;
    }
    
    public bool IsGameTimeUp()
    {
        return gameTimer != null && gameTimer.IsTimeUp();
    }
    
    // Методы для работы с drag'n'drop
    public void EnableDragDrop()
    {
        if (dragEventSystem != null)
        {
            dragEventSystem.EnableDragDrop();
        }
    }
    
    public void DisableDragDrop()
    {
        if (dragEventSystem != null)
        {
            dragEventSystem.DisableDragDrop();
        }
    }
    
    public void SetDragDropEnabled(bool enabled)
    {
        if (dragEventSystem != null)
        {
            dragEventSystem.SetDragDropEnabled(enabled);
        }
    }
    
    public bool IsDragDropEnabled()
    {
        return dragEventSystem != null && dragEventSystem.IsDragDropEnabled();
    }
    
    // Методы для работы с режимом выбора заказа
    public bool IsChooseOrderMode()
    {
        return chooseOrderMode;
    }
    
    // Методы для работы с кнопкой Tasty
    public void MakeTastyButtonActive()
    {
        if (tastyButton != null)
        {
            tastyButton.MakeActive();
        }
    }
    
    public void MakeTastyButtonInactive()
    {
        if (tastyButton != null)
        {
            tastyButton.MakeInactive();
        }
    }
    
    public bool IsTastyButtonActive()
    {
        return tastyButton != null && tastyButton.IsActivated();
    }
    
    public void SetChooseOrderMode(bool mode)
    {
        chooseOrderMode = mode;
        Debug.Log($"GameManager: Режим выбора заказа установлен на {(chooseOrderMode ? "активный" : "неактивный")}");
        
        // Управляем drag'n'drop в зависимости от режима
        if (dragEventSystem != null)
        {
            if (chooseOrderMode)
            {
                dragEventSystem.DisableDragDrop();
            }
            else
            {
                dragEventSystem.EnableDragDrop();
            }
        }
        
        // Управляем кнопкой Tasty
        if (tastyButton != null)
        {
            if (chooseOrderMode)
            {
                tastyButton.MakeInactive();
            }
            else
            {
                tastyButton.MakeActive();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (queueManager != null)
        {
            queueManager.OnQueueReady -= OnQueueReady;
        }
        
        // Отписываемся от событий таймера
        if (gameTimer != null)
        {
            gameTimer.OnTimerFinished -= OnGameTimerFinished;
        }
    }
} 
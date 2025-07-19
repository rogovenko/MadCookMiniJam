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
    
    [Header("Настройки игры")]
    [Tooltip("Создать персонажа при старте игры")]
    [SerializeField] private bool createCharacterOnStart = true;
    
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
        DestroyCurrentCharacter();
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
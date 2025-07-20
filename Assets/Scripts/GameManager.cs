using UnityEngine;
using System.Collections.Generic;

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
    
    [Tooltip("Спавнер бумаг")]
    public PaperSpawner paperSpawner;
    
    [Tooltip("Менеджер рецептов")]
    public RecipeManager recipeManager;
    
    [Header("Рецепты уровня")]
    [Tooltip("Рецепты, которые нужно выполнить на текущем уровне")]
    public List<RecipeData> currentLevelRecipes = new List<RecipeData>();
    
    [Header("Настройки заказов")]
    [Tooltip("Префаб заказа")]
    public GameObject orderPrefab;
    
    [Tooltip("Canvas для размещения заказов")]
    public Canvas orderCanvas;
    
    [Tooltip("Позиция спавна заказов")]
    public Transform orderSpawnPosition;
    
    [Tooltip("Оффсет между заказами по оси X")]
    public float orderOffsetX = 2f;
    
    [Header("Настройки игры")]
    [Tooltip("Создать персонажа при старте игры")]
    [SerializeField] private bool createCharacterOnStart = true;
    
    [Tooltip("Режим выбора заказа")]
    [SerializeField] private bool chooseOrderMode = false;
    
    [Header("Текущий персонаж")]
    [Tooltip("Текущий активный персонаж")]
    [SerializeField] private GameObject currentCharacter;
    
    [Header("Текущая бумажка")]
    [Tooltip("Текущая созданная бумажка")]
    [SerializeField] private GameObject currentPaper;
    
    [Header("Настройки очереди")]
    [Tooltip("Начальная очередь персонажей")]
    [SerializeField] private List<CharInfo> initialQueue = new List<CharInfo>();
    
    [Tooltip("Создать очередь при старте игры")]
    [SerializeField] private bool createQueueOnStart = true;
    
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
        
        // Проверяем наличие PaperSpawner
        if (paperSpawner == null)
        {
            paperSpawner = FindObjectOfType<PaperSpawner>();
            if (paperSpawner == null)
            {
                Debug.LogError("GameManager: PaperSpawner не найден на сцене!");
                return;
            }
        }
        
        // Проверяем наличие RecipeManager
        if (recipeManager == null)
        {
            recipeManager = FindObjectOfType<RecipeManager>();
            if (recipeManager == null)
            {
                Debug.LogError("GameManager: RecipeManager не найден на сцене!");
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
        
        // Создаем очередь при старте
        if (createQueueOnStart)
        {
            DefineQueue();
            CreateQueue();
            InitRecipes();
        }
    }

    public void DefineQueue()
    {
        if (currentLevelRecipes == null || currentLevelRecipes.Count == 0)
        {
            Debug.LogWarning("GameManager: Список currentLevelRecipes пуст! Нечего подсчитывать.");
            return;
        }
        
        // Словарь для подсчета овощей
        Dictionary<CharacterType, int> vegetableCounts = new Dictionary<CharacterType, int>();
        
        // Инициализируем все типы овощей с нулевым количеством
        foreach (CharacterType vegType in (CharacterType[])System.Enum.GetValues(typeof(CharacterType)))
        {
            vegetableCounts[vegType] = 0;
        }
        
        // Подсчитываем овощи в рецептах
        foreach (RecipeData recipe in currentLevelRecipes)
        {
            if (recipe != null && recipe.ingredients != null)
            {
                foreach (RecipeIngredient ingredient in recipe.ingredients)
                {
                    CharacterType vegType = ingredient.ingredientType;
                    int amount = ingredient.amount;
                    
                    if (vegetableCounts.ContainsKey(vegType))
                    {
                        vegetableCounts[vegType] += amount;
                    }
                }
            }
        }
        
        // Выводим результат в консоль
        Debug.Log("GameManager: Подсчет овощей в рецептах:");
        foreach (var kvp in vegetableCounts)
        {
            if (kvp.Value > 0) // Показываем только те, которые встречаются
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value} шт.");
            }
        }
        
        // Здесь можно использовать vegetableCounts для создания очереди
        // Например, создать персонажей в соответствии с количеством овощей
        CreateQueueFromVegetableCounts(vegetableCounts);
    }
    
    // Создать очередь на основе подсчитанных овощей
    private void CreateQueueFromVegetableCounts(Dictionary<CharacterType, int> vegetableCounts)
    {
        if (queueManager == null)
        {
            Debug.LogError("GameManager: QueueManager не назначен! Нельзя создать очередь.");
            return;
        }
        
        // Создаем список CharInfo для очереди
        List<CharInfo> queueCharacters = new List<CharInfo>();
        
        // Получаем текущую дату
        System.DateTime currentDate = System.DateTime.Now;
        
        // Добавляем персонажей в соответствии с количеством овощей (первая половина - правильные)
        foreach (var kvp in vegetableCounts)
        {
            CharacterType vegType = kvp.Key;
            int count = kvp.Value;
            
            // Добавляем персонажа столько раз, сколько нужно овощей этого типа
            for (int i = 0; i < count; i++)
            {
                // Создаем правильный персонаж без ошибок и дефектов
                CharInfo correctCharacter = new CharInfo(vegType, false, false, currentDate);
                queueCharacters.Add(correctCharacter);
            }
        }
        
        // Перемешиваем первую половину (овощи из рецептов)
        ShuffleList(queueCharacters);
        
        // Подсчитываем общее количество овощей из рецептов
        int requiredVegetablesCount = queueCharacters.Count;
        
        // Вычисляем количество ошибок для случайных овощей
        int errorNum = Mathf.RoundToInt(requiredVegetablesCount / 2f);
        
        // Создаем списки индексов для распределения ошибок
        List<int> defectIndices = new List<int>();
        List<int> mistakeIndices = new List<int>();
        
        // Генерируем случайные индексы для дефектов
        for (int i = 0; i < errorNum; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, requiredVegetablesCount);
            } while (defectIndices.Contains(randomIndex));
            defectIndices.Add(randomIndex);
        }
        
        // Генерируем случайные индексы для ошибок
        for (int i = 0; i < errorNum; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, requiredVegetablesCount);
            } while (mistakeIndices.Contains(randomIndex));
            mistakeIndices.Add(randomIndex);
        }
        
        // Добавляем случайные овощи (вторая половина)
        List<CharacterType> allVegetableTypes = new List<CharacterType>((CharacterType[])System.Enum.GetValues(typeof(CharacterType)));
        
        for (int i = 0; i < requiredVegetablesCount; i++)
        {
            // Выбираем случайный тип овоща
            CharacterType randomVegType = allVegetableTypes[Random.Range(0, allVegetableTypes.Count)];
            
            // Определяем, нужны ли ошибки для этого персонажа
            bool hasDefect = defectIndices.Contains(i);
            bool hasMistake = mistakeIndices.Contains(i);
            
            // Создаем случайный персонаж с возможными ошибками
            CharInfo randomCharacter = new CharInfo(randomVegType, hasMistake, hasDefect, currentDate);
            queueCharacters.Add(randomCharacter);
        }
        
        // Перемешиваем всю очередь
        ShuffleList(queueCharacters);
        
        // Сохраняем очередь для использования в CreateQueue()
        initialQueue = queueCharacters;
        
        Debug.Log($"GameManager: Создана очередь из {queueCharacters.Count} персонажей:");
        Debug.Log($"  - {requiredVegetablesCount} правильных овощей из рецептов");
        Debug.Log($"  - {requiredVegetablesCount} случайных овощей");
        Debug.Log($"  - {errorNum} случайных овощей с дефектами");
        Debug.Log($"  - {errorNum} случайных овощей с ошибками в документах");
    }
    
    // Перемешивание списка (алгоритм Fisher-Yates)
    private void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    // Создать очередь персонажей
    public void CreateQueue()
    {
        if (queueManager != null)
        {
            queueManager.CreateQueue(initialQueue);
        }
        else
        {
            Debug.LogError("GameManager: QueueManager не назначен!");
        }
    }
    
    // Метод для инициализации рецептов - создает заказы для каждого рецепта
    public void InitRecipes()
    {
        if (currentLevelRecipes == null || currentLevelRecipes.Count == 0)
        {
            Debug.LogWarning("GameManager: Список currentLevelRecipes пуст!");
            return;
        }
        
        // Проверяем необходимые компоненты
        if (orderPrefab == null)
        {
            Debug.LogError("GameManager: orderPrefab не назначен! Нельзя создать заказы.");
            return;
        }
        
        if (orderCanvas == null)
        {
            Debug.LogError("GameManager: orderCanvas не назначен! Нельзя создать заказы.");
            return;
        }
        
        if (orderSpawnPosition == null)
        {
            Debug.LogError("GameManager: orderSpawnPosition не назначен! Нельзя создать заказы.");
            return;
        }

        foreach (RecipeData recipe in currentLevelRecipes)
        {
            if (recipe != null && !string.IsNullOrEmpty(recipe.recipeName))
            {
                // Создаем заказ с названием рецепта
                CreateOrder(recipe.recipeName);
            }
        }
    }
    
    // Вспомогательный метод для создания заказа
    private void CreateOrder(string recipeName)
    {
        // Проверки уже выполнены в InitRecipes()
        
        // Вычисляем позицию для заказа с учетом оффсета
        Vector3 spawnPosition = orderSpawnPosition.position;
        
        // Если это не первый заказ, добавляем оффсет
        int orderIndex = GetCurrentOrderCount();
        if (orderIndex > 0)
        {
            spawnPosition.x += orderIndex * orderOffsetX;
        }
        
        // Конвертируем мировую позицию в локальную позицию Canvas
        Vector2 localPosition = orderCanvas.transform.InverseTransformPoint(spawnPosition);
        
        // Создаем заказ как дочерний объект Canvas
        GameObject orderObject = Instantiate(orderPrefab, orderCanvas.transform);
        
        // Устанавливаем позицию заказа
        RectTransform orderRectTransform = orderObject.GetComponent<RectTransform>();
        if (orderRectTransform != null)
        {
            // Используем локальную позицию Canvas
            orderRectTransform.anchoredPosition = localPosition;
        }
        else
        {
            // Если нет RectTransform, используем обычную позицию
            orderObject.transform.position = spawnPosition;
        }
        
        // Настраиваем текст заказа (если есть компонент Order)
        Order orderComponent = orderObject.GetComponent<Order>();
        if (orderComponent != null)
        {
            // Используем корутину чтобы дождаться Start()
            StartCoroutine(SetOrderTextAfterStart(orderComponent, recipeName));
        }
        else
        {
            Debug.LogWarning($"GameManager: На созданном заказе отсутствует компонент Order!");
        }
    }
    
    // Корутина для установки текста заказа после Start()
    private System.Collections.IEnumerator SetOrderTextAfterStart(Order orderComponent, string recipeName)
    {
        // Ждем один кадр чтобы Start() успел выполниться
        yield return null;
        
        // Теперь устанавливаем текст
        orderComponent.SetOrderText(recipeName);
    }
    
    // Получить количество текущих заказов (для вычисления оффсета)
    private int GetCurrentOrderCount()
    {
        // Ищем все объекты с компонентом Order на сцене
        Order[] existingOrders = FindObjectsOfType<Order>();
        return existingOrders.Length;
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
                CharInfo charInfo = queueManager.RemoveFirstFromQueue();
                GameObject character = charManager.CreateCharacter(charInfo.characterType);
                if (character != null)
                {
                    // Применяем информацию о персонаже (дефекты, сорт, место происхождения и т.д.)
                    ApplyCharacterInfo(character, charInfo);
                    
                    SetCurrentCharacter(character);
                    
                    // Запускаем таймер после создания первого персонажа
                    if (gameTimer != null)
                    {
                        gameTimer.StartTimer();
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
    
    // Применить информацию о персонаже к созданному объекту
    private void ApplyCharacterInfo(GameObject character, CharInfo charInfo)
    {
        Debug.Log($"GameManager: Применена информация к персонажу {charInfo.name} ({charInfo.GetVarietyDisplayName()} из {charInfo.GetOriginDisplayName()})");
        // Передаем CharInfo в компонент Character
        Character characterComponent = character.GetComponent<Character>();
        if (characterComponent != null)
        {
            characterComponent.SetCharacterInfo(charInfo);
        }
        else
        {
            Debug.LogWarning("GameManager: На персонаже отсутствует компонент Character!");
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
            // Уничтожаем текущую бумажку, если она есть
            DestroyCurrentPaper();
            
            // Уничтожаем персонажа
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
    
    // "Раздеть" текущего персонажа
    public void StripCurrentCharacter()
    {
        if (currentCharacter != null)
        {
            Character characterComponent = currentCharacter.GetComponent<Character>();
            if (characterComponent != null)
            {
                characterComponent.GetNaked();
            }
            else
            {
                Debug.LogError("GameManager: На текущем персонаже отсутствует компонент Character!");
            }
        }
        else
        {
            Debug.LogWarning("GameManager: Нет текущего персонажа для раздевания!");
        }
    }
    
    // Запустить спавн бумаг
    public void PapersPlease()
    {
        if (currentCharacter != null)
        {
            Character characterComponent = currentCharacter.GetComponent<Character>();
            if (characterComponent != null)
            {
                // Проверяем, была ли уже запрошена бумажка у этого персонажа
                if (characterComponent.HasBeenAskedForPapers())
                {
                    Debug.LogWarning("GameManager: У этого персонажа уже была запрошена бумажка!");
                    return;
                }
                
                // Отмечаем персонажа как уже запрошенного
                characterComponent.MarkAsAskedForPapers();
                
                // Запускаем спавн бумаг
                if (paperSpawner != null)
                {
                    // Создаем бумажку и сохраняем ссылку на неё
                    currentPaper = paperSpawner.SpawnPaper();
                }
                else
                {
                    Debug.LogError("GameManager: PaperSpawner не назначен!");
                }
            }
            else
            {
                Debug.LogError("GameManager: На текущем персонаже отсутствует компонент Character!");
            }
        }
        else
        {
            Debug.LogWarning("GameManager: Нет текущего персонажа для запроса бумажек!");
        }
    }
    
    // Уничтожить текущую бумажку
    public void DestroyCurrentPaper()
    {
        if (currentPaper != null)
        {
            Destroy(currentPaper);
            currentPaper = null;
        }
    }
    
    // Получить текущую бумажку
    public GameObject GetCurrentPaper()
    {
        return currentPaper;
    }
    
    // Установить текущую бумажку
    public void SetCurrentPaper(GameObject paper)
    {
        currentPaper = paper;
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
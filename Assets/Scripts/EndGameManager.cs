using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndGameManager : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Ссылка на GameManager")]
    [SerializeField] private GameManager gameManager;
    
    [Tooltip("Объект для спавна заказов (Vertical Layout Group)")]
    [SerializeField] private Transform orderSpawn;
    
    [Header("Префабы")]
    [Tooltip("Префаб заказа для отображения в конце игры")]
    [SerializeField] private GameObject orderPrefab;
    
    [Header("UI Тексты")]
    [Tooltip("Первый текст окончания игры")]
    [SerializeField] private TMPro.TextMeshProUGUI endText1;
    
    [Tooltip("Второй текст окончания игры")]
    [SerializeField] private TMPro.TextMeshProUGUI endText2;
    
    [Header("UI Кнопки")]
    [Tooltip("Кнопка перехода на следующий уровень")]
    [SerializeField] private Button nextLevelButton;
    
    [Header("Управление уровнями")]
    [Tooltip("Текущий уровень")]
    [SerializeField] private int currentLevel = 1;
    [Tooltip("Максимальное количество уровней")]
    [SerializeField] private int maxLevels = 5;
    
    [Header("Игровые объекты")]
    [Tooltip("Нож в EndGame")]
    [SerializeField] private Knife endGameKnife;
    
    [Header("Экраны")]
    [Tooltip("Экран окончания игры (Game Over)")]
    [SerializeField] private GameObject gameOverScreen;
    
    [Header("UI Тексты Game Over")]
    [Tooltip("Сообщение Game Over")]
    [SerializeField] private TMPro.TextMeshProUGUI gameOverMessage;
    
    [Header("Статистика заказов")]
    [Tooltip("Количество проваленных заказов")]
    [SerializeField] private int failedOrdersCount = 0;
    
    void Start()
    {
        // Если GameManager не назначен, пытаемся найти его
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("EndGameManager: GameManager не найден на сцене!");
                return;
            }
        }
        
        // Инициализируем кнопку NextLevel
        InitializeNextLevelButton();
        
        // Инициализируем нож
        InitializeEndGameKnife();
        
        // Инициализируем GameOverScreen
        InitializeGameOverScreen();
        
        // Скрываем UI по умолчанию
        // TODO вернуть
        gameObject.SetActive(false);
    }
    
    // Показать экран окончания игры
    public void ShowEndGameScreen()
    {
        Debug.Log("EndGameManager: Показываем экран окончания игры");
        gameObject.SetActive(true);
        
        // Сбрасываем счетчик проваленных заказов
        ResetFailedOrdersCount();
        
        // Обновляем тексты после сброса счетчика
        UpdateEndTexts();
        
        // Создаем заказы с штампами
        CreateOrdersWithStamps();
    }
    
    // Создать заказы с штампами результата
    private void CreateOrdersWithStamps()
    {
        if (gameManager == null || orderSpawn == null || orderPrefab == null)
        {
            Debug.LogError("EndGameManager: Не хватает ссылок для создания заказов!");
            return;
        }
        
        // Получаем все заказы из GameManager
        List<Order> allOrders = gameManager.GetAllOrders();
        
        Debug.Log($"EndGameManager: Создаем {allOrders.Count} заказов с штампами");
        
        // Запускаем корутину для создания заказов
        StartCoroutine(CreateOrdersWithStampsCoroutine(allOrders));
    }
    
    // Корутина для создания заказов с ожиданием инициализации
    private System.Collections.IEnumerator CreateOrdersWithStampsCoroutine(List<Order> allOrders)
    {
        foreach (Order originalOrder in allOrders)
        {
            // Создаем копию заказа
            GameObject orderObject = Instantiate(orderPrefab, orderSpawn);
            Order newOrder = orderObject.GetComponent<Order>();
            
            if (newOrder != null)
            {
                // Ждем один кадр, чтобы заказ полностью инициализировался
                yield return null;
                
                // Копируем рецепт
                RecipeData recipe = originalOrder.GetOrderRecipe();
                if (recipe != null)
                {
                    newOrder.SetOrderRecipe(recipe);
                    // Устанавливаем текст заказа после инициализации
                    newOrder.SetOrderText(recipe.recipeName);
                }
                
                // Копируем добавленных персонажей
                List<CharInfo> addedChars = originalOrder.GetAddedCharacters();
                foreach (CharInfo charInfo in addedChars)
                {
                    newOrder.AddCharacter(charInfo);
                }
                
                // Копируем наклейки из оригинального заказа
                List<CharacterType> orderedVegetables = originalOrder.GetOrderedVegetables();
                foreach (CharacterType vegType in orderedVegetables)
                {
                    newOrder.AddSticker(vegType);
                }
                
                Debug.Log($"EndGameManager: Скопировано {orderedVegetables.Count} наклеек для заказа '{recipe?.recipeName}'");
                
                // Проверяем правильность ингредиентов (только ингредиенты, не ошибки/дефекты)
                bool isCorrectlyAssembled = newOrder.CheckIngredientsCorrectness();
                
                // Увеличиваем счетчик проваленных заказов если заказ собран неправильно
                if (!isCorrectlyAssembled)
                {
                    IncrementFailedOrdersCount();
                }
                
                // Показываем соответствующий штамп
                newOrder.ShowResultStamp(isCorrectlyAssembled);
                
                // Отключаем интерактивность заказа
                Button orderButton = orderObject.GetComponent<Button>();
                if (orderButton != null)
                {
                    orderButton.interactable = false;
                }
                
                Debug.Log($"EndGameManager: Создан заказ '{recipe?.recipeName}' с штампом {(isCorrectlyAssembled ? "успеха" : "неудачи")}");
            }
        }

        // TODO

    }
    
    // Скрыть экран окончания игры
    public void HideEndGameScreen()
    {
        Debug.Log("EndGameManager: Скрываем экран окончания игры");
        gameObject.SetActive(false);
    }
    
    // Публичные методы для получения данных
    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
    }
    
    public void SetOrderSpawn(Transform spawn)
    {
        orderSpawn = spawn;
    }
    
    // Скрыть палец (вызывается из Hands.cs)
    public void HideFinger(bool isLeftHand, int fingerIndex)
    {
        Debug.Log($"EndGameManager: Получено уведомление о скрытом пальце. Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
        
        // Уменьшаем счетчик проваленных заказов при отрезании пальца
        DecrementFailedOrdersCount();
        
        // Уведомляем GameManager о скрытом пальце
        if (gameManager != null)
        {
            gameManager.HideFinger(isLeftHand, fingerIndex);
            Debug.Log($"EndGameManager: Уведомлен GameManager о скрытом пальце. Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
        }
        else
        {
            Debug.LogWarning("EndGameManager: GameManager не найден!");
        }
        
        // Здесь можно добавить дополнительную логику для EndGameManager
        // Например, обновить UI, показать анимацию, и т.д.
        
        // Проверяем количество оставшихся пальцев после отрезания
        int remainingFingers = GetRemainingFingersCount();
        
        if (remainingFingers <= 0)
        {
            Debug.LogWarning("EndGameManager: Не осталось пальцев! Показываем Game Over экран");
            ShowGameOverScreen(false); // false = поражение (потеряны все пальцы)
        }
        
        Debug.Log($"EndGameManager: Обработано скрытие пальца. Рука: {(isLeftHand ? "левая" : "правая")}, палец: {fingerIndex}");
    }
    
    // Перейти на следующий уровень
    public void NextLevel()
    {
        if (gameManager != null)
        {
            Debug.Log($"EndGameManager: Переходим на следующий уровень. Текущий уровень: {currentLevel}, Максимум: {maxLevels}");
            
            // Останавливаем нож перед переходом на следующий уровень
            StopEndGameKnife();
            
            // Проверяем количество оставшихся пальцев
            int remainingFingers = GetRemainingFingersCount();
            
            if (remainingFingers > 0)
            {
                // Проверяем, остались ли еще уровни
                if (currentLevel < maxLevels)
                {
                    Debug.Log($"EndGameManager: Осталось пальцев: {remainingFingers}. Переходим на следующий уровень");
                    
                    // Воспроизводим звук завершения уровня
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayLevelComplete();
                    }
                    
                    // Увеличиваем номер уровня
                    currentLevel++;
                    
                    // Переходим на следующий уровень через GameManager
                    gameManager.NextLevel();
                    
                    // Скрываем экран окончания игры
                    HideEndGameScreen();
                }
                else
                {
                    Debug.Log($"EndGameManager: Достигнут максимальный уровень ({maxLevels})! Показываем экран победы");
                    
                    // Воспроизводим звук победы
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayGameWin();
                    }
                    
                    // Показываем GameOverScreen с победой
                    ShowGameOverScreen(true); // true = победа (прошли все уровни)
                }
            }
            else
            {
                Debug.LogWarning("EndGameManager: Нет оставшихся пальцев! Нельзя перейти на следующий уровень");
            }
        }
        else
        {
            Debug.LogError("EndGameManager: GameManager не найден! Не удалось перейти на следующий уровень");
        }
    }
    
    // Методы для управления счетчиком проваленных заказов
    
    // Сбросить счетчик проваленных заказов
    public void ResetFailedOrdersCount()
    {
        failedOrdersCount = 0;
        Debug.Log("EndGameManager: Счетчик проваленных заказов сброшен");
        UpdateEndTexts();
    }
    
    // Увеличить счетчик проваленных заказов
    public void IncrementFailedOrdersCount()
    {
        failedOrdersCount++;
        Debug.Log($"EndGameManager: Провален заказ. Всего провалено: {failedOrdersCount}");
        UpdateEndTexts();
    }
    
    // Уменьшить счетчик проваленных заказов на 1
    public void DecrementFailedOrdersCount()
    {
        if (failedOrdersCount > 0)
        {
            failedOrdersCount--;
            Debug.Log($"EndGameManager: Отменен провал заказа. Осталось провалено: {failedOrdersCount}");
            UpdateEndTexts();
        }
        else
        {
            Debug.LogWarning("EndGameManager: Попытка уменьшить счетчик проваленных заказов ниже 0!");
        }
    }
    
    // Получить количество проваленных заказов
    public int GetFailedOrdersCount()
    {
        return failedOrdersCount;
    }
    
    // Установить количество проваленных заказов
    public void SetFailedOrdersCount(int count)
    {
        failedOrdersCount = Mathf.Max(0, count); // Не меньше 0
        Debug.Log($"EndGameManager: Установлено количество проваленных заказов: {failedOrdersCount}");
        UpdateEndTexts();
    }
    
    // Получить информацию о проваленных заказах
    public string GetFailedOrdersInfo()
    {
        return $"Проваленных заказов: {failedOrdersCount}";
    }
    
    // Обновить тексты окончания игры
    private void UpdateEndTexts()
    {
        if (endText1 == null || endText2 == null)
        {
            Debug.LogWarning("EndGameManager: EndText1 или EndText2 не назначены!");
            return;
        }
        
        if (failedOrdersCount > 0)
        {
            // Если есть проваленные заказы
            endText1.text = "Try harder!";
            endText2.text = $"Cut fingers: {failedOrdersCount}";
            Debug.Log($"EndGameManager: Обновлены тексты для проваленных заказов. Количество: {failedOrdersCount}");
        }
        else
        {
            // Если нет проваленных заказов
            endText1.text = "Click here";
            endText2.text = "to continue";
            Debug.Log("EndGameManager: Обновлены тексты для успешного прохождения");
        }
        
        // Обновляем интерактивность кнопки NextLevel
        UpdateNextLevelButtonInteractivity();
    }
    
    // Публичный метод для принудительного обновления текстов
    public void ForceUpdateEndTexts()
    {
        UpdateEndTexts();
    }
    
    // Инициализировать кнопку NextLevel
    private void InitializeNextLevelButton()
    {
        if (nextLevelButton == null)
        {
            Debug.LogWarning("EndGameManager: NextLevelButton не назначена!");
            return;
        }
        
        // По умолчанию делаем кнопку неинтерактивной
        nextLevelButton.interactable = false;
        
        // Добавляем обработчик клика
        nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
        
        Debug.Log("EndGameManager: Кнопка NextLevel инициализирована");
    }
    
    // Обработчик клика по кнопке NextLevel
    private void OnNextLevelButtonClick()
    {
        Debug.Log("EndGameManager: Клик по кнопке NextLevel");
        NextLevel();
    }
    
    // Обновить интерактивность кнопки NextLevel
    private void UpdateNextLevelButtonInteractivity()
    {
        if (nextLevelButton == null)
        {
            Debug.LogWarning("EndGameManager: NextLevelButton не назначена!");
            return;
        }
        
        bool shouldBeInteractive = (failedOrdersCount == 0);
        nextLevelButton.interactable = shouldBeInteractive;
        
        Debug.Log($"EndGameManager: Кнопка NextLevel {(shouldBeInteractive ? "активирована" : "деактивирована")}. Проваленных заказов: {failedOrdersCount}");
    }
    
    // Публичный метод для принудительного обновления интерактивности кнопки
    public void ForceUpdateNextLevelButton()
    {
        UpdateNextLevelButtonInteractivity();
    }
    
    // Инициализировать нож в EndGame
    private void InitializeEndGameKnife()
    {
        if (endGameKnife == null)
        {
            Debug.LogWarning("EndGameManager: EndGameKnife не назначен!");
            return;
        }
        
        Debug.Log("EndGameManager: Нож в EndGame инициализирован");
    }
    
    // Остановить нож в EndGame
    private void StopEndGameKnife()
    {
        if (endGameKnife != null)
        {
            endGameKnife.StopFollowing();
            Debug.Log("EndGameManager: Нож в EndGame остановлен");
        }
        else
        {
            Debug.LogWarning("EndGameManager: EndGameKnife не найден!");
        }
    }
    
    // Получить нож в EndGame
    public Knife GetEndGameKnife()
    {
        return endGameKnife;
    }
    
    // Установить нож в EndGame
    public void SetEndGameKnife(Knife knife)
    {
        endGameKnife = knife;
        Debug.Log("EndGameManager: Нож в EndGame установлен");
    }
    
    // Получить количество оставшихся пальцев
    private int GetRemainingFingersCount()
    {
        if (gameManager != null && gameManager.handsGame != null)
        {
            // Получаем общее количество пальцев (10) и вычитаем количество скрытых
            int totalFingers = 10;
            int hiddenFingers = gameManager.handsGame.GetHiddenFingersCount();
            int remainingFingers = totalFingers - hiddenFingers;
            
            Debug.Log($"EndGameManager: Всего пальцев: {totalFingers}, скрыто: {hiddenFingers}, осталось: {remainingFingers}");
            return remainingFingers;
        }
        else
        {
            Debug.LogWarning("EndGameManager: GameManager или HandsGame не найден! Не удалось получить количество пальцев");
            return 0; // Возвращаем 0 если не можем получить информацию
        }
    }
    
    // Инициализировать GameOverScreen
    private void InitializeGameOverScreen()
    {
        if (gameOverScreen == null)
        {
            Debug.LogWarning("EndGameManager: GameOverScreen не назначен!");
            return;
        }
        
        // Скрываем GameOverScreen по умолчанию
        gameOverScreen.SetActive(false);
        
        Debug.Log("EndGameManager: GameOverScreen инициализирован и скрыт");
    }
    
    // Показать GameOverScreen
    public void ShowGameOverScreen(bool isWin)
    {
        if (gameOverScreen == null)
        {
            Debug.LogError("EndGameManager: GameOverScreen не назначен! Не удалось показать экран Game Over");
            return;
        }
        
        // Воспроизводим соответствующий звук
        if (AudioManager.Instance != null)
        {
            if (isWin)
            {
                AudioManager.Instance.PlayGameWin();
            }
            else
            {
                AudioManager.Instance.PlayGameLose();
            }
        }
        
        // Устанавливаем сообщение в зависимости от результата
        SetGameOverMessage(isWin);
        
        gameOverScreen.SetActive(true);
        Debug.Log($"EndGameManager: Показан экран Game Over. Результат: {(isWin ? "Победа" : "Поражение")}");
    }
    
    // Скрыть GameOverScreen
    public void HideGameOverScreen()
    {
        if (gameOverScreen == null)
        {
            Debug.LogWarning("EndGameManager: GameOverScreen не назначен!");
            return;
        }
        
        gameOverScreen.SetActive(false);
        Debug.Log("EndGameManager: Скрыт экран Game Over");
    }
    
    // Получить GameOverScreen
    public GameObject GetGameOverScreen()
    {
        return gameOverScreen;
    }
    
    // Установить GameOverScreen
    public void SetGameOverScreen(GameObject screen)
    {
        gameOverScreen = screen;
        Debug.Log("EndGameManager: GameOverScreen установлен");
    }
    
    // Установить сообщение Game Over
    private void SetGameOverMessage(bool isWin)
    {
        if (gameOverMessage == null)
        {
            Debug.LogWarning("EndGameManager: GameOverMessage не назначен!");
            return;
        }
        
        if (isWin)
        {
            gameOverMessage.text = "Bravo! You survived!";
            Debug.Log("EndGameManager: Установлено сообщение победы");
        }
        else
        {
            gameOverMessage.text = "You lost all fingers!";
            Debug.Log("EndGameManager: Установлено сообщение поражения");
        }
    }
    
    // Публичный метод для принудительного обновления сообщения
    public void ForceUpdateGameOverMessage(bool isWin)
    {
        SetGameOverMessage(isWin);
    }
    
    // Методы для управления уровнями
    
    // Получить текущий уровень
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    // Установить текущий уровень
    public void SetCurrentLevel(int level)
    {
        currentLevel = Mathf.Max(1, level);
        Debug.Log($"EndGameManager: Установлен уровень {currentLevel}");
    }
    
    // Получить максимальное количество уровней
    public int GetMaxLevels()
    {
        return maxLevels;
    }
    
    // Установить максимальное количество уровней
    public void SetMaxLevels(int maxLevel)
    {
        maxLevels = Mathf.Max(1, maxLevel);
        Debug.Log($"EndGameManager: Установлено максимальное количество уровней: {maxLevels}");
    }
    
    // Проверить, остались ли еще уровни
    public bool HasMoreLevels()
    {
        return currentLevel < maxLevels;
    }
    
    // Получить информацию об уровнях
    public string GetLevelInfo()
    {
        return $"Уровень {currentLevel} из {maxLevels}";
    }
}
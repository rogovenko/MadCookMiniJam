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
        
        // Скрываем UI по умолчанию
        // TODO вернуть
        // gameObject.SetActive(false);
    }
    
    // Показать экран окончания игры
    public void ShowEndGameScreen()
    {
        Debug.Log("EndGameManager: Показываем экран окончания игры");
        gameObject.SetActive(true);
        
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
    
    // Перейти на следующий уровень
    public void NextLevel()
    {
        if (gameManager != null)
        {
            Debug.Log("EndGameManager: Переходим на следующий уровень");
            
            // Переходим на следующий уровень через GameManager
            gameManager.NextLevel();
            
            // Скрываем экран окончания игры
            HideEndGameScreen();
        }
        else
        {
            Debug.LogError("EndGameManager: GameManager не найден! Не удалось перейти на следующий уровень");
        }
    }
}
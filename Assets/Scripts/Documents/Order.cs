using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class StickerSlot
{
    [Tooltip("Image компонент для наклейки")]
    public Image stickerImage;
    
    [Tooltip("Тип овоща в этом слоте (null если слот пустой)")]
    public CharacterType? vegetableType;
    
    [Tooltip("Активен ли слот (показывается ли)")]
    public bool isActive = false;
}

public class Order : Paper
{
    [Header("Ссылки")]
    [Tooltip("Ссылка на GameManager")]
    [SerializeField] private GameManager gameManager;
    
    [Header("Наклейки заказа")]
    [Tooltip("Максимальное количество наклеек")]
    [SerializeField] private int maxStickers = 6;
    
    [Tooltip("Слоты для наклеек")]
    [SerializeField] private List<StickerSlot> stickerSlots = new List<StickerSlot>();
    
    [Header("Текстуры наклеек")]
    [Tooltip("Текстура наклейки помидора")]
    [SerializeField] private Sprite tomatoSticker;
    [Tooltip("Текстура наклейки лука")]
    [SerializeField] private Sprite onionSticker;
    [Tooltip("Текстура наклейки картошки")]
    [SerializeField] private Sprite potatoSticker;
    [Tooltip("Текстура наклейки огурца")]
    [SerializeField] private Sprite cucumberSticker;
    [Tooltip("Текстура наклейки баклажана")]
    [SerializeField] private Sprite eggplantSticker;
    [Tooltip("Текстура наклейки моркови")]
    [SerializeField] private Sprite carrotSticker;
    
    [Header("Добавленные персонажи")]
    [Tooltip("Список CharInfo добавленных в заказ персонажей")]
    [SerializeField] private List<CharInfo> addedChars = new List<CharInfo>();
    
    [Header("Рецепт заказа")]
    [Tooltip("Рецепт для этого заказа")]
    [SerializeField] private RecipeData orderRecipe;
    
    [Header("Штампы результата")]
    [Tooltip("Штамп успеха (зеленая галочка)")]
    [SerializeField] private GameObject successStamp;
    
    [Tooltip("Штамп неудачи (красный крестик)")]
    [SerializeField] private GameObject failStamp;
    
    private Dictionary<CharacterType, Sprite> stickerSprites = new Dictionary<CharacterType, Sprite>();
    
    protected override void Start()
    {
        base.Start();
        InitializeStickerSprites();
        InitializeStickerSlots();
        
        // Если GameManager не назначен, пытаемся найти его
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        // Автоматически настраиваем Button компонент
        SetupButtonComponent();
    }
    
    private void InitializeStickerSprites()
    {
        stickerSprites[CharacterType.Tomato] = tomatoSticker;
        stickerSprites[CharacterType.Onion] = onionSticker;
        stickerSprites[CharacterType.Potato] = potatoSticker;
        stickerSprites[CharacterType.Cucumber] = cucumberSticker;
        stickerSprites[CharacterType.Eggplant] = eggplantSticker;
        stickerSprites[CharacterType.Carrot] = carrotSticker;
    }
    
    private void InitializeStickerSlots()
    {
        // Инициализируем слоты если их нет
        if (stickerSlots.Count == 0)
        {
            // Ищем все Image компоненты с тегом "Sticker" или именем содержащим "Sticker"
            Image[] allImages = GetComponentsInChildren<Image>();
            int stickerCount = 0;
            
            foreach (Image img in allImages)
            {
                if (img.name.ToLower().Contains("sticker") || 
                    img.CompareTag("Sticker") || 
                    stickerCount < maxStickers)
                {
                    StickerSlot slot = new StickerSlot
                    {
                        stickerImage = img,
                        vegetableType = null,
                        isActive = false
                    };
                    stickerSlots.Add(slot);
                    
                    // Скрываем наклейку по умолчанию
                    if (img != null)
                    {
                        img.gameObject.SetActive(false);
                    }
                    
                    stickerCount++;
                    if (stickerCount >= maxStickers) break;
                }
            }
        }
        else
        {
            // Скрываем все существующие слоты
            foreach (StickerSlot slot in stickerSlots)
            {
                if (slot.stickerImage != null)
                {
                    slot.stickerImage.gameObject.SetActive(false);
                    slot.vegetableType = null;
                    slot.isActive = false;
                }
            }
        }
    }
    
    // Добавить наклейку
    public void AddSticker(CharacterType vegetableType)
    {
        // Находим первый пустой слот
        StickerSlot emptySlot = FindEmptyStickerSlot();
        
        if (emptySlot != null && stickerSprites.ContainsKey(vegetableType))
        {
            // Активируем слот
            emptySlot.isActive = true;
            emptySlot.vegetableType = vegetableType;
            
            // Показываем наклейку
            if (emptySlot.stickerImage != null)
            {
                emptySlot.stickerImage.gameObject.SetActive(true);
                emptySlot.stickerImage.sprite = stickerSprites[vegetableType];
            }
        }
        else
        {
            Debug.LogWarning($"Order: Не удалось добавить наклейку {vegetableType}. Все слоты заполнены или текстура не найдена!");
        }
    }
    
    // Найти пустой слот для наклейки
    private StickerSlot FindEmptyStickerSlot()
    {
        foreach (StickerSlot slot in stickerSlots)
        {
            if (!slot.isActive)
            {
                return slot;
            }
        }
        return null;
    }
    
    // Очистить все наклейки
    public void ClearAllStickers()
    {
        foreach (StickerSlot slot in stickerSlots)
        {
            slot.isActive = false;
            slot.vegetableType = null;
            if (slot.stickerImage != null)
            {
                slot.stickerImage.gameObject.SetActive(false);
            }
        }
        
        // Очищаем список добавленных персонажей
        ClearAddedCharacters();
    }
    
    // Получить количество активных наклеек
    public int GetActiveStickerCount()
    {
        int count = 0;
        foreach (StickerSlot slot in stickerSlots)
        {
            if (slot.isActive)
            {
                count++;
            }
        }
        return count;
    }
    
    // Получить список типов овощей в заказе
    public List<CharacterType> GetOrderedVegetables()
    {
        List<CharacterType> vegetables = new List<CharacterType>();
        foreach (StickerSlot slot in stickerSlots)
        {
            if (slot.isActive && slot.vegetableType.HasValue)
            {
                vegetables.Add(slot.vegetableType.Value);
            }
        }
        return vegetables;
    }
    
    // Проверить, заполнен ли заказ
    public bool IsOrderFull()
    {
        return GetActiveStickerCount() >= maxStickers;
    }
    
    // Проверить, пустой ли заказ
    public bool IsOrderEmpty()
    {
        return GetActiveStickerCount() == 0;
    }
    
    // Получить количество свободных слотов
    public int GetAvailableSlots()
    {
        return maxStickers - GetActiveStickerCount();
    }
    
    // Добавить персонажа в список добавленных
    public void AddCharacter(CharInfo charInfo)
    {
        if (charInfo != null)
        {
            addedChars.Add(charInfo);
            Debug.Log($"Order: Добавлен персонаж {charInfo.name} ({charInfo.GetVarietyDisplayName()} из {charInfo.GetOriginDisplayName()}) в заказ");
        }
    }
    
    // Получить список добавленных персонажей
    public List<CharInfo> GetAddedCharacters()
    {
        return new List<CharInfo>(addedChars);
    }
    
    // Очистить список добавленных персонажей
    public void ClearAddedCharacters()
    {
        addedChars.Clear();
        Debug.Log("Order: Список добавленных персонажей очищен");
    }
    
    // Получить количество добавленных персонажей
    public int GetAddedCharactersCount()
    {
        return addedChars.Count;
    }
    
    // Установить рецепт для заказа
    public void SetOrderRecipe(RecipeData recipe)
    {
        orderRecipe = recipe;
        Debug.Log($"Order: Установлен рецепт '{recipe.recipeName}' для заказа");
    }
    
    // Получить рецепт заказа
    public RecipeData GetOrderRecipe()
    {
        return orderRecipe;
    }
    
    // Проверить правильность собранных ингредиентов
    public bool CheckIngredientsCorrectness()
    {
        if (orderRecipe == null)
        {
            Debug.LogWarning("Order: Рецепт не установлен для заказа!");
            return false;
        }
        
        // Подсчитываем количество каждого типа ингредиентов в добавленных персонажах
        Dictionary<CharacterType, int> addedIngredients = new Dictionary<CharacterType, int>();
        
        foreach (CharInfo charInfo in addedChars)
        {
            if (charInfo != null)
            {
                CharacterType vegType = charInfo.characterType;
                if (addedIngredients.ContainsKey(vegType))
                {
                    addedIngredients[vegType]++;
                }
                else
                {
                    addedIngredients[vegType] = 1;
                }
            }
        }
        
        // Проверяем, соответствует ли количество добавленных ингредиентов рецепту
        foreach (RecipeIngredient requiredIngredient in orderRecipe.ingredients)
        {
            CharacterType requiredType = requiredIngredient.ingredientType;
            int requiredAmount = requiredIngredient.amount;
            
            // Проверяем, есть ли нужный тип ингредиента
            if (!addedIngredients.ContainsKey(requiredType))
            {
                Debug.Log($"Order: Отсутствует ингредиент {requiredType} (требуется: {requiredAmount})");
                return false;
            }
            
            // Проверяем количество
            int addedAmount = addedIngredients[requiredType];
            if (addedAmount != requiredAmount)
            {
                Debug.Log($"Order: Неправильное количество {requiredType} (добавлено: {addedAmount}, требуется: {requiredAmount})");
                return false;
            }
        }
        
        // Проверяем, нет ли лишних ингредиентов
        foreach (var kvp in addedIngredients)
        {
            CharacterType addedType = kvp.Key;
            int addedAmount = kvp.Value;
            
            // Ищем этот тип в рецепте
            bool foundInRecipe = false;
            foreach (RecipeIngredient requiredIngredient in orderRecipe.ingredients)
            {
                if (requiredIngredient.ingredientType == addedType)
                {
                    foundInRecipe = true;
                    break;
                }
            }
            
            if (!foundInRecipe)
            {
                Debug.Log($"Order: Лишний ингредиент {addedType} (добавлено: {addedAmount})");
                return false;
            }
        }
        
        Debug.Log("Order: Все ингредиенты собраны правильно!");
        return true;
    }
    
    // Получить детальную информацию о проверке ингредиентов
    public string GetIngredientsCheckDetails()
    {
        if (orderRecipe == null)
        {
            return "Рецепт не установлен";
        }
        
        // Подсчитываем количество каждого типа ингредиентов в добавленных персонажах
        Dictionary<CharacterType, int> addedIngredients = new Dictionary<CharacterType, int>();
        
        foreach (CharInfo charInfo in addedChars)
        {
            if (charInfo != null)
            {
                CharacterType vegType = charInfo.characterType;
                if (addedIngredients.ContainsKey(vegType))
                {
                    addedIngredients[vegType]++;
                }
                else
                {
                    addedIngredients[vegType] = 1;
                }
            }
        }
        
        string details = $"Рецепт: {orderRecipe.recipeName}\n";
        details += $"Требуется: {orderRecipe.GetIngredientsList()}\n";
        details += $"Добавлено: ";
        
        if (addedIngredients.Count == 0)
        {
            details += "ничего";
        }
        else
        {
            int index = 0;
            foreach (var kvp in addedIngredients)
            {
                details += $"{kvp.Key} x{kvp.Value}";
                if (index < addedIngredients.Count - 1)
                {
                    details += ", ";
                }
                index++;
            }
        }
        
        return details;
    }
    
    // Установить текст заказа
    public void SetOrderText(string orderText)
    {   
        if (paperText != null)
        {
            paperText.text = orderText;
        }
        else
        {
            Debug.LogWarning("Order: Не найден paperText компонент для отображения текста заказа!");
        }
    }
    
    // Обработчик клика по заказу
    public void OnOrderClick()
    {
        if (gameManager == null)
        {
            Debug.LogError("Order: GameManager не найден!");
            return;
        }
        
        // Проверяем, включен ли режим выбора заказа
        if (gameManager.IsChooseOrderMode())
        {
            // Проверяем, есть ли место для новых стикеров
            if (IsOrderFull())
            {
                Debug.LogWarning("Order: Заказ уже заполнен! Максимум 6 стикеров.");
                return; // Выходим без изменений
            }
            
            // Получаем текущего персонажа
            GameObject currentCharacter = gameManager.GetCurrentCharacter();
            if (currentCharacter != null)
            {
                // Получаем компонент персонажа
                Character characterComponent = currentCharacter.GetComponent<Character>();
                if (characterComponent != null)
                {
                    CharacterType characterType = characterComponent.GetCharacterType();
                    
                    // Получаем CharInfo персонажа
                    CharInfo charInfo = characterComponent.GetCharacterInfo();
                    
                    // Добавляем наклейку в заказ
                    AddSticker(characterType);
                    
                    // Добавляем персонажа в список добавленных
                    AddCharacter(charInfo);
                    
                    // Уничтожаем текущего персонажа
                    gameManager.DestroyCurrentCharacter();
                    
                    // Включаем drag'n'drop обратно
                    gameManager.EnableDragDrop();
                    
                    // Отключаем режим выбора заказа
                    gameManager.SetChooseOrderMode(false);
                }
                else
                {
                    Debug.LogError("Order: На персонаже отсутствует компонент Character!");
                }
            }
            else
            {
                Debug.LogWarning("Order: Нет текущего персонажа!");
            }
        }
        else
        {
            Debug.Log("Order: Режим выбора заказа не активен");
        }
    }
    
    // Показать штамп результата
    public void ShowResultStamp(bool isSuccess)
    {
        // Сначала скрываем оба штампа
        if (successStamp != null)
        {
            successStamp.SetActive(false);
        }
        
        if (failStamp != null)
        {
            failStamp.SetActive(false);
        }
        
        // Показываем нужный штамп
        if (isSuccess)
        {
            if (successStamp != null)
            {
                successStamp.SetActive(true);
                Debug.Log("Order: Показан штамп успеха");
            }
            else
            {
                Debug.LogWarning("Order: SuccessStamp не назначен в инспекторе!");
            }
        }
        else
        {
            if (failStamp != null)
            {
                failStamp.SetActive(true);
                Debug.Log("Order: Показан штамп неудачи");
            }
            else
            {
                Debug.LogWarning("Order: FailStamp не назначен в инспекторе!");
            }
        }
    }
    
    // Скрыть все штампы
    public void HideResultStamps()
    {
        if (successStamp != null)
        {
            successStamp.SetActive(false);
        }
        
        if (failStamp != null)
        {
            failStamp.SetActive(false);
        }
        
        Debug.Log("Order: Все штампы скрыты");
    }
    
    private void SetupButtonComponent()
    {
        // Получаем или добавляем Button компонент
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        
        // Настраиваем цвета кнопки - отключаем все изменения цвета и альфы
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        colors.disabledColor = Color.white;
        colors.fadeDuration = 0f; // Отключаем анимацию перехода
        button.colors = colors;
        
        // Очищаем существующие события
        button.onClick.RemoveAllListeners();
        
        // Добавляем наш метод в события клика
        button.onClick.AddListener(OnOrderClick);
    }
} 
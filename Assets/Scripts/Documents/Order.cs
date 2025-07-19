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
            
            Debug.Log($"Order: Добавлена наклейка {vegetableType} в слот {stickerSlots.IndexOf(emptySlot)}");
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
        Debug.Log("Order: Все наклейки очищены");
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
    
    // Установить текст заказа
    public void SetOrderText(string orderText)
    {
        // Ищем TextMeshPro компонент для отображения текста заказа
        TextMeshProUGUI orderTextComponent = GetComponentInChildren<TextMeshProUGUI>();
        
        if (orderTextComponent != null)
        {
            orderTextComponent.text = orderText;
            Debug.Log($"Order: Установлен текст заказа: {orderText}");
        }
        else
        {
            Debug.LogWarning("Order: Не найден TextMeshProUGUI компонент для отображения текста заказа!");
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
                // Получаем тип персонажа
                Character characterComponent = currentCharacter.GetComponent<Character>();
                if (characterComponent != null)
                {
                    CharacterType characterType = characterComponent.GetCharacterType();
                    
                    // Добавляем наклейку в заказ
                    AddSticker(characterType);
                    
                    Debug.Log($"Order: Добавлен персонаж {characterType} в заказ");
                    
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
    
    private void SetupButtonComponent()
    {
        // Получаем или добавляем Button компонент
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        
        // Очищаем существующие события
        button.onClick.RemoveAllListeners();
        
        // Добавляем наш метод в события клика
        button.onClick.AddListener(OnOrderClick);
        
        Debug.Log("Order: Button компонент настроен автоматически");
    }
} 
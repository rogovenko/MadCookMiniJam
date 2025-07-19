using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Настройки персонажа")]
    [Tooltip("Текущий тип персонажа")]
    [SerializeField] private CharacterType characterType = CharacterType.Tomato;
    
    [Tooltip("Форма персонажа")]
    [SerializeField] private CharacterShape characterShape = CharacterShape.Round;
    
    [Tooltip("Кастомное название персонажа")]
    [SerializeField] private string customCharacterName = "";

    [Header("Компоненты")]
    [Tooltip("Image компонент для отображения тела персонажа")]
    [SerializeField] private Image bodyImage;
    
    [Tooltip("Image компонент для отображения глаз")]
    [SerializeField] private Image eyesImage;
    
    [Tooltip("Image компонент для отображения одежды")]
    [SerializeField] private Image clothesImage;

    void Start()
    {
        // Если Image компоненты не назначены, пытаемся найти их на этом объекте
        if (bodyImage == null)
        {
            bodyImage = GetComponent<Image>();
        }
        
        // Глаза и одежда должны быть дочерними объектами
        if (eyesImage == null)
        {
            eyesImage = transform.Find("Eyes")?.GetComponent<Image>();
        }
        
        if (clothesImage == null)
        {
            clothesImage = transform.Find("Clothes")?.GetComponent<Image>();
        }
    }

    // Установить тип персонажа
    public void SetCharacterType(CharacterType newType)
    {
        characterType = newType;
    }
    
    // Установить форму персонажа
    public void SetCharacterShape(CharacterShape newShape)
    {
        characterShape = newShape;
    }
    
    // Установить спрайт тела
    public void SetBodySprite(Sprite sprite)
    {
        if (bodyImage == null)
        {
            Debug.LogError($"Character: Body Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            bodyImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт тела для {characterType} не назначен!");
        }
    }
    
    // Установить спрайт глаз
    public void SetEyesSprite(Sprite sprite)
    {
        if (eyesImage == null)
        {
            Debug.LogError($"Character: Eyes Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            eyesImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт глаз для {characterType} не назначен!");
        }
    }
    
    // Установить спрайт одежды
    public void SetClothesSprite(Sprite sprite)
    {
        if (clothesImage == null)
        {
            Debug.LogError($"Character: Clothes Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            clothesImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт одежды для {characterType} не назначен!");
        }
    }

    // Получить текущий тип персонажа
    public CharacterType GetCharacterType()
    {
        return characterType;
    }
    
    // Получить форму персонажа
    public CharacterShape GetCharacterShape()
    {
        return characterShape;
    }

    // Получить имя персонажа
    public string GetCharacterName()
    {
        if (!string.IsNullOrEmpty(customCharacterName))
        {
            return customCharacterName;
        }
        return characterType.ToString();
    }

    // Установить кастомное имя
    public void SetCustomName(string name)
    {
        customCharacterName = name;
    }
} 
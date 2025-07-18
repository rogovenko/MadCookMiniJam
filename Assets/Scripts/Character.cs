using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Настройки персонажа")]
    [Tooltip("Текущий тип персонажа")]
    [SerializeField] private CharacterType characterType = CharacterType.Carrot;
    
    [Tooltip("Кастомное название персонажа")]
    [SerializeField] private string customCharacterName = "";

    [Header("Компоненты")]
    [Tooltip("Image компонент для отображения спрайта персонажа")]
    [SerializeField] private Image characterImage;

    void Start()
    {
        // Если Image не назначен, пытаемся найти его на этом объекте
        if (characterImage == null)
        {
            characterImage = GetComponent<Image>();
        }
    }

    // Установить тип персонажа
    public void SetCharacterType(CharacterType newType)
    {
        characterType = newType;
    }

    // Установить спрайт персонажа
    public void SetCharacterSprite(Sprite sprite)
    {
        if (characterImage == null)
        {
            Debug.LogError($"Character: Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            characterImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт для {characterType} не назначен!");
        }
    }

    // Получить текущий тип персонажа
    public CharacterType GetCharacterType()
    {
        return characterType;
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
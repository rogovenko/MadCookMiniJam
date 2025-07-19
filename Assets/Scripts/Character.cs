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
    
    [Tooltip("Текущий дефект персонажа")]
    [SerializeField] private CharacterDefect currentDefect = CharacterDefect.None;

    [Header("Компоненты")]
    [Tooltip("Image компонент для отображения тела персонажа")]
    [SerializeField] private Image bodyImage;
    
    [Tooltip("Image компонент для отображения глаз")]
    [SerializeField] private Image eyesImage;
    
    [Tooltip("Image компонент для отображения одежды")]
    [SerializeField] private Image clothesImage;
    
    [Tooltip("Image компонент для отображения дефектов")]
    [SerializeField] private Image defectImage;

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
        
        if (defectImage == null)
        {
            defectImage = transform.Find("Defect")?.GetComponent<Image>();
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
    
    // Применить дефект к персонажу
    public void ApplyDefect(CharacterDefect defect)
    {
        ApplyDefect(defect, characterShape);
    }
    
    // Применить дефект к персонажу с указанием формы
    public void ApplyDefect(CharacterDefect defect, CharacterShape shape)
    {
        currentDefect = defect;
        
        if (defectImage == null)
        {
            Debug.LogError($"Character: Defect Image компонент не найден на {gameObject.name}!");
            return;
        }
        
        if (defect == CharacterDefect.None)
        {
            // Скрываем компонент дефекта
            defectImage.gameObject.SetActive(false);
            Debug.Log($"Character: Дефект убран с {gameObject.name}");
        }
        else
        {
            // Показываем компонент дефекта
            defectImage.gameObject.SetActive(true);
            
            // Получаем спрайт дефекта из CharManager с учетом формы
            CharManager charManager = FindObjectOfType<CharManager>();
            if (charManager != null)
            {
                Sprite defectSprite = charManager.GetDefectSprite(defect, shape);
                if (defectSprite != null)
                {
                    defectImage.sprite = defectSprite;
                    Debug.Log($"Character: Применен дефект {defect} (форма: {shape}) к {gameObject.name}");
                }
                else
                {
                    Debug.LogWarning($"Character: Спрайт для дефекта {defect} (форма: {shape}) не найден!");
                    defectImage.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Character: CharManager не найден в сцене!");
                defectImage.gameObject.SetActive(false);
            }
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
    
    // Получить текущий дефект персонажа
    public CharacterDefect GetCurrentDefect()
    {
        return currentDefect;
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
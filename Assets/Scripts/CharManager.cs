using UnityEngine;
using System.Collections.Generic;

public enum CharacterType
{
    Tomato,
    Onion,
    Potato,
    Cucumber,
    Eggplant,
    Carrot
}

public enum CharacterShape
{
    Round,
    Long
}

[System.Serializable]
public class CharacterSpriteData
{
    [Tooltip("Тип персонажа")]
    public CharacterType characterType;
    
    [Tooltip("Форма персонажа")]
    public CharacterShape characterShape;
    
    [Tooltip("Спрайт тела персонажа")]
    public Sprite bodySprite;
    
    [Tooltip("Название персонажа")]
    public string characterName;
}

[System.Serializable]
public class EyesSpriteData
{
    [Tooltip("Форма персонажа")]
    public CharacterShape shape;
    
    [Tooltip("Список спрайтов глаз")]
    public List<Sprite> eyesSprites = new List<Sprite>();
}

[System.Serializable]
public class ClothesSpriteData
{
    [Tooltip("Форма персонажа")]
    public CharacterShape shape;
    
    [Tooltip("Список спрайтов одежды")]
    public List<Sprite> clothesSprites = new List<Sprite>();
}

public class CharManager : MonoBehaviour
{
    [Header("Настройки менеджера персонажей")]
    [Tooltip("Префаб персонажа")]
    public GameObject characterPrefab;
    
    [Tooltip("Родительский объект для создания персонажей")]
    public Transform characterParent;
    
    [Tooltip("Позиция для создания персонажей (если не указана, используется Vector2.zero)")]
    public Transform spawnPosition;
    
    [Header("Спрайты персонажей")]
    [Tooltip("Список всех спрайтов персонажей")]
    [SerializeField] private List<CharacterSpriteData> characterSprites = new List<CharacterSpriteData>();
    
    [Header("Спрайты глаз")]
    [Tooltip("Список спрайтов глаз для каждой формы")]
    [SerializeField] private List<EyesSpriteData> eyesSprites = new List<EyesSpriteData>();
    
    [Header("Спрайты одежды")]
    [Tooltip("Список спрайтов одежды для каждой формы")]
    [SerializeField] private List<ClothesSpriteData> clothesSprites = new List<ClothesSpriteData>();
    
    void Start()
    {
        InitializeDefaultCharacters();
    }
    
    void InitializeDefaultCharacters()
    {
        // Создаем базовые записи персонажей с их формами
        if (characterSprites.Count == 0)
        {
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Tomato, 
                characterShape = CharacterShape.Round,
                characterName = "Tomato"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Onion, 
                characterShape = CharacterShape.Round,
                characterName = "Onion"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Potato, 
                characterShape = CharacterShape.Round,
                characterName = "Potato"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Cucumber, 
                characterShape = CharacterShape.Long,
                characterName = "Cucumber"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Eggplant, 
                characterShape = CharacterShape.Long,
                characterName = "Eggplant"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Carrot, 
                characterShape = CharacterShape.Long,
                characterName = "Carrot"
            });
        }
    }
    
    // Создать персонажа указанного типа в позиции spawnPosition
    public GameObject CreateCharacter(CharacterType characterType)
    {
        if (characterPrefab == null)
        {
            Debug.LogError("CharManager: Не назначен префаб персонажа!");
            return null;
        }
        
        // Определяем позицию создания
        Vector2 position = Vector2.zero;
        if (spawnPosition != null)
        {
            if (spawnPosition is RectTransform rectTransform)
            {
                position = rectTransform.anchoredPosition;
            }
            else
            {
                position = spawnPosition.position;
            }
        }
        
        // Создаем объект персонажа
        GameObject characterObj = Instantiate(characterPrefab, characterParent);
        RectTransform rect = characterObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = position;
        }
        else
        {
            characterObj.transform.localPosition = position;
        }
        
        // Находим данные для этого типа персонажа
        CharacterSpriteData spriteData = characterSprites.Find(c => c.characterType == characterType);
        if (spriteData != null)
        {
            // Получаем компонент Character и устанавливаем тип и спрайты
            Character characterComponent = characterObj.GetComponent<Character>();
            if (characterComponent != null)
            {
                characterComponent.SetCharacterType(characterType);
                characterComponent.SetCharacterShape(spriteData.characterShape);
                
                // Устанавливаем спрайт тела
                if (spriteData.bodySprite != null)
                {
                    characterComponent.SetBodySprite(spriteData.bodySprite);
                }
                
                // Случайно выбираем глаза для данной формы
                Sprite randomEyes = GetRandomEyesForShape(spriteData.characterShape);
                if (randomEyes != null)
                {
                    characterComponent.SetEyesSprite(randomEyes);
                }
                
                // Случайно выбираем одежду для данной формы
                Sprite randomClothes = GetRandomClothesForShape(spriteData.characterShape);
                if (randomClothes != null)
                {
                    characterComponent.SetClothesSprite(randomClothes);
                }
            }
            else
            {
                Debug.LogError("CharManager: На префабе отсутствует компонент Character!");
            }
        }
        else
        {
            Debug.LogWarning($"CharManager: Данные для персонажа {characterType} не найдены!");
        }
        
        return characterObj;
    }
    
    // Получить случайные глаза для формы
    private Sprite GetRandomEyesForShape(CharacterShape shape)
    {
        EyesSpriteData eyesData = eyesSprites.Find(e => e.shape == shape);
        if (eyesData != null && eyesData.eyesSprites.Count > 0)
        {
            int randomIndex = Random.Range(0, eyesData.eyesSprites.Count);
            return eyesData.eyesSprites[randomIndex];
        }
        return null;
    }
    
    // Получить случайную одежду для формы
    private Sprite GetRandomClothesForShape(CharacterShape shape)
    {
        ClothesSpriteData clothesData = clothesSprites.Find(c => c.shape == shape);
        if (clothesData != null && clothesData.clothesSprites.Count > 0)
        {
            int randomIndex = Random.Range(0, clothesData.clothesSprites.Count);
            return clothesData.clothesSprites[randomIndex];
        }
        return null;
    }
    
    // Получить спрайт тела для типа персонажа
    public Sprite GetCharacterBodySprite(CharacterType characterType)
    {
        CharacterSpriteData spriteData = characterSprites.Find(c => c.characterType == characterType);
        return spriteData?.bodySprite;
    }
    
    // Получить список всех доступных типов персонажей
    public List<CharacterType> GetAllCharacterTypes()
    {
        List<CharacterType> types = new List<CharacterType>();
        foreach (CharacterSpriteData data in characterSprites)
        {
            types.Add(data.characterType);
        }
        return types;
    }
} 
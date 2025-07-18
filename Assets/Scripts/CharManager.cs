using UnityEngine;
using System.Collections.Generic;

public enum CharacterType
{
    Carrot,
    Eggplant,
    Tomato,
    Cucumber,
    Pepper
}

[System.Serializable]
public class CharacterSpriteData
{
    [Tooltip("Тип персонажа")]
    public CharacterType characterType;
    
    [Tooltip("Спрайт персонажа")]
    public Sprite characterSprite;
    
    [Tooltip("Название персонажа")]
    public string characterName;
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
    
    void Start()
    {
        InitializeDefaultCharacters();
    }
    
    void InitializeDefaultCharacters()
    {
        // Создаем базовые записи персонажей
        if (characterSprites.Count == 0)
        {
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Carrot, 
                characterName = "Carrot"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Eggplant, 
                characterName = "Eggplant"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Tomato, 
                characterName = "Tomato"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Cucumber, 
                characterName = "Cucumber"
            });
            characterSprites.Add(new CharacterSpriteData 
            { 
                characterType = CharacterType.Pepper, 
                characterName = "Pepper"
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
        
        // Находим спрайт для этого типа персонажа
        CharacterSpriteData spriteData = characterSprites.Find(c => c.characterType == characterType);
        if (spriteData != null && spriteData.characterSprite != null)
        {
            // Получаем компонент Character и устанавливаем тип и спрайт
            Character characterComponent = characterObj.GetComponent<Character>();
            if (characterComponent != null)
            {
                characterComponent.SetCharacterType(characterType);
                characterComponent.SetCharacterSprite(spriteData.characterSprite);
            }
            else
            {
                Debug.LogError("CharManager: На префабе отсутствует компонент Character!");
            }
        }
        else
        {
            Debug.LogWarning($"CharManager: Спрайт для персонажа {characterType} не найден!");
        }
        
        return characterObj;
    }
    
    // Получить спрайт для типа персонажа
    public Sprite GetCharacterSprite(CharacterType characterType)
    {
        CharacterSpriteData spriteData = characterSprites.Find(c => c.characterType == characterType);
        return spriteData?.characterSprite;
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
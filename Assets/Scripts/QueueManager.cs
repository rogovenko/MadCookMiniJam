using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] private GameObject charShadowPrefab;
    [SerializeField] private float horizontalOffset = 100f;
    
    [Header("Initial Queue")]
    [SerializeField] private List<CharacterType> initialQueue = new List<CharacterType>();
    
    [Header("Character Sprites")]
    [SerializeField] private Sprite tomatoSprite;
    [SerializeField] private Sprite onionSprite;
    [SerializeField] private Sprite potatoSprite;
    [SerializeField] private Sprite cucumberSprite;
    [SerializeField] private Sprite eggplantSprite;
    [SerializeField] private Sprite carrotSprite;
    
    private List<GameObject> queueObjects = new List<GameObject>();
    private Dictionary<CharacterType, Sprite> characterSprites = new Dictionary<CharacterType, Sprite>();
    
    // Событие, которое вызывается когда очередь готова
    public event Action OnQueueReady;
    
    private void Start()
    {
        InitializeCharacterSprites();
        CreateQueue(initialQueue);
    }
    
    private void InitializeCharacterSprites()
    {
        characterSprites[CharacterType.Tomato] = tomatoSprite;
        characterSprites[CharacterType.Onion] = onionSprite;
        characterSprites[CharacterType.Potato] = potatoSprite;
        characterSprites[CharacterType.Cucumber] = cucumberSprite;
        characterSprites[CharacterType.Eggplant] = eggplantSprite;
        characterSprites[CharacterType.Carrot] = carrotSprite;
    }
    
    public void CreateQueue(List<CharacterType> characterTypes)
    {
        ClearQueue();
        
        Vector2 currentPosition = (Vector2)transform.position;
        
        foreach (CharacterType characterType in characterTypes)
        {
            GameObject queueObject = Instantiate(charShadowPrefab, currentPosition, Quaternion.identity, transform);
            
            // Найти и установить спрайт
            Image image = queueObject.GetComponent<Image>();
            if (image != null && characterSprites.ContainsKey(characterType))
            {
                image.sprite = characterSprites[characterType];
            }
            
            queueObjects.Add(queueObject);
            currentPosition.x += horizontalOffset;
        }
        
        // Уведомляем что очередь готова
        OnQueueReady?.Invoke();
    }
    
    public void ClearQueue()
    {
        foreach (GameObject obj in queueObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        queueObjects.Clear();
    }
    
    public CharacterType RemoveFirstFromQueue()
    {
        if (queueObjects.Count > 0)
        {
            GameObject firstObject = queueObjects[0];
            CharacterType characterType = GetCharacterTypeFromQueueObject(firstObject);
            queueObjects.RemoveAt(0);
            
            if (firstObject != null)
            {
                Destroy(firstObject);
            }
            
            // Сдвинуть оставшиеся объекты влево
            RepositionQueue();
            
            return characterType;
        }
        return CharacterType.Tomato; // Возвращаем Tomato как fallback
    }
    
    private void RepositionQueue()
    {
        Vector2 currentPosition = (Vector2)transform.position;
        
        foreach (GameObject obj in queueObjects)
        {
            if (obj != null)
            {
                obj.transform.position = currentPosition;
                currentPosition.x += horizontalOffset;
            }
        }
    }
    
    // Получить тип первого персонажа в очереди
    public CharacterType GetFirstCharacterType()
    {
        if (queueObjects.Count > 0 && queueObjects[0] != null)
        {
            // Получаем тип персонажа из первого объекта в очереди
            return GetCharacterTypeFromQueueObject(queueObjects[0]);
        }
        return CharacterType.Tomato; // Возвращаем Tomato как fallback
    }
    
    // Получить тип персонажа из объекта очереди
    private CharacterType GetCharacterTypeFromQueueObject(GameObject queueObject)
    {
        // Проверяем спрайт объекта и определяем тип персонажа
        Image image = queueObject.GetComponent<Image>();
        if (image != null && image.sprite != null)
        {
            foreach (var kvp in characterSprites)
            {
                if (kvp.Value == image.sprite)
                {
                    return kvp.Key;
                }
            }
        }
        return CharacterType.Tomato; // Возвращаем Tomato как fallback
    }
    
    // Получить количество персонажей в очереди
    public int GetQueueCount()
    {
        return queueObjects.Count;
    }
} 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Settings")]
    [SerializeField] private GameObject charShadowPrefab;
    [SerializeField] private float horizontalOffset = 100f;
    

    
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
    
    // private void Start()
    // {
    //     InitializeCharacterSprites();
    //     // Очередь теперь создается из GameManager
    // }
    
    private void InitializeCharacterSprites()
    {
        characterSprites[CharacterType.Tomato] = tomatoSprite;
        characterSprites[CharacterType.Onion] = onionSprite;
        characterSprites[CharacterType.Potato] = potatoSprite;
        characterSprites[CharacterType.Cucumber] = cucumberSprite;
        characterSprites[CharacterType.Eggplant] = eggplantSprite;
        characterSprites[CharacterType.Carrot] = carrotSprite;
    }
    
    public void CreateQueue(List<CharInfo> characterInfos)
    {
        InitializeCharacterSprites();
        ClearQueue();
        
        Vector2 currentPosition = (Vector2)transform.position;
        
        foreach (CharInfo charInfo in characterInfos)
        {
            GameObject queueObject = Instantiate(charShadowPrefab, currentPosition, Quaternion.identity, transform);
            
            // Найти и установить спрайт
            Image image = queueObject.GetComponent<Image>();
            if (image != null && characterSprites.ContainsKey(charInfo.characterType))
            {
                image.sprite = characterSprites[charInfo.characterType];
            }
            
            // Сохраняем CharInfo в объекте очереди для дальнейшего использования
            QueueObjectData queueData = queueObject.GetComponent<QueueObjectData>();
            if (queueData == null)
            {
                queueData = queueObject.AddComponent<QueueObjectData>();
            }
            queueData.charInfo = charInfo;
            
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
    
    public CharInfo RemoveFirstFromQueue()
    {
        if (queueObjects.Count > 0)
        {
            GameObject firstObject = queueObjects[0];
            CharInfo charInfo = GetCharInfoFromQueueObject(firstObject);
            queueObjects.RemoveAt(0);
            
            if (firstObject != null)
            {
                Destroy(firstObject);
            }
            
            // Сдвинуть оставшиеся объекты влево
            RepositionQueue();
            
            return charInfo;
        }
        return new CharInfo(CharacterType.Tomato, false, false, System.DateTime.Now); // Возвращаем Tomato как fallback
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
    
    // Получить информацию о первом персонаже в очереди
    public CharInfo GetFirstCharacterInfo()
    {
        if (queueObjects.Count > 0 && queueObjects[0] != null)
        {
            // Получаем информацию о персонаже из первого объекта в очереди
            return GetCharInfoFromQueueObject(queueObjects[0]);
        }
        return new CharInfo(CharacterType.Tomato, false, false, System.DateTime.Now); // Возвращаем Tomato как fallback
    }
    
    // Получить информацию о персонаже из объекта очереди
    private CharInfo GetCharInfoFromQueueObject(GameObject queueObject)
    {
        // Получаем CharInfo из компонента QueueObjectData
        QueueObjectData queueData = queueObject.GetComponent<QueueObjectData>();
        if (queueData != null && queueData.charInfo != null)
        {
            return queueData.charInfo;
        }
        
        // Fallback: определяем тип по спрайту
        Image image = queueObject.GetComponent<Image>();
        if (image != null && image.sprite != null)
        {
            foreach (var kvp in characterSprites)
            {
                if (kvp.Value == image.sprite)
                {
                    return new CharInfo(kvp.Key, false, false, System.DateTime.Now);
                }
            }
        }
        return new CharInfo(CharacterType.Tomato, false, false, System.DateTime.Now); // Возвращаем Tomato как fallback
    }
    
    // Получить количество персонажей в очереди
    public int GetQueueCount()
    {
        return queueObjects.Count;
    }
} 
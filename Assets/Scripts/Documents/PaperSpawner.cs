using UnityEngine;

public class PaperSpawner : MonoBehaviour
{
    [Header("Настройки спаунера бумаги")]
    [Tooltip("Префаб бумаги (Paper)")]
    public GameObject paperPrefab;

    [Tooltip("Родительский объект для новых бумаг (например, Canvas)")]
    public Transform parentTransform;

    [Tooltip("Позиция спауна (если Vector2.zero, берётся позиция этого объекта)")]
    public Vector2 spawnPosition = Vector2.zero;

    [Tooltip("Случайное смещение при спауне (разброс)")]
    public float randomOffset = 30f;

    public GameObject SpawnPaper()
    {
        if (paperPrefab == null)
        {
            Debug.LogError("PaperSpawner: Не назначен префаб бумаги!");
            return null;
        }

        // Определяем позицию спауна
        Vector2 basePosition = spawnPosition;
        if (spawnPosition == Vector2.zero)
        {
            // Если spawnPosition не задан, берём позицию объекта
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
                basePosition = rt.anchoredPosition;
            else
                basePosition = (Vector2)transform.localPosition;
        }

        Vector2 offset = Random.insideUnitCircle * randomOffset;
        Vector2 finalPosition = basePosition + offset;

        // Создаём бумагу
        GameObject paperObj = Instantiate(paperPrefab, parentTransform);
        RectTransform rect = paperObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = finalPosition;
        }
        else
        {
            paperObj.transform.localPosition = finalPosition;
        }
        
        Debug.Log($"PaperSpawner: Создана бумажка {paperObj.name} в позиции {finalPosition}");
        
        return paperObj;
    }
} 
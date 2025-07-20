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

    public GameObject SpawnPaper(string paperText = "")
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
        
        // Устанавливаем текст на бумаге, если он передан
        if (!string.IsNullOrEmpty(paperText))
        {
            SetPaperText(paperObj, paperText);
        }
        
        return paperObj;
    }
    
    // Перегрузка для обратной совместимости (без текста)
    public GameObject SpawnPaper()
    {
        return SpawnPaper("");
    }
    
    // Вспомогательный метод для установки текста на бумаге
    private void SetPaperText(GameObject paperObj, string text)
    {
        // Получаем компонент Paper
        Paper paperComponent = paperObj.GetComponent<Paper>();
        if (paperComponent != null)
        {
            paperComponent.SetPaperText(text);
            Debug.Log($"PaperSpawner: Установлен текст на бумаге: '{text}'");
        }
        else
        {
            Debug.LogWarning("PaperSpawner: На созданной бумаге отсутствует компонент Paper!");
        }
    }
} 
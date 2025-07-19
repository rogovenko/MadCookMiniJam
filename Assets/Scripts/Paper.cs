using UnityEngine;
using TMPro;

public class Paper : Draggable
{
    [Header("Компоненты бумаги")]
    public UnityEngine.UI.Image paperImage;
    public TextMeshProUGUI paperText;
    
    [Header("Содержимое бумаги")]
    public string paperContent = "Sample Document";
    
    [Header("Настройки случайного вращения")]
    [Tooltip("Применять ли случайное вращение при создании")]
    [SerializeField] protected bool enableRandomRotation = true;
    
    [Tooltip("Максимальный угол случайного вращения в градусах")]
    [Range(0f, 180f)]
    [SerializeField] protected float maxRotationAngle = 15f;
    
    [Header("Настройки масштабирования")]
    [Tooltip("Стандартный масштаб бумаги")]
    [SerializeField] protected Vector3 defaultScale = Vector3.one;
    
    [Tooltip("Масштаб бумаги на полке")]
    [SerializeField] public Vector3 shelfScale = new Vector3(0.33f, 0.33f, 0.33f);
    
    [Tooltip("Время анимации изменения масштаба")]
    [SerializeField] protected float scaleAnimationDuration = 0.3f;
    
    // Переменные для запоминания позиции относительно курсора
    private Vector2 cursorOffset;
    private bool isOnShelf = false;
    
    protected override void Start()
    {
        base.Start();
        
        // Устанавливаем текст на бумаге
        if (paperText != null)
        {
            paperText.text = paperContent;
        }
        
        // Случайное вращение для реалистичности (если включено)
        if (enableRandomRotation)
        {
            float rotation = Random.Range(-maxRotationAngle, maxRotationAngle);
            rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }
    
    // Метод для изменения содержимого бумаги
    public void SetContent(string content)
    {
        paperContent = content;
        if (paperText != null)
        {
            paperText.text = content;
        }
    }
    
    // Переопределяем методы для дополнительной логики
    protected override void OnStartDrag()
    {

    }
    
    protected override void OnEndDrag()
    {

    }
    
    // Метод для принудительного применения случайного вращения
    public void ApplyRandomRotation()
    {
        if (enableRandomRotation)
        {
            float rotation = Random.Range(-maxRotationAngle, maxRotationAngle);
            rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }
    
    // Метод для сброса вращения
    public void ResetRotation()
    {
        rectTransform.localRotation = Quaternion.identity;
    }
    
    // Метод для установки масштаба полки
    public void SetShelfScale()
    {
        Debug.Log("Paper: SetShelfScale");
        if (rectTransform != null)
        {
            rectTransform.localScale = shelfScale;
        }
    }
    
    // Метод для установки стандартного масштаба
    public void SetDefaultScale()
    {
        Debug.Log("Paper: SetDefaultScale");
        if (rectTransform != null)
        {
            rectTransform.localScale = defaultScale;
        }
    }
    
    // Метод для плавного изменения масштаба
    public void AnimateScale(Vector3 targetScale)
    {
        if (rectTransform != null)
        {
            StartCoroutine(AnimateScaleCoroutine(targetScale));
        }
    }
    
    // Корутина для плавной анимации масштаба
    private System.Collections.IEnumerator AnimateScaleCoroutine(Vector3 targetScale)
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < scaleAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / scaleAnimationDuration;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        
        rectTransform.localScale = targetScale;
    }
    
    // Метод для проверки, находится ли бумага на полке
    public bool IsOnShelf()
    {
        return rectTransform.localScale == shelfScale;
    }

    // Уменьшить бумагу и центрировать под курсором, обновить offset для drag
    public void SnapToCursorAndScale(Vector3 newScale)
    {
        if (rectTransform == null) return;
        RectTransform parentRect = rectTransform.parent as RectTransform;
        if (parentRect == null) return;

        // Получаем позицию курсора в локальных координатах родителя
        Vector2 mousePosition = Input.mousePosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, mousePosition, null, out Vector2 localCursorPos))
        {
            // Сохраняем offset между центром бумаги и курсором
            Vector2 oldOffset = (Vector2)rectTransform.localPosition - localCursorPos;

            // Меняем масштаб
            rectTransform.localScale = newScale;

            // Пересчитываем offset с учетом нового масштаба
            Vector2 newOffset = oldOffset * (newScale.x / rectTransform.localScale.x);

            // Ставим бумагу так, чтобы центр был под курсором
            rectTransform.localPosition = localCursorPos + newOffset;

            // Обновляем offset в Draggable (чтобы drag был от центра)
            var draggable = GetComponent<Draggable>();
            if (draggable != null)
            {
                var offsetField = typeof(Draggable).GetField("offset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (offsetField != null)
                {
                    offsetField.SetValue(draggable, Vector2.zero);
                }
            }
        }
    }
} 
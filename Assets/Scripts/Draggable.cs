using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Опции перетаскивания")]
    [Tooltip("Увеличивать ли объект при перетаскивании")]
    [SerializeField] protected bool enableScaleOnDrag = true;
    
    [Tooltip("Во сколько раз увеличивать объект при перетаскивании")]
    [SerializeField] protected float dragScaleMultiplier = 1.1f;
    
    [Tooltip("Скорость изменения размера (чем больше, тем быстрее)")]
    [SerializeField] protected float scaleSpeed = 5f;
    
    [Tooltip("Делать ли объект полупрозрачным при перетаскивании")]
    [SerializeField] protected bool enableAlphaOnDrag = true;
    
    [Tooltip("Прозрачность объекта во время перетаскивания (0 = полностью прозрачный, 1 = полностью непрозрачный)")]
    [Range(0f, 1f)]
    [SerializeField] protected float dragAlpha = 0.8f;
    
    protected bool isDragging = false;
    protected Vector2 offset;
    protected Canvas canvas;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;
    protected Vector3 originalScale;
    protected Vector3 targetScale;
    
    protected virtual void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Если нет CanvasGroup, создаем его
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Сохраняем оригинальный размер
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
    }
    
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // Начинаем перетаскивание
        isDragging = true;
        
        // Получаем позицию мыши в локальных координатах родителя
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        
        // Вычисляем смещение от позиции объекта до позиции мыши
        offset = (Vector2)rectTransform.localPosition - localPoint;
        
        // Делаем объект полупрозрачным во время перетаскивания (если включено)
        if (enableAlphaOnDrag)
        {
            canvasGroup.alpha = dragAlpha;
        }
        
        // Увеличиваем размер объекта (если включено)
        if (enableScaleOnDrag)
        {
            targetScale = originalScale * dragScaleMultiplier;
        }
        
        // Уведомляем систему событий
        if (DragEventSystem.Instance != null)
        {
            DragEventSystem.Instance.NotifyDragStart(this);
        }
        
        // Вызываем метод для дополнительной логики в наследниках
        OnStartDrag();
    }
    
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Получаем текущую позицию мыши в локальных координатах родителя
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);
            
            // Устанавливаем позицию объекта с учетом смещения
            rectTransform.localPosition = localPoint + offset;
            
            // Вызываем метод для дополнительной логики в наследниках
            OnDrag();
        }
    }
    
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // Заканчиваем перетаскивание
        isDragging = false;
        
        // Возвращаем полную прозрачность
        canvasGroup.alpha = 1f;
        
        // Возвращаем нормальный размер объекта (если было включено увеличение)
        // Но только если объект не находится на полке
        if (enableScaleOnDrag)
        {
            Paper paper = GetComponent<Paper>();
            if (paper == null || !paper.IsOnShelf())
            {
                targetScale = originalScale;
            }
        }
        
        // Уведомляем систему событий
        if (DragEventSystem.Instance != null)
        {
            DragEventSystem.Instance.NotifyDragEnd(this);
        }
        
        // Вызываем метод для дополнительной логики в наследниках
        OnEndDrag();
    }
    
    protected virtual void Update()
    {
        // Плавно изменяем размер объекта (если включено)
        // Но не изменяем масштаб, если объект находится на полке
        if (enableScaleOnDrag && rectTransform.localScale != targetScale)
        {
            Paper paper = GetComponent<Paper>();
            if (paper == null || !paper.IsOnShelf())
            {
                rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            }
        }
    }
    
    // Виртуальные методы для переопределения в наследниках
    protected virtual void OnStartDrag() { }
    protected virtual void OnDrag() { }
    protected virtual void OnEndDrag() { }
} 
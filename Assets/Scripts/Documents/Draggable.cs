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
    
    [Tooltip("Имя элемента, перед которым нужно ставить объект при перетаскивании (оставьте пустым, если не нужно)")]
    [SerializeField] protected string dragInsertBeforeName;
    
    protected Transform dragInsertBefore;
    
    protected bool isDragging = false;
    protected Vector2 offset;
    protected Canvas canvas;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;
    protected Vector3 originalScale;
    protected Vector3 targetScale;
    protected Vector3 originalPosition;
    protected Vector3 dragStartPosition; // Позиция в момент начала перетаскивания
    protected Vector3 dragStartScale; // Масштаб в момент начала перетаскивания
    protected Vector2 originalSize;
    protected bool wasOnShelfAtDragStart; // Был ли объект на полке в момент начала перетаскивания

    protected GameManager gameManager; // Ссылка на GameManager
    
    protected virtual void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Находим GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning($"{nameof(Draggable)}: GameManager не найден на сцене!");
        }
        
        // Если нет CanvasGroup, создаем его
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Сохраняем оригинальный размер и позицию
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalSize = rectTransform.sizeDelta;
        targetScale = originalScale;
        
        // Ищем элемент с нужным именем среди детей родителя
        if (!string.IsNullOrEmpty(dragInsertBeforeName) && rectTransform.parent != null)
        {
            foreach (Transform child in rectTransform.parent)
            {
                if (child.name == dragInsertBeforeName)
                {
                    dragInsertBefore = child;
                    break;
                }
            }
        }
        // Если нашли dragInsertBefore, размещаем объект перед ним (targetIndex - 1)
        if (dragInsertBefore != null && rectTransform.parent == dragInsertBefore.parent)
        {
            int targetIndex = dragInsertBefore.GetSiblingIndex();
            rectTransform.SetSiblingIndex(targetIndex - 1);
        }
    }
    
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (gameManager.isTutorial)
        {
            return;
        }

        // Проверяем, включен ли drag'n'drop глобально
        if (DragEventSystem.Instance != null && !DragEventSystem.Instance.IsDragDropEnabled())
        {
            return; // Не начинаем перетаскивание если drag'n'drop отключен
        }
        
        // Начинаем перетаскивание
        isDragging = true;
        
        // Сохраняем позицию и масштаб в момент начала перетаскивания
        dragStartPosition = rectTransform.localPosition;
        dragStartScale = rectTransform.localScale;
        
        // Определяем, был ли объект на полке в момент начала перетаскивания
        wasOnShelfAtDragStart = IsOnShelfAtPosition(dragStartPosition);
        
        // Получаем позицию мыши в локальных координатах родителя
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        
        // Вычисляем смещение от позиции объекта до позиции мыши
        offset = (Vector2)rectTransform.localPosition - localPoint;
        
        // Воспроизводим звук начала перетаскивания
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDrag();
        }
        
        // Увеличиваем размер объекта (если включено)
        if (enableScaleOnDrag)
        {
            targetScale = originalScale * dragScaleMultiplier;
        }
        
        // Перемещаем объект в иерархии перед найденным элементом, если он найден
        if (dragInsertBefore != null && rectTransform.parent == dragInsertBefore.parent)
        {
            int targetIndex = dragInsertBefore.GetSiblingIndex();
            rectTransform.SetSiblingIndex(targetIndex - 1);
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
        
        // Воспроизводим звук окончания перетаскивания
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDrop();
        }
        
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
    
    // Возврат в исходную позицию
    public void ReturnToOriginalPosition()
    {
        if (rectTransform != null)
        {
            rectTransform.localPosition = originalPosition;
            rectTransform.sizeDelta = originalSize;
            rectTransform.localScale = originalScale;
        }
    }
    
    // Возврат в позицию начала перетаскивания
    public void ReturnToDragStartPosition()
    {
        if (rectTransform != null)
        {
            rectTransform.localPosition = dragStartPosition;
            rectTransform.localScale = dragStartScale;
            // Не изменяем размер, оставляем как есть
        }
    }
    
    // Получение исходной позиции
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
    
    // Получение позиции в момент начала перетаскивания
    public Vector3 GetDragStartPosition()
    {
        return dragStartPosition;
    }
    
    // Проверяем, был ли объект на полке в момент начала перетаскивания
    public bool WasOnShelfAtDragStart()
    {
        return wasOnShelfAtDragStart;
    }
    
    // Получение масштаба в момент начала перетаскивания
    public Vector3 GetDragStartScale()
    {
        return dragStartScale;
    }
    
    // Проверяем, находится ли позиция на полке
    private bool IsOnShelfAtPosition(Vector3 position)
    {
        // Ищем все ShelfZone в сцене
        ShelfZone[] shelfZones = FindObjectsOfType<ShelfZone>();
        
        foreach (ShelfZone shelfZone in shelfZones)
        {
            RectTransform shelfRect = shelfZone.GetComponent<RectTransform>();
            if (shelfRect != null)
            {
                // Преобразуем мировую позицию в экранные координаты
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);
                
                // Преобразуем в локальные координаты зоны
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    shelfRect, screenPoint, null, out localPoint))
                {
                    Rect shelfBounds = shelfRect.rect;
                    if (shelfBounds.Contains(localPoint))
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
} 
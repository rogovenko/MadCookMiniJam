using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class ShelfZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки полки")]
    [Tooltip("Название зоны полки")]
    [SerializeField] private string zoneName = "Полка";
    
    [Tooltip("Цвет подсветки при наведении")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Tooltip("Стандартный цвет зоны")]
    [SerializeField] private Color defaultColor = new Color(1f, 1f, 1f, 0.1f);
    
    [Tooltip("Включить ли подсветку при наведении")]
    [SerializeField] private bool enableHighlight = true;
    
    private UnityEngine.UI.Image zoneImage;
    private Color originalColor;
    private bool isHighlighted = false;
    private bool isPointerOver = false;
    private bool isZoneActive = false;
    
    void Start()
    {
        // Получаем компонент Image для визуальных эффектов
        zoneImage = GetComponent<UnityEngine.UI.Image>();
        if (zoneImage != null)
        {
            originalColor = zoneImage.color;
        }
        
        // Регистрируемся в системе событий
        if (DragEventSystem.Instance != null)
        {
            DragEventSystem.Instance.RegisterShelfZone(this);
        }
    }
    
    void OnDestroy()
    {
        // Отменяем регистрацию при уничтожении
        if (DragEventSystem.Instance != null)
        {
            DragEventSystem.Instance.UnregisterShelfZone(this);
        }
    }
    
    // Вызывается при входе указателя в зону
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Подсвечиваем зону при входе указателя (только когда не перетаскивается объект)
        if (!Input.GetMouseButton(0)) // Только если не зажата кнопка мыши
        {
            isPointerOver = true;
            HighlightZone();
        }
    }
    
    // Вызывается при выходе указателя из зоны
    public void OnPointerExit(PointerEventData eventData)
    {
        // Возвращаем нормальный цвет при выходе указателя (только когда не перетаскивается объект)
        if (!Input.GetMouseButton(0)) // Только если не зажата кнопка мыши
        {
            isPointerOver = false;
            UnhighlightZone();
        }
    }
    
    void Update()
    {
        // Проверяем, есть ли перетаскиваемый объект
        if (Input.GetMouseButton(0)) // Если зажата левая кнопка мыши
        {
            CheckDraggedObjectOverZone();
        }
        else if (isPointerOver && !isHighlighted)
        {
            // Если указатель над зоной, но подсветка выключена
            HighlightZone();
        }
    }
    
    void CheckDraggedObjectOverZone()
    {
        // Получаем позицию мыши в экранных координатах
        Vector2 mousePosition = Input.mousePosition;
        
        // Преобразуем в локальные координаты зоны
        RectTransform zoneRect = GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            zoneRect, mousePosition, null, out Vector2 localPoint))
        {
            // Проверяем, находится ли точка внутри зоны
            Rect zoneRectBounds = zoneRect.rect;
            bool isInside = zoneRectBounds.Contains(localPoint);
            
            if (isInside && !isHighlighted)
            {
                HighlightZone();
                
                // Уведомляем систему событий о входе в зону
                if (DragEventSystem.Instance != null)
                {
                    Draggable currentDragged = DragEventSystem.Instance.GetCurrentDraggedObject();
                    if (currentDragged != null)
                    {
                        DragEventSystem.Instance.NotifyEnterShelfZone(currentDragged, this);
                        // Активируем изменение размера бумаги
                        ActivatePaperResize(currentDragged);
                    }
                }
            }
            else if (!isInside && isHighlighted)
            {
                UnhighlightZone();
                
                // Уведомляем систему событий о выходе из зоны
                if (DragEventSystem.Instance != null)
                {
                    Draggable currentDragged = DragEventSystem.Instance.GetCurrentDraggedObject();
                    if (currentDragged != null)
                    {
                        DragEventSystem.Instance.NotifyExitShelfZone(currentDragged, this);
                        // Деактивируем изменение размера бумаги
                        DeactivatePaperResize(currentDragged);
                    }
                }
            }
        }
    }
    
    // Подсветка зоны
    private void HighlightZone()
    {
        if (zoneImage != null)
        {
            zoneImage.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a);
            isHighlighted = true;
        }
    }
    
    // Убираем подсветку зоны
    private void UnhighlightZone()
    {
        if (zoneImage != null)
        {
            zoneImage.color = originalColor;
            isHighlighted = false;
        }
    }
    
    // Активируем изменение размера бумаги
    private void ActivatePaperResize(Draggable draggable)
    {
        Paper paper = draggable.GetComponent<Paper>();
        if (paper != null)
        {
            paper.SnapToCursorAndScale(paper.shelfScale);
            Debug.Log($"ShelfZone: Бумага '{paper.paperContent}' уменьшена и центрирована на полке");
        }
    }
    
    // Деактивируем изменение размера бумаги
    private void DeactivatePaperResize(Draggable draggable)
    {
        Paper paper = draggable.GetComponent<Paper>();
        if (paper != null)
        {
            paper.SetDefaultScale();
            Debug.Log($"ShelfZone: Бумага '{paper.paperContent}' возвращена к стандартному размеру");
        }
    }
    
    // Метод для принудительной активации/деактивации зоны
    public void SetZoneActive(bool active)
    {
        isZoneActive = active;
    }
    
    // Метод для изменения цвета подсветки
    public void SetHighlightColor(Color color)
    {
        highlightColor = color;
    }
    
    // Метод для изменения стандартного цвета
    public void SetDefaultColor(Color color)
    {
        defaultColor = color;
        if (zoneImage != null && !isHighlighted)
        {
            zoneImage.color = color;
        }
    }
    
    // Метод для получения информации о зоне
    public string GetZoneInfo()
    {
        return $"Зона полки: {zoneName}\n" +
               $"Подсветка: {(isHighlighted ? "Включена" : "Выключена")}\n" +
               $"Активна: {isZoneActive}";
    }
} 
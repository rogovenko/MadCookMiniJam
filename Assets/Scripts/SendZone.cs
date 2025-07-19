using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SendZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки зоны утилизации")]
    [Tooltip("Название зоны утилизации")]
    [SerializeField] private string zoneName = "Утилизатор";
    
    [Tooltip("Скорость исчезновения объекта")]
    [SerializeField] private float destroySpeed = 2f;
    
    [Tooltip("Звук при утилизации (опционально)")]
    [SerializeField] private AudioClip destroySound;
    
    [Header("Визуальные эффекты")]
    [Tooltip("Цвет подсветки зоны при наведении")]
    [SerializeField] private Color highlightColor = Color.red;
    
    [Tooltip("Прозрачность подсветки")]
    [Range(0f, 1f)]
    [SerializeField] private float highlightAlpha = 0.3f;
    
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
            DragEventSystem.Instance.RegisterZone(this);
        }
    }
    
    void OnDestroy()
    {
        // Отменяем регистрацию при уничтожении
        if (DragEventSystem.Instance != null)
        {
            DragEventSystem.Instance.UnregisterZone(this);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Подсвечиваем зону при входе указателя (только когда не перетаскивается объект)
        if (!Input.GetMouseButton(0)) // Только если не зажата кнопка мыши
        {
            isPointerOver = true;
            HighlightZone();
        }
    }
    
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
                        DragEventSystem.Instance.NotifyEnterZone(currentDragged, this);
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
                        DragEventSystem.Instance.NotifyExitZone(currentDragged, this);
                    }
                }
            }
        }
    }
    
    void HighlightZone()
    {
        if (zoneImage != null)
        {
            zoneImage.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightAlpha);
            isHighlighted = true;
        }
    }
    
    void UnhighlightZone()
    {
        if (zoneImage != null)
        {
            zoneImage.color = originalColor;
            isHighlighted = false;
        }
    }
    
    public void HandleDrop(Draggable draggable)
    {
        Paper paper = draggable.GetComponent<Paper>();
        if (paper != null)
        {
            // Проверяем, является ли это Order
            Order order = paper.GetComponent<Order>();
            if (order != null)
            {
                // Если это Order - размещаем его в SendZone
                Debug.Log("SendZone: Размещаем Order в SendZone");
                CompleteOrder(order);
            }
            else
            {
                // Если это не Order - проверяем, откуда был взят объект
                if (WasObjectFromShelfZone(draggable))
                {
                    // Если объект был взят из ShelfZone - возвращаем его туда
                    Debug.Log("SendZone: Возвращаем объект обратно в ShelfZone");
                    ReturnToShelfZone(draggable);
                }
                else
                {
                    // Если объект был взят из другого места - возвращаем в исходную позицию
                    Debug.Log("SendZone: Возвращаем объект в исходную позицию");
                    ReturnToOriginalPosition(draggable);
                }
            }
        }
    }
    

    
    // Метод для проверки, находится ли объект в зоне
    public bool IsObjectInZone(GameObject obj)
    {
        if (obj == null) return false;
        
        RectTransform zoneRect = GetComponent<RectTransform>();
        RectTransform objRect = obj.GetComponent<RectTransform>();
        
        if (zoneRect == null || objRect == null) return false;
        
        // Проверяем, пересекаются ли прямоугольники
        Rect zoneRectBounds = zoneRect.rect;
        Rect objRectBounds = objRect.rect;
        
        Vector3 zoneWorldPos = zoneRect.position;
        Vector3 objWorldPos = objRect.position;
        
        // Простая проверка расстояния между центрами
        float distance = Vector3.Distance(zoneWorldPos, objWorldPos);
        float zoneRadius = Mathf.Max(zoneRectBounds.width, zoneRectBounds.height) * 0.5f;
        
        return distance < zoneRadius;
    }
    
    // Метод для активации/деактивации зоны
    public void SetZoneActive(bool active)
    {
        isZoneActive = active;
    }
    
    // Проверяем, был ли объект взят из ShelfZone
    private bool WasObjectFromShelfZone(Draggable draggable)
    {
        if (draggable == null) return false;
        
        // Используем встроенную проверку из Draggable
        return draggable.WasOnShelfAtDragStart();
    }
    
    // Размещение Order в SendZone
    private void CompleteOrder(Order order)
    {
        if (order != null)
        {
            RectTransform orderRect = order.GetComponent<RectTransform>();
            RectTransform sendZoneRect = GetComponent<RectTransform>();
            
            if (orderRect != null && sendZoneRect != null)
            {
                // Устанавливаем позицию Order в позицию SendZone
                orderRect.localPosition = sendZoneRect.localPosition;
                
                // Устанавливаем масштаб 1,1,1
                orderRect.localScale = Vector3.one;
                
                // Устанавливаем угол поворота 0
                orderRect.localRotation = Quaternion.identity;
                
                // Отключаем возможность перетаскивания
                Draggable draggable = order.GetComponent<Draggable>();
                if (draggable != null)
                {
                    draggable.enabled = false;
                }
                
                Debug.Log($"SendZone: Order {order.name} размещен в SendZone с нормальным масштабом и углом");
            }
        }
    }
    
    // Возврат объекта в ShelfZone
    private void ReturnToShelfZone(Draggable draggable)
    {
        if (draggable != null)
        {
            // Возвращаем в позицию начала перетаскивания
            draggable.ReturnToDragStartPosition();
            
            // Устанавливаем масштаб полки (так как объект был на полке)
            Paper paper = draggable.GetComponent<Paper>();
            if (paper != null)
            {
                paper.SnapToCursorAndScale(paper.shelfScale);
            }
            
            Debug.Log($"SendZone: Объект {draggable.name} возвращен в ShelfZone с масштабом полки");
        }
    }
    
    // Метод для возврата объекта в позицию начала перетаскивания
    private void ReturnToOriginalPosition(Draggable draggable)
    {
        if (draggable != null)
        {
            // Возвращаем в позицию и масштаб начала перетаскивания
            draggable.ReturnToDragStartPosition();
            
            // Воспроизводим звук возврата (если есть)
            if (destroySound != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, Camera.main.transform.position);
            }
        }
    }
    
    // Метод для получения статистики зоны
    public string GetZoneInfo()
    {
        return $"Зона утилизации: {zoneName}\n" +
               $"Скорость уничтожения: {destroySpeed}\n" +
               $"Подсветка: {(isHighlighted ? "Включена" : "Выключена")}\n" +
               $"Активна: {isZoneActive}";
    }
} 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DragEventSystem : MonoBehaviour
{
    [Header("Система событий перетаскивания")]
    public static DragEventSystem Instance { get; private set; }

    public UnityEvent<Draggable> OnDragStart = new UnityEvent<Draggable>();
    public UnityEvent<Draggable> OnDragEnd = new UnityEvent<Draggable>();
    public UnityEvent<Draggable, SendZone> OnEnterZone = new UnityEvent<Draggable, SendZone>();
    public UnityEvent<Draggable, SendZone> OnExitZone = new UnityEvent<Draggable, SendZone>();
    public UnityEvent<Draggable, ShelfZone> OnEnterShelfZone = new UnityEvent<Draggable, ShelfZone>();
    public UnityEvent<Draggable, ShelfZone> OnExitShelfZone = new UnityEvent<Draggable, ShelfZone>();

    private Draggable currentDraggedObject;
    private List<SendZone> activeZones = new List<SendZone>();
    private List<ShelfZone> activeShelfZones = new List<ShelfZone>();
    private SendZone currentZoneUnderDraggedObject;
    private ShelfZone currentShelfZoneUnderDraggedObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FindAllZones();
    }

    void FindAllZones()
    {
        SendZone[] zones = FindObjectsOfType<SendZone>();
        activeZones.AddRange(zones);
        
        ShelfZone[] shelfZones = FindObjectsOfType<ShelfZone>();
        activeShelfZones.AddRange(shelfZones);
    }

    public void RegisterZone(SendZone zone)
    {
        if (!activeZones.Contains(zone))
        {
            activeZones.Add(zone);
        }
    }

    public void UnregisterZone(SendZone zone)
    {
        if (activeZones.Contains(zone))
        {
            activeZones.Remove(zone);
        }
    }
    
    public void RegisterShelfZone(ShelfZone zone)
    {
        if (!activeShelfZones.Contains(zone))
        {
            activeShelfZones.Add(zone);
        }
    }

    public void UnregisterShelfZone(ShelfZone zone)
    {
        if (activeShelfZones.Contains(zone))
        {
            activeShelfZones.Remove(zone);
        }
    }

    public void NotifyDragStart(Draggable draggable)
    {
        currentDraggedObject = draggable;
        OnDragStart.Invoke(draggable);
        Debug.Log($"Начато перетаскивание: {draggable.name}");
        ActivateAllZones();
    }
    
    public void NotifyEnterShelfZone(Draggable draggable, ShelfZone zone)
    {
        currentShelfZoneUnderDraggedObject = zone;
        OnEnterShelfZone.Invoke(draggable, zone);
        Debug.Log($"Объект {draggable.name} вошел в полку {zone.name}");
    }

    public void NotifyExitShelfZone(Draggable draggable, ShelfZone zone)
    {
        if (currentShelfZoneUnderDraggedObject == zone)
            currentShelfZoneUnderDraggedObject = null;
        OnExitShelfZone.Invoke(draggable, zone);
        Debug.Log($"Объект {draggable.name} вышел из полки {zone.name}");
    }

    public void NotifyDragEnd(Draggable draggable)
    {
        OnDragEnd.Invoke(draggable);
        Debug.Log($"Завершено перетаскивание: {draggable.name}");
        DeactivateAllZones();

        // Если объект был над зоной, вызываем HandleDrop
        if (currentZoneUnderDraggedObject != null)
        {
            currentZoneUnderDraggedObject.HandleDrop(draggable);
            currentZoneUnderDraggedObject = null;
        }
        currentDraggedObject = null;
    }

    public void NotifyEnterZone(Draggable draggable, SendZone zone)
    {
        currentZoneUnderDraggedObject = zone;
        OnEnterZone.Invoke(draggable, zone);
    }

    public void NotifyExitZone(Draggable draggable, SendZone zone)
    {
        if (currentZoneUnderDraggedObject == zone)
            currentZoneUnderDraggedObject = null;
        OnExitZone.Invoke(draggable, zone);
    }

    private void ActivateAllZones()
    {
        foreach (SendZone zone in activeZones)
        {
            zone.SetZoneActive(true);
        }
        foreach (ShelfZone zone in activeShelfZones)
        {
            zone.SetZoneActive(true);
        }
    }

    private void DeactivateAllZones()
    {
        foreach (SendZone zone in activeZones)
        {
            zone.SetZoneActive(false);
        }
        foreach (ShelfZone zone in activeShelfZones)
        {
            zone.SetZoneActive(false);
        }
    }

    public Draggable GetCurrentDraggedObject()
    {
        return currentDraggedObject;
    }

    public bool IsDragging()
    {
        return currentDraggedObject != null;
    }

    public List<SendZone> GetAllZones()
    {
        return new List<SendZone>(activeZones);
    }
} 
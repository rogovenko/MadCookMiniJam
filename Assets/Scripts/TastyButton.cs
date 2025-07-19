using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TastyButton : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Image компонент кнопки")]
    [SerializeField] private Image buttonImage;
    
    [Tooltip("Text компонент кнопки")]
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [Header("Colors")]
    [Tooltip("Цвет активного состояния (зеленый)")]
    [SerializeField] private Color activeColor = Color.green;
    
    [Tooltip("Цвет неактивного состояния (серый)")]
    [SerializeField] private Color inactiveColor = Color.gray;
    
    [Header("Texts")]
    [Tooltip("Текст активного состояния")]
    [SerializeField] private string activeText = "TASTY";
    
    [Tooltip("Текст неактивного состояния")]
    [SerializeField] private string inactiveText = "USED";
    
    [Header("State")]
    [Tooltip("Активна ли кнопка")]
    [SerializeField] private bool isActivated = true;
    
    private Button button;
    
    private void Start()
    {
        // Получаем компонент Button
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("TastyButton: На объекте отсутствует компонент Button!");
            return;
        }
        
        // Добавляем обработчик клика
        button.onClick.AddListener(OnButtonClick);
        
        // Инициализируем состояние
        UpdateButtonState();
    }
    
    private void OnButtonClick()
    {
        // Переключаем состояние
        isActivated = !isActivated;
        UpdateButtonState();
        
        Debug.Log($"TastyButton: Состояние изменено на {(isActivated ? "активное" : "неактивное")}");
    }
    
    private void UpdateButtonState()
    {
        // Обновляем цвет
        if (buttonImage != null)
        {
            buttonImage.color = isActivated ? activeColor : inactiveColor;
        }
        
        // Обновляем текст
        if (buttonText != null)
        {
            buttonText.text = isActivated ? activeText : inactiveText;
        }
        
        // Обновляем интерактивность кнопки
        if (button != null)
        {
            button.interactable = true; // Кнопка всегда интерактивна, но меняет состояние
        }
    }
    
    // Сделать кнопку активной
    public void MakeActive()
    {
        isActivated = true;
        UpdateButtonState();
    }
    
    // Сделать кнопку неактивной
    public void MakeInactive()
    {
        isActivated = false;
        UpdateButtonState();
    }
    
    // Переключить состояние кнопки
    public void ToggleState()
    {
        isActivated = !isActivated;
        UpdateButtonState();
    }
    
    // Установить конкретное состояние
    public void SetState(bool active)
    {
        isActivated = active;
        UpdateButtonState();
    }
    
    // Получить текущее состояние
    public bool IsActivated()
    {
        return isActivated;
    }
    
    // Получить количество использований (сколько раз была нажата)
    public int GetUsageCount()
    {
        // Можно добавить счетчик использований если нужно
        return 0;
    }
    
    // Сбросить кнопку в активное состояние
    public void ResetButton()
    {
        isActivated = true;
        UpdateButtonState();
    }
    
    // Проверить, можно ли использовать кнопку
    public bool CanUse()
    {
        return isActivated;
    }
    
    // Использовать кнопку (сделать неактивной)
    public void UseButton()
    {
        if (isActivated)
        {
            MakeInactive();
        }
    }
} 
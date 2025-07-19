using UnityEngine;
using TMPro;

public class Calendar : MonoBehaviour
{
    [Header("Calendar Settings")]
    [SerializeField] private int selectedDay = 1;
    [SerializeField] private Month selectedMonth = Month.January;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI monthText;
    
    // Перечисление месяцев
    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
    
    private void Start()
    {
        UpdateCalendarDisplay();
    }
    
    // Обновить отображение календаря
    private void UpdateCalendarDisplay()
    {
        // Обновляем день
        if (dayText != null)
        {
            dayText.text = selectedDay.ToString();
        }
        
        // Обновляем месяц
        if (monthText != null)
        {
            monthText.text = GetMonthName(selectedMonth);
        }
    }
    
    // Получить название месяца
    private string GetMonthName(Month month)
    {
        switch (month)
        {
            case Month.January: return "JANUARY";
            case Month.February: return "FEBRUARY";
            case Month.March: return "MARCH";
            case Month.April: return "APRIL";
            case Month.May: return "MAY";
            case Month.June: return "JUNE";
            case Month.July: return "JULY";
            case Month.August: return "AUGUST";
            case Month.September: return "SEPTEMBER";
            case Month.October: return "OCTOBER";
            case Month.November: return "NOVEMBER";
            case Month.December: return "DECEMBER";
            default: return "JANUARY";
        }
    }
    
    // Установить день
    public void SetDay(int day)
    {
        selectedDay = Mathf.Clamp(day, 1, 31);
        UpdateCalendarDisplay();
    }
    
    // Установить месяц
    public void SetMonth(Month month)
    {
        selectedMonth = month;
        UpdateCalendarDisplay();
    }
    
    // Установить дату
    public void SetDate(int day, Month month)
    {
        SetDay(day);
        SetMonth(month);
    }
    
    // Получить текущий день
    public int GetDay()
    {
        return selectedDay;
    }
    
    // Получить текущий месяц
    public Month GetMonth()
    {
        return selectedMonth;
    }
    
    // Получить полную дату как строку
    public string GetFullDate()
    {
        return $"{selectedDay} {GetMonthName(selectedMonth)}";
    }
    
    // Увеличить день на 1
    public void NextDay()
    {
        SetDay(selectedDay + 1);
    }
    
    // Уменьшить день на 1
    public void PreviousDay()
    {
        SetDay(selectedDay - 1);
    }
    
    // Следующий месяц
    public void NextMonth()
    {
        int currentMonthIndex = (int)selectedMonth;
        int nextMonthIndex = (currentMonthIndex + 1) % 12;
        SetMonth((Month)nextMonthIndex);
    }
    
    // Предыдущий месяц
    public void PreviousMonth()
    {
        int currentMonthIndex = (int)selectedMonth;
        int previousMonthIndex = (currentMonthIndex - 1 + 12) % 12;
        SetMonth((Month)previousMonthIndex);
    }
} 
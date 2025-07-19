using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float initialTimeInSeconds = 300f;
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Timer State")]
    [SerializeField] private float currentTime;
    [SerializeField] private bool isRunning = false;
    
    // События
    public event Action OnTimerFinished;
    public event Action<float> OnTimeChanged;
    
    private void Start()
    {
        // Инициализируем таймер
        currentTime = initialTimeInSeconds;
        UpdateTimerDisplay();
    }
    
    private void Update()
    {
        if (isRunning && currentTime > 0)
        {
            // Уменьшаем время
            currentTime -= Time.deltaTime;
            
            // Проверяем, не истекло ли время
            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                OnTimerFinished?.Invoke();
            }
            
            // Обновляем отображение
            UpdateTimerDisplay();
            OnTimeChanged?.Invoke(currentTime);
        }
    }
    
    // Запустить таймер
    public void StartTimer()
    {
        isRunning = true;
    }
    
    // Остановить таймер
    public void StopTimer()
    {
        isRunning = false;
    }
    
    // Сбросить таймер к начальному времени
    public void ResetTimer()
    {
        currentTime = initialTimeInSeconds;
        UpdateTimerDisplay();
    }
    
    // Добавить время к таймеру
    public void AddTime(float secondsToAdd)
    {
        currentTime += secondsToAdd;
        UpdateTimerDisplay();
    }
    
    // Установить время
    public void SetTime(float timeInSeconds)
    {
        currentTime = Mathf.Max(0, timeInSeconds);
        UpdateTimerDisplay();
    }
    
    // Получить текущее время в секундах
    public float GetCurrentTime()
    {
        return currentTime;
    }
    
    // Проверить, истекло ли время
    public bool IsTimeUp()
    {
        return currentTime <= 0;
    }
    
    // Обновить отображение таймера
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }
    }
    
    // Форматировать время в формат MM:SS
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    // Получить форматированное время как строку
    public string GetFormattedTime()
    {
        return FormatTime(currentTime);
    }
} 
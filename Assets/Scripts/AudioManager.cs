using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Настройки AudioManager")]
    [Tooltip("Главная громкость звуков")]
    [SerializeField] private float masterVolume = 1f;
    
    [Tooltip("Громкость звуковых эффектов")]
    [SerializeField] private float sfxVolume = .5f;
    
    [Tooltip("Громкость музыки")]
    [SerializeField] private float musicVolume = 0.3f;
    
    [Header("Audio Sources")]
    [Tooltip("Audio Source для звуковых эффектов")]
    [SerializeField] private AudioSource sfxSource;
    
    [Tooltip("Audio Source для музыки")]
    [SerializeField] private AudioSource musicSource;
    
    [Header("Звуковые эффекты")]
    [Tooltip("Звук нажатия кнопки")]
    [SerializeField] private AudioClip buttonSound;
    
    [Tooltip("Звук перетаскивания")]
    [SerializeField] private AudioClip dragSound;
    
    [Tooltip("Звук бросания")]
    [SerializeField] private AudioClip dropSound;
    
    [Tooltip("Звук завершения заказа")]
    [SerializeField] private AudioClip orderDoneSound;
    
    [Tooltip("Звук шагов")]
    [SerializeField] private AudioClip stepSound;
    
    [Tooltip("Звук мусорки")]
    [SerializeField] private AudioClip trashSound;
    
    [Tooltip("Звук открытия книги")]
    [SerializeField] private AudioClip bookOpenSound;
    
    [Header("Музыка")]
    [Tooltip("Фоновая музыка")]
    [SerializeField] private AudioClip backgroundMusic;
    
    // Синглтон
    public static AudioManager Instance { get; private set; }
    
    // События для изменения громкости
    public System.Action<float> OnMasterVolumeChanged;
    public System.Action<float> OnSfxVolumeChanged;
    public System.Action<float> OnMusicVolumeChanged;
    
    void Awake()
    {
        // Синглтон паттерн
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Загружаем сохраненные настройки громкости
        // LoadAudioSettings();
        
        // Запускаем фоновую музыку
        PlayBackgroundMusic();
    }
    
    // Инициализация Audio Sources
    private void InitializeAudioSources()
    {
        // Создаем Audio Source для звуковых эффектов
        if (sfxSource == null)
        {
            GameObject sfxObject = new GameObject("SFX Audio Source");
            sfxObject.transform.SetParent(transform);
            sfxSource = sfxObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        // Создаем Audio Source для музыки
        if (musicSource == null)
        {
            GameObject musicObject = new GameObject("Music Audio Source");
            musicObject.transform.SetParent(transform);
            musicSource = musicObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }
    
    // === МЕТОДЫ ДЛЯ ЗВУКОВЫХ ЭФФЕКТОВ ===
    
    // Воспроизвести звук кнопки
    public void PlayButton()
    {
        PlaySFX(buttonSound);
    }
    
    // Воспроизвести звук перетаскивания
    public void PlayDrag()
    {
        PlaySFX(dragSound);
    }
    
    // Воспроизвести звук бросания
    public void PlayDrop()
    {
        PlaySFX(dropSound);
    }
    
    // Воспроизвести звук завершения заказа
    public void PlayOrderDone()
    {
        PlaySFX(orderDoneSound);
    }
    
    // Воспроизвести звук шагов
    public void PlayStep()
    {
        PlaySFX(stepSound);
    }
    
    // Воспроизвести звук мусорки
    public void PlayTrash()
    {
        PlaySFX(trashSound);
    }
    
    // Воспроизвести звук открытия книги
    public void PlayBookOpen()
    {
        PlaySFX(bookOpenSound);
    }
    
    // === ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ ДЛЯ СОВМЕСТИМОСТИ ===
    
    // Совместимость со старыми методами
    public void PlayButtonClick()
    {
        PlayButton();
    }
    
    public void PlayLevelComplete()
    {
        PlayOrderDone();
    }
    
    public void PlayGameWin()
    {
        PlayOrderDone();
    }
    
    public void PlayGameLose()
    {
        PlayTrash();
    }
    
    public void PlayFingerCut()
    {
        PlayTrash();
    }
    
    public void PlayCorrectOrder()
    {
        PlayOrderDone();
    }
    
    public void PlayWrongOrder()
    {
        PlayTrash();
    }
    
    public void PlayCharacterSpawn()
    {
        PlayStep();
    }
    
    public void PlayOrderSpawn()
    {
        PlayStep();
    }
    
    public void PlayPaperSpawn()
    {
        PlayBookOpen();
    }
    
    // === МЕТОДЫ ДЛЯ МУЗЫКИ ===
    
    // Воспроизвести фоновую музыку
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
            Debug.Log("AudioManager: Запущена фоновая музыка");
        }
    }
    
    // Остановить фоновую музыку
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            Debug.Log("AudioManager: Остановлена фоновая музыка");
        }
    }
    
    // === ОБЩИЕ МЕТОДЫ ===
    
    // Воспроизвести звуковой эффект
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            Debug.Log($"AudioManager: Воспроизведен звук {clip.name}");
        }
        else
        {
            Debug.LogWarning($"AudioManager: Не удалось воспроизвести звук {clip?.name}");
        }
    }
    
    // Воспроизвести звук с кастомной громкостью
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
            Debug.Log($"AudioManager: Воспроизведен звук {clip.name} с громкостью {volume}");
        }
    }
    
    // Воспроизвести звук с кастомной громкостью и питчем
    public void PlaySFX(AudioClip clip, float volume, float pitch)
    {
        if (clip != null && sfxSource != null)
        {
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
            sfxSource.pitch = originalPitch;
            Debug.Log($"AudioManager: Воспроизведен звук {clip.name} с громкостью {volume} и питчем {pitch}");
        }
    }
    
    // === УПРАВЛЕНИЕ ГРОМКОСТЬЮ ===
    
    // Установить главную громкость
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        OnMasterVolumeChanged?.Invoke(masterVolume);
        SaveAudioSettings();
        Debug.Log($"AudioManager: Установлена главная громкость: {masterVolume}");
    }
    
    // Установить громкость звуковых эффектов
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        OnSfxVolumeChanged?.Invoke(sfxVolume);
        SaveAudioSettings();
        Debug.Log($"AudioManager: Установлена громкость SFX: {sfxVolume}");
    }
    
    // Установить громкость музыки
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        OnMusicVolumeChanged?.Invoke(musicVolume);
        SaveAudioSettings();
        Debug.Log($"AudioManager: Установлена громкость музыки: {musicVolume}");
    }
    
    // Обновить все громкости
    private void UpdateAllVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    
    // Получить главную громкость
    public float GetMasterVolume()
    {
        return masterVolume;
    }
    
    // Получить громкость звуковых эффектов
    public float GetSfxVolume()
    {
        return sfxVolume;
    }
    
    // Получить громкость музыки
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    
    // === СОХРАНЕНИЕ НАСТРОЕК ===
    
    // Сохранить настройки звука
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }
    
    // Загрузить настройки звука
    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        UpdateAllVolumes();
        Debug.Log("AudioManager: Загружены настройки звука");
    }
    
    // === ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ ===
    
    // Включить/выключить звук
    public void ToggleSound(bool enabled)
    {
        if (enabled)
        {
            SetMasterVolume(1f);
        }
        else
        {
            SetMasterVolume(0f);
        }
    }
    
    // Включить/выключить музыку
    public void ToggleMusic(bool enabled)
    {
        if (enabled)
        {
            SetMusicVolume(0.7f);
            if (musicSource != null && !musicSource.isPlaying)
            {
                PlayBackgroundMusic();
            }
        }
        else
        {
            SetMusicVolume(0f);
            StopBackgroundMusic();
        }
    }
    
    // Остановить все звуки
    public void StopAllSounds()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        Debug.Log("AudioManager: Остановлены все звуки");
    }
    
    // Пауза/возобновление музыки
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
} 
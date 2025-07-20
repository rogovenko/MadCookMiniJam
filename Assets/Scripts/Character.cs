using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Character : MonoBehaviour
{
    [Header("Настройки персонажа")]
    [Tooltip("Текущий тип персонажа")]
    [SerializeField] private CharacterType characterType = CharacterType.Tomato;
    
    [Tooltip("Форма персонажа")]
    [SerializeField] private CharacterShape characterShape = CharacterShape.Round;
    
    [Tooltip("Кастомное название персонажа")]
    [SerializeField] private string customCharacterName = "";
    
    [Tooltip("Текущий дефект персонажа")]
    [SerializeField] private CharacterDefect currentDefect = CharacterDefect.None;
    
    [Tooltip("Была ли уже запрошена бумажка у этого персонажа")]
    [SerializeField] private bool hasBeenAskedForPapers = false;
    
    [Tooltip("Информация о персонаже (сорт, место происхождения, дефекты и т.д.)")]
    [SerializeField] private CharInfo characterInfo;

    [Header("Компоненты")]
    [Tooltip("Image компонент для отображения тела персонажа")]
    [SerializeField] private Image bodyImage;
    
    [Tooltip("Image компонент для отображения глаз")]
    [SerializeField] private Image eyesImage;
    
    [Tooltip("Image компонент для отображения одежды")]
    [SerializeField] private Image clothesImage;
    
    [Tooltip("Image компонент для отображения дефектов")]
    [SerializeField] private Image defectImage;

    void Start()
    {
        // Если Image компоненты не назначены, пытаемся найти их на этом объекте
        if (bodyImage == null)
        {
            bodyImage = GetComponent<Image>();
        }
        
        // Глаза и одежда должны быть дочерними объектами
        if (eyesImage == null)
        {
            eyesImage = transform.Find("Eyes")?.GetComponent<Image>();
        }
        
        if (clothesImage == null)
        {
            clothesImage = transform.Find("Clothes")?.GetComponent<Image>();
        }
        
        if (defectImage == null)
        {
            defectImage = transform.Find("Defect")?.GetComponent<Image>();
        }
    }

    // Установить тип персонажа
    public void SetCharacterType(CharacterType newType)
    {
        characterType = newType;
    }
    
    // Установить форму персонажа
    public void SetCharacterShape(CharacterShape newShape)
    {
        characterShape = newShape;
    }
    
    // Установить спрайт тела
    public void SetBodySprite(Sprite sprite)
    {
        if (bodyImage == null)
        {
            Debug.LogError($"Character: Body Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            bodyImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт тела для {characterType} не назначен!");
        }
    }
    
    // Установить спрайт глаз
    public void SetEyesSprite(Sprite sprite)
    {
        if (eyesImage == null)
        {
            Debug.LogError($"Character: Eyes Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            eyesImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт глаз для {characterType} не назначен!");
        }
    }
    
    // Установить спрайт одежды
    public void SetClothesSprite(Sprite sprite)
    {
        if (clothesImage == null)
        {
            Debug.LogError($"Character: Clothes Image компонент не найден на {gameObject.name}!");
            return;
        }

        if (sprite != null)
        {
            clothesImage.sprite = sprite;
        }
        else
        {
            Debug.LogError($"Character: Спрайт одежды для {characterType} не назначен!");
        }
    }
    
    // Применить дефект к персонажу
    public void ApplyDefect(CharacterDefect defect)
    {
        ApplyDefect(defect, characterShape);
    }
    
    // Применить дефект к персонажу с указанием формы
    public void ApplyDefect(CharacterDefect defect, CharacterShape shape)
    {
        currentDefect = defect;
        
        if (defectImage == null)
        {
            Debug.LogError($"Character: Defect Image компонент не найден на {gameObject.name}!");
            return;
        }
        
        if (defect == CharacterDefect.None)
        {
            // Скрываем компонент дефекта
            defectImage.gameObject.SetActive(false);
        }
        else
        {
            // Показываем компонент дефекта
            defectImage.gameObject.SetActive(true);
            
            // Получаем спрайт дефекта из CharManager с учетом формы
            CharManager charManager = FindObjectOfType<CharManager>();
            if (charManager != null)
            {
                Sprite defectSprite = charManager.GetDefectSprite(defect, shape);
                if (defectSprite != null)
                {
                    defectImage.sprite = defectSprite;
                    Debug.Log($"Character: Применен дефект {defect} (форма: {shape}) к {gameObject.name}");
                }
                else
                {
                    Debug.LogWarning($"Character: Спрайт для дефекта {defect} (форма: {shape}) не найден!");
                    defectImage.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Character: CharManager не найден в сцене!");
                defectImage.gameObject.SetActive(false);
            }
        }
    }

    // Получить текущий тип персонажа
    public CharacterType GetCharacterType()
    {
        return characterType;
    }
    
    // Получить форму персонажа
    public CharacterShape GetCharacterShape()
    {
        return characterShape;
    }
    
    // Получить текущий дефект персонажа
    public CharacterDefect GetCurrentDefect()
    {
        return currentDefect;
    }
    
    // Получить спрайт персонажа (тело)
    public Sprite GetCharacterSprite()
    {
        return bodyImage != null ? bodyImage.sprite : null;
    }
    
    // Получить спрайт глаз
    public Sprite GetEyesSprite()
    {
        return eyesImage != null ? eyesImage.sprite : null;
    }
    
    // Проверить, была ли уже запрошена бумажка
    public bool HasBeenAskedForPapers()
    {
        return hasBeenAskedForPapers;
    }
    
    // Отметить, что бумажка была запрошена
    public void MarkAsAskedForPapers()
    {
        hasBeenAskedForPapers = true;
    }
    
    // Сбросить флаг запроса бумажек (для повторного использования персонажа)
    public void ResetPapersRequest()
    {
        hasBeenAskedForPapers = false;
    }
    
    // "Раздеть" персонажа - убрать одежду
    public void GetNaked()
    {
        if (clothesImage != null)
        {
            // Скрываем компонент одежды
            clothesImage.gameObject.SetActive(false);
            
            Debug.Log($"Character: Персонаж {gameObject.name} разделся (одежда убрана)");
        }
        else
        {
            Debug.LogWarning($"Character: Clothes Image компонент не найден на {gameObject.name}!");
        }
    }

    // Получить имя персонажа
    public string GetCharacterName()
    {
        if (!string.IsNullOrEmpty(customCharacterName))
        {
            return customCharacterName;
        }
        return characterType.ToString();
    }

    // Установить кастомное имя
    public void SetCustomName(string name)
    {
        customCharacterName = name;
    }
    
    // Установить информацию о персонаже
    public void SetCharacterInfo(CharInfo info)
    {
        characterInfo = info;

        Debug.Log($"Character: есть ли дефекты? {characterInfo.HasDefects()}");
        
        // Автоматически применяем дефекты из CharInfo
        if (characterInfo != null && characterInfo.HasDefects())
        {
            Debug.Log($"Character: есть дефекты {characterInfo.defects[0]}");
            // Берем первый дефект (можно расширить для множественных дефектов)
            foreach (CharacterDefect defect in characterInfo.defects)
            {
                if (defect != CharacterDefect.None)
                {
                    ApplyDefect(defect, characterShape);
                    Debug.Log($"Character: Применен дефект {defect} из CharInfo к {gameObject.name}");
                    break; // Применяем только первый дефект для простоты
                }else{
                    Debug.Log($"Character: дефект None");
                    ApplyDefect(CharacterDefect.None, characterShape);
                }
            }
        }
        else if (characterInfo != null)
        {
            // Если дефектов нет, убираем все дефекты
            ApplyDefect(CharacterDefect.None, characterShape);
        }
        
        Debug.Log($"Character: Установлена информация о персонаже {characterInfo?.name} ({characterInfo?.GetVarietyDisplayName()} из {characterInfo?.GetOriginDisplayName()})");
    }
    
    // Получить информацию о персонаже
    public CharInfo GetCharacterInfo()
    {
        return characterInfo;
    }
    
    // Получить сорт овоща
    public VegetableVariety GetVariety()
    {
        return characterInfo?.variety ?? VegetableVariety.SunnyCherry;
    }
    
    // Получить место происхождения
    public OriginLocation GetOrigin()
    {
        return characterInfo?.origin ?? OriginLocation.SunnyvaleFarm;
    }
    
    // Получить срок годности в виде строки
    public string GetExpiryDate()
    {
        return characterInfo?.GetExpiryDateString() ?? "01/01";
    }
    
    // Проверить, есть ли дефекты у персонажа
    public bool HasCharacterDefects()
    {
        return characterInfo?.HasDefects() ?? false;
    }
    
    // Проверить, соответствует ли сорт месту происхождения
    public bool IsVarietyValidForOrigin()
    {
        return characterInfo?.IsVarietyValidForOrigin() ?? false;
    }
    
    // Определить ошибки в данных персонажа
    public List<string> GetCharacterErrors()
    {
        List<string> errors = new List<string>();
        
        if (characterInfo == null)
        {
            errors.Add("Отсутствует информация о персонаже");
            return errors;
        }
        
        // Проверяем правильность имени
        string expectedName = GetExpectedNameForType(characterType);
        if (characterInfo.name != expectedName)
        {
            errors.Add($"Неправильное имя: '{characterInfo.name}' вместо '{expectedName}'");
        }
        
        // Проверяем правильность сорта для типа персонажа
        VegetableVariety expectedVariety = GetExpectedVarietyForType(characterType);
        if (characterInfo.variety != expectedVariety)
        {
            errors.Add($"Неправильный сорт: '{characterInfo.GetVarietyDisplayName()}' вместо '{GetVarietyDisplayName(expectedVariety)}'");
        }
        
        // Проверяем правильность места происхождения для сорта
        OriginLocation expectedOrigin = GetExpectedOriginForVariety(characterInfo.variety);
        if (characterInfo.origin != expectedOrigin)
        {
            errors.Add($"Неправильное место происхождения: '{characterInfo.GetOriginDisplayName()}' вместо '{GetOriginDisplayName(expectedOrigin)}'");
        }
        
        // Проверяем срок годности (не должен быть просрочен)
        if (IsExpiryDateExpired(characterInfo))
        {
            errors.Add($"Просроченный срок годности: {characterInfo.GetExpiryDateString()}");
        }
        
        return errors;
    }
    
    // Получить ожидаемое имя для типа персонажа
    private string GetExpectedNameForType(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Tomato: return "Tomato";
            case CharacterType.Onion: return "Onion";
            case CharacterType.Potato: return "Potato";
            case CharacterType.Cucumber: return "Cucumber";
            case CharacterType.Carrot: return "Carrot";
            case CharacterType.Eggplant: return "Eggplant";
            default: return "Unknown";
        }
    }
    
    // Получить ожидаемый сорт для типа персонажа
    private VegetableVariety GetExpectedVarietyForType(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Tomato: return VegetableVariety.SunnyCherry;
            case CharacterType.Onion: return VegetableVariety.SnowShallot;
            case CharacterType.Potato: return VegetableVariety.Bellarose;
            case CharacterType.Cucumber: return VegetableVariety.PicklerGuest;
            case CharacterType.Carrot: return VegetableVariety.NantesShine;
            case CharacterType.Eggplant: return VegetableVariety.PrinceLilac;
            default: return VegetableVariety.SunnyCherry;
        }
    }
    
    // Получить ожидаемое место происхождения для сорта
    private OriginLocation GetExpectedOriginForVariety(VegetableVariety variety)
    {
        switch (variety)
        {
            case VegetableVariety.SunnyCherry: return OriginLocation.SunnyvaleFarm;
            case VegetableVariety.BeefsteakRed: return OriginLocation.MapleGroveOrchards;
            case VegetableVariety.GoldenVine: return OriginLocation.RedwoodGreenhouses;
            case VegetableVariety.SnowShallot: return OriginLocation.BlueRiverRanch;
            case VegetableVariety.HoneyBulb: return OriginLocation.GoldenFieldsEstate;
            case VegetableVariety.PhoenixCrimson: return OriginLocation.SilverstoneHollow;
            case VegetableVariety.Bellarose: return OriginLocation.HarvestHillHomestead;
            case VegetableVariety.LadyCream: return OriginLocation.EmeraldValleyFarms;
            case VegetableVariety.Rosara: return OriginLocation.WindmillCreekPlantation;
            case VegetableVariety.PicklerGuest: return OriginLocation.HarvestMoonGardens;
            case VegetableVariety.PicadorFresh: return OriginLocation.SunnyvaleFarm;
            case VegetableVariety.HermanRough: return OriginLocation.MapleGroveOrchards;
            case VegetableVariety.NantesShine: return OriginLocation.RedwoodGreenhouses;
            case VegetableVariety.ChantenOrange: return OriginLocation.BlueRiverRanch;
            case VegetableVariety.RubyTriangle: return OriginLocation.GoldenFieldsEstate;
            case VegetableVariety.PrinceLilac: return OriginLocation.SilverstoneHollow;
            case VegetableVariety.AuroraDark: return OriginLocation.HarvestHillHomestead;
            case VegetableVariety.GladiatorViolet: return OriginLocation.EmeraldValleyFarms;
            default: return OriginLocation.SunnyvaleFarm;
        }
    }
    
    // Получить отображаемое имя сорта
    private string GetVarietyDisplayName(VegetableVariety variety)
    {
        return variety.ToString().Replace("_", " ");
    }
    
    // Получить отображаемое имя места происхождения
    private string GetOriginDisplayName(OriginLocation origin)
    {
        return origin.ToString().Replace("_", " ");
    }
    
    // Проверить, просрочен ли срок годности
    private bool IsExpiryDateExpired(CharInfo charInfo)
    {
        if (charInfo == null) return false;
        
        // Получаем игровую дату из GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            return gameManager.IsExpiryDateExpired(charInfo.expiryMonth, charInfo.expiryDay);
        }
        
        // Fallback на реальную дату если GameManager не найден
        System.DateTime currentDate = System.DateTime.Now;
        System.DateTime expiryDate = new System.DateTime(currentDate.Year, charInfo.expiryMonth, charInfo.expiryDay);
        
        // Если дата срока годности раньше или равна текущей дате, то продукт просрочен
        return expiryDate <= currentDate;
    }
    
    // Получить полную текстовую информацию о персонаже
    public string GetFullTextInfo()
    {
        if (characterInfo == null)
        {
            return "Character information unavailable";
        }
        
        StringBuilder info = new System.Text.StringBuilder();
        
        // Имя овоща
        info.AppendLine($"Name: {characterInfo.name}");
        
        // Срок годности
        info.AppendLine($"Expiry Date: {characterInfo.GetExpiryDateString()}");
        
        // Сорт овоща
        info.AppendLine($"Variety: {characterInfo.GetVarietyDisplayName()}");
        
        // Ферма производства
        info.AppendLine($"Origin: {characterInfo.GetOriginDisplayName()}");
        
        // Ошибки в данных (если есть)
        // List<string> errors = GetCharacterErrors();
        // if (errors.Count > 0)
        // {
        //     info.AppendLine("Data Errors:");
        //     foreach (string error in errors)
        //     {
        //         info.AppendLine($"  - {error}");
        //     }
        // }
        
        return info.ToString();
    }
} 
using System.Collections.Generic;
using UnityEngine;

// Enum для мест происхождения
public enum OriginLocation
{
    SunnyvaleFarm,
    MapleGroveOrchards,
    RedwoodGreenhouses,
    BlueRiverRanch,
    GoldenFieldsEstate,
    SilverstoneHollow,
    HarvestHillHomestead,
    EmeraldValleyFarms,
    WindmillCreekPlantation,
    HarvestMoonGardens
}

// Enum для сортов овощей
public enum VegetableVariety
{
    // Помидоры
    SunnyCherry,
    BeefsteakRed,
    GoldenVine,
    
    // Лук
    SnowShallot,
    HoneyBulb,
    PhoenixCrimson,
    
    // Картошка
    Bellarose,
    LadyCream,
    Rosara,
    
    // Огурцы
    PicklerGuest,
    PicadorFresh,
    HermanRough,
    
    // Морковь
    NantesShine,
    ChantenOrange,
    RubyTriangle,
    
    // Баклажаны
    PrinceLilac,
    AuroraDark,
    GladiatorViolet
}

[System.Serializable]
public class CharInfo
{
    [Tooltip("Тип персонажа (овоща)")]
    public CharacterType characterType;
    
    [Tooltip("Сорт овоща")]
    public VegetableVariety variety;
    
    [Tooltip("Место происхождения")]
    public OriginLocation origin;
    
    [Tooltip("Имя персонажа")]
    public string name;
    
    [Tooltip("Срок годности (месяц)")]
    [Range(1, 12)]
    public int expiryMonth = 1;
    
    [Tooltip("Срок годности (день)")]
    [Range(1, 31)]
    public int expiryDay = 1;
    
    [Tooltip("Дефекты персонажа")]
    public List<CharacterDefect> defects = new List<CharacterDefect>();
    
    // Основной конструктор с логикой ошибок
    public CharInfo(CharacterType type, bool isMistake, bool isDefect, System.DateTime currDate)
    {
        characterType = type;
        defects = new List<CharacterDefect>();
        
        // Обрабатываем дефекты
        if (isDefect)
        {
            // Выбираем случайный дефект (не None)
            CharacterDefect[] allDefects = { 
                CharacterDefect.Bite, CharacterDefect.Bruise, CharacterDefect.Mold, 
                CharacterDefect.Parasites, CharacterDefect.Rot, CharacterDefect.Spot 
            };
            defects.Add(allDefects[Random.Range(0, allDefects.Length)]);
        }
        else
        {
            // Нет дефектов
            defects.Add(CharacterDefect.None);
        }
        
        if (!isMistake)
        {
            // Правильные данные
            GenerateCorrectData(currDate);
        }
        else
        {
            // Данные с ошибками
            GenerateMistakeData(currDate);
        }
    }
    
    // Метод для получения срока годности в виде строки
    public string GetExpiryDateString()
    {
        return $"{expiryMonth:D2}/{expiryDay:D2}";
    }
    
    // Метод для проверки, есть ли дефекты
    public bool HasDefects()
    {
        return defects != null && defects.Count > 0 && defects[0] != CharacterDefect.None;
    }
    
    // Метод для добавления дефекта
    public void AddDefect(CharacterDefect defect)
    {
        if (defects == null)
        {
            defects = new List<CharacterDefect>();
        }
        
        if (!defects.Contains(defect))
        {
            defects.Add(defect);
        }
    }
    
    // Метод для удаления дефекта
    public void RemoveDefect(CharacterDefect defect)
    {
        if (defects != null)
        {
            defects.Remove(defect);
        }
    }
    
    // Метод для получения названия сорта в читаемом виде
    public string GetVarietyDisplayName()
    {
        switch (variety)
        {
            // Помидоры
            case VegetableVariety.SunnyCherry: return "Sunny Cherry";
            case VegetableVariety.BeefsteakRed: return "Beefsteak Red";
            case VegetableVariety.GoldenVine: return "Golden Vine";
            
            // Лук
            case VegetableVariety.SnowShallot: return "Snow Shallot";
            case VegetableVariety.HoneyBulb: return "Honey Bulb";
            case VegetableVariety.PhoenixCrimson: return "Phoenix Crimson";
            
            // Картошка
            case VegetableVariety.Bellarose: return "Bellarose";
            case VegetableVariety.LadyCream: return "Lady Cream";
            case VegetableVariety.Rosara: return "Rosara";
            
            // Огурцы
            case VegetableVariety.PicklerGuest: return "Pickler Guest";
            case VegetableVariety.PicadorFresh: return "Picador Fresh";
            case VegetableVariety.HermanRough: return "Herman Rough";
            
            // Морковь
            case VegetableVariety.NantesShine: return "Nantes Shine";
            case VegetableVariety.ChantenOrange: return "Chanten Orange";
            case VegetableVariety.RubyTriangle: return "Ruby Triangle";
            
            // Баклажаны
            case VegetableVariety.PrinceLilac: return "Prince Lilac";
            case VegetableVariety.AuroraDark: return "Aurora Dark";
            case VegetableVariety.GladiatorViolet: return "Gladiator Violet";
            
            default: return variety.ToString();
        }
    }
    
    // Метод для проверки, соответствует ли сорт типу овоща
    public bool IsVarietyValidForType()
    {
        switch (characterType)
        {
            case CharacterType.Tomato:
                return variety == VegetableVariety.SunnyCherry || 
                       variety == VegetableVariety.BeefsteakRed || 
                       variety == VegetableVariety.GoldenVine;
            
            case CharacterType.Onion:
                return variety == VegetableVariety.SnowShallot || 
                       variety == VegetableVariety.HoneyBulb || 
                       variety == VegetableVariety.PhoenixCrimson;
            
            case CharacterType.Potato:
                return variety == VegetableVariety.Bellarose || 
                       variety == VegetableVariety.LadyCream || 
                       variety == VegetableVariety.Rosara;
            
            case CharacterType.Cucumber:
                return variety == VegetableVariety.PicklerGuest || 
                       variety == VegetableVariety.PicadorFresh || 
                       variety == VegetableVariety.HermanRough;
            
            case CharacterType.Carrot:
                return variety == VegetableVariety.NantesShine || 
                       variety == VegetableVariety.ChantenOrange || 
                       variety == VegetableVariety.RubyTriangle;
            
            case CharacterType.Eggplant:
                return variety == VegetableVariety.PrinceLilac || 
                       variety == VegetableVariety.AuroraDark || 
                       variety == VegetableVariety.GladiatorViolet;
            
            default:
                return false;
        }
    }
    
    // Метод для получения названия места происхождения в читаемом виде
    public string GetOriginDisplayName()
    {
        switch (origin)
        {
            case OriginLocation.SunnyvaleFarm: return "Sunnyvale Farm";
            case OriginLocation.MapleGroveOrchards: return "Maple Grove Orchards";
            case OriginLocation.RedwoodGreenhouses: return "Redwood Greenhouses";
            case OriginLocation.BlueRiverRanch: return "Blue River Ranch";
            case OriginLocation.GoldenFieldsEstate: return "Golden Fields Estate";
            case OriginLocation.SilverstoneHollow: return "Silverstone Hollow";
            case OriginLocation.HarvestHillHomestead: return "Harvest Hill Homestead";
            case OriginLocation.EmeraldValleyFarms: return "Emerald Valley Farms";
            case OriginLocation.WindmillCreekPlantation: return "Windmill Creek Plantation";
            case OriginLocation.HarvestMoonGardens: return "Harvest Moon Gardens";
            default: return origin.ToString();
        }
    }
    
    // Метод для проверки, соответствует ли сорт месту происхождения
    public bool IsVarietyValidForOrigin()
    {
        switch (origin)
        {
            case OriginLocation.SunnyvaleFarm:
                return variety == VegetableVariety.SunnyCherry || variety == VegetableVariety.BeefsteakRed;
            
            case OriginLocation.MapleGroveOrchards:
                return variety == VegetableVariety.GoldenVine || variety == VegetableVariety.SnowShallot;
            
            case OriginLocation.RedwoodGreenhouses:
                return variety == VegetableVariety.HoneyBulb || variety == VegetableVariety.PhoenixCrimson;
            
            case OriginLocation.BlueRiverRanch:
                return variety == VegetableVariety.Bellarose || variety == VegetableVariety.LadyCream;
            
            case OriginLocation.GoldenFieldsEstate:
                return variety == VegetableVariety.Rosara || variety == VegetableVariety.PicklerGuest;
            
            case OriginLocation.SilverstoneHollow:
                return variety == VegetableVariety.PicadorFresh || variety == VegetableVariety.HermanRough;
            
            case OriginLocation.HarvestHillHomestead:
                return variety == VegetableVariety.NantesShine || variety == VegetableVariety.ChantenOrange;
            
            case OriginLocation.EmeraldValleyFarms:
                return variety == VegetableVariety.RubyTriangle || variety == VegetableVariety.PrinceLilac;
            
            case OriginLocation.WindmillCreekPlantation:
                return variety == VegetableVariety.AuroraDark;
            
            case OriginLocation.HarvestMoonGardens:
                return variety == VegetableVariety.GladiatorViolet;
            
            default:
                return false;
        }
    }
    
    // Метод для получения случайного сорта для места происхождения
    public static VegetableVariety GetRandomVarietyForOrigin(OriginLocation origin)
    {
        switch (origin)
        {
            case OriginLocation.SunnyvaleFarm:
                VegetableVariety[] sunnyvaleVarieties = { VegetableVariety.SunnyCherry, VegetableVariety.BeefsteakRed };
                return sunnyvaleVarieties[Random.Range(0, sunnyvaleVarieties.Length)];
            
            case OriginLocation.MapleGroveOrchards:
                VegetableVariety[] mapleVarieties = { VegetableVariety.GoldenVine, VegetableVariety.SnowShallot };
                return mapleVarieties[Random.Range(0, mapleVarieties.Length)];
            
            case OriginLocation.RedwoodGreenhouses:
                VegetableVariety[] redwoodVarieties = { VegetableVariety.HoneyBulb, VegetableVariety.PhoenixCrimson };
                return redwoodVarieties[Random.Range(0, redwoodVarieties.Length)];
            
            case OriginLocation.BlueRiverRanch:
                VegetableVariety[] blueRiverVarieties = { VegetableVariety.Bellarose, VegetableVariety.LadyCream };
                return blueRiverVarieties[Random.Range(0, blueRiverVarieties.Length)];
            
            case OriginLocation.GoldenFieldsEstate:
                VegetableVariety[] goldenFieldsVarieties = { VegetableVariety.Rosara, VegetableVariety.PicklerGuest };
                return goldenFieldsVarieties[Random.Range(0, goldenFieldsVarieties.Length)];
            
            case OriginLocation.SilverstoneHollow:
                VegetableVariety[] silverstoneVarieties = { VegetableVariety.PicadorFresh, VegetableVariety.HermanRough };
                return silverstoneVarieties[Random.Range(0, silverstoneVarieties.Length)];
            
            case OriginLocation.HarvestHillHomestead:
                VegetableVariety[] harvestHillVarieties = { VegetableVariety.NantesShine, VegetableVariety.ChantenOrange };
                return harvestHillVarieties[Random.Range(0, harvestHillVarieties.Length)];
            
            case OriginLocation.EmeraldValleyFarms:
                VegetableVariety[] emeraldValleyVarieties = { VegetableVariety.RubyTriangle, VegetableVariety.PrinceLilac };
                return emeraldValleyVarieties[Random.Range(0, emeraldValleyVarieties.Length)];
            
            case OriginLocation.WindmillCreekPlantation:
                return VegetableVariety.AuroraDark;
            
            case OriginLocation.HarvestMoonGardens:
                return VegetableVariety.GladiatorViolet;
            
            default:
                return VegetableVariety.SunnyCherry; // Fallback
        }
    }
    
    // Метод для получения случайного места происхождения для сорта
    public static OriginLocation GetOriginForVariety(VegetableVariety variety)
    {
        switch (variety)
        {
            case VegetableVariety.SunnyCherry:
            case VegetableVariety.BeefsteakRed:
                return OriginLocation.SunnyvaleFarm;
            
            case VegetableVariety.GoldenVine:
            case VegetableVariety.SnowShallot:
                return OriginLocation.MapleGroveOrchards;
            
            case VegetableVariety.HoneyBulb:
            case VegetableVariety.PhoenixCrimson:
                return OriginLocation.RedwoodGreenhouses;
            
            case VegetableVariety.Bellarose:
            case VegetableVariety.LadyCream:
                return OriginLocation.BlueRiverRanch;
            
            case VegetableVariety.Rosara:
            case VegetableVariety.PicklerGuest:
                return OriginLocation.GoldenFieldsEstate;
            
            case VegetableVariety.PicadorFresh:
            case VegetableVariety.HermanRough:
                return OriginLocation.SilverstoneHollow;
            
            case VegetableVariety.NantesShine:
            case VegetableVariety.ChantenOrange:
                return OriginLocation.HarvestHillHomestead;
            
            case VegetableVariety.RubyTriangle:
            case VegetableVariety.PrinceLilac:
                return OriginLocation.EmeraldValleyFarms;
            
            case VegetableVariety.AuroraDark:
                return OriginLocation.WindmillCreekPlantation;
            
            case VegetableVariety.GladiatorViolet:
                return OriginLocation.HarvestMoonGardens;
            
            default:
                return OriginLocation.SunnyvaleFarm; // Fallback
        }
    }
    
    // Генерация правильных данных
    private void GenerateCorrectData(System.DateTime currDate)
    {
        // Правильное имя
        name = GetCharacterName(characterType);
        
        // Правильный сорт для типа
        variety = GetRandomVarietyForType(characterType);
        
        // Правильное место происхождения для сорта
        origin = GetOriginForVariety(variety);
        
        // Правильная дата (не превышает текущую)
        GenerateValidExpiryDate(currDate);
    }
    
    // Генерация данных с ошибками
    private void GenerateMistakeData(System.DateTime currDate)
    {
        // Случайно выбираем, где делать ошибки (50/50 для каждого поля)
        bool wrongName = Random.Range(0, 2) == 0;
        bool wrongVariety = Random.Range(0, 2) == 0;
        bool wrongOrigin = Random.Range(0, 2) == 0;
        bool wrongDate = Random.Range(0, 2) == 0;
        
        // Имя
        if (wrongName)
        {
            // Выбираем случайное неправильное имя
            CharacterType[] allTypes = { CharacterType.Tomato, CharacterType.Onion, CharacterType.Potato, 
                                       CharacterType.Cucumber, CharacterType.Carrot, CharacterType.Eggplant };
            CharacterType wrongType = allTypes[Random.Range(0, allTypes.Length)];
            name = GetCharacterName(wrongType);
        }
        else
        {
            name = GetCharacterName(characterType);
        }
        
        // Сорт
        if (wrongVariety)
        {
            // Выбираем случайный неправильный сорт
            VegetableVariety[] allVarieties = (VegetableVariety[])System.Enum.GetValues(typeof(VegetableVariety));
            variety = allVarieties[Random.Range(0, allVarieties.Length)];
        }
        else
        {
            variety = GetRandomVarietyForType(characterType);
        }
        
        // Место происхождения
        if (wrongOrigin)
        {
            // Выбираем случайное неправильное место
            OriginLocation[] allOrigins = (OriginLocation[])System.Enum.GetValues(typeof(OriginLocation));
            origin = allOrigins[Random.Range(0, allOrigins.Length)];
        }
        else
        {
            origin = GetOriginForVariety(variety);
        }
        
        // Дата
        if (wrongDate)
        {
            // Генерируем неправильную дату (может быть просроченной)
            GenerateInvalidExpiryDate(currDate);
        }
        else
        {
            GenerateValidExpiryDate(currDate);
        }
    }
    
    // Получение имени персонажа по типу
    private string GetCharacterName(CharacterType type)
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
    
    // Генерация правильной даты (не превышает текущую)
    private void GenerateValidExpiryDate(System.DateTime currDate)
    {
        // Генерируем дату в пределах 1-12 месяцев от текущей
        int monthsAhead = Random.Range(1, 13);
        System.DateTime expiryDate = currDate.AddMonths(monthsAhead);
        
        expiryMonth = expiryDate.Month;
        expiryDay = expiryDate.Day;
    }
    
    // Генерация неправильной даты (может быть просроченной)
    private void GenerateInvalidExpiryDate(System.DateTime currDate)
    {
        // 50% шанс просроченной даты
        if (Random.Range(0, 2) == 0)
        {
            // Просроченная дата (прошлая)
            int monthsBack = Random.Range(1, 13);
            System.DateTime expiredDate = currDate.AddMonths(-monthsBack);
            
            expiryMonth = expiredDate.Month;
            expiryDay = expiredDate.Day;
        }
        else
        {
            // Слишком далекая дата (больше года)
            int monthsAhead = Random.Range(13, 25);
            System.DateTime farDate = currDate.AddMonths(monthsAhead);
            
            expiryMonth = farDate.Month;
            expiryDay = farDate.Day;
        }
    }
    
    // Метод для получения случайного сорта для типа овоща
    public static VegetableVariety GetRandomVarietyForType(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Tomato:
                VegetableVariety[] tomatoVarieties = { VegetableVariety.SunnyCherry, VegetableVariety.BeefsteakRed, VegetableVariety.GoldenVine };
                return tomatoVarieties[Random.Range(0, tomatoVarieties.Length)];
            
            case CharacterType.Onion:
                VegetableVariety[] onionVarieties = { VegetableVariety.SnowShallot, VegetableVariety.HoneyBulb, VegetableVariety.PhoenixCrimson };
                return onionVarieties[Random.Range(0, onionVarieties.Length)];
            
            case CharacterType.Potato:
                VegetableVariety[] potatoVarieties = { VegetableVariety.Bellarose, VegetableVariety.LadyCream, VegetableVariety.Rosara };
                return potatoVarieties[Random.Range(0, potatoVarieties.Length)];
            
            case CharacterType.Cucumber:
                VegetableVariety[] cucumberVarieties = { VegetableVariety.PicklerGuest, VegetableVariety.PicadorFresh, VegetableVariety.HermanRough };
                return cucumberVarieties[Random.Range(0, cucumberVarieties.Length)];
            
            case CharacterType.Carrot:
                VegetableVariety[] carrotVarieties = { VegetableVariety.NantesShine, VegetableVariety.ChantenOrange, VegetableVariety.RubyTriangle };
                return carrotVarieties[Random.Range(0, carrotVarieties.Length)];
            
            case CharacterType.Eggplant:
                VegetableVariety[] eggplantVarieties = { VegetableVariety.PrinceLilac, VegetableVariety.AuroraDark, VegetableVariety.GladiatorViolet };
                return eggplantVarieties[Random.Range(0, eggplantVarieties.Length)];
            
            default:
                return VegetableVariety.SunnyCherry; // Fallback
        }
    }
} 
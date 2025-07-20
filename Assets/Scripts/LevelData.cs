using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    [Header("Рецепты уровня")]
    [Tooltip("Рецепты для этого уровня")]
    public List<RecipeData> recipes = new List<RecipeData>();
    
    // Конструктор для создания уровня
    public LevelData(List<RecipeData> levelRecipes)
    {
        recipes = new List<RecipeData>(levelRecipes);
    }
    
    // Получить количество рецептов в уровне
    public int GetRecipeCount()
    {
        return recipes != null ? recipes.Count : 0;
    }
    
    // Проверить, есть ли рецепты в уровне
    public bool HasRecipes()
    {
        return recipes != null && recipes.Count > 0;
    }
    
    // Получить информацию об уровне
    public string GetLevelInfo()
    {
        return $"Рецептов: {GetRecipeCount()}";
    }
} 
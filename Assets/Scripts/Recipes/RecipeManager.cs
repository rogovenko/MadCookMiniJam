using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [Header("Настройки менеджера рецептов")]
    [Tooltip("Все доступные рецепты в игре")]
    [SerializeField] private List<RecipeData> allRecipes = new List<RecipeData>();
    
    private Dictionary<string, RecipeData> recipesByName = new Dictionary<string, RecipeData>();
    
    void Start()
    {
        InitializeRecipeDictionary();
    }
    
    // Инициализация словаря рецептов для быстрого поиска
    private void InitializeRecipeDictionary()
    {
        recipesByName.Clear();
        
        // Добавляем все рецепты в словарь
        foreach (RecipeData recipe in allRecipes)
        {
            if (recipe != null && !string.IsNullOrEmpty(recipe.recipeName))
            {
                recipesByName[recipe.recipeName] = recipe;
            }
        }
        
        Debug.Log($"RecipeManager: Инициализировано {recipesByName.Count} рецептов");
    }
    
    // Получить рецепт по названию
    public RecipeData GetRecipeByName(string recipeName)
    {
        if (recipesByName.ContainsKey(recipeName))
        {
            return recipesByName[recipeName];
        }
        
        Debug.LogWarning($"RecipeManager: Рецепт '{recipeName}' не найден!");
        return null;
    }
    
    // Получить все рецепты
    public List<RecipeData> GetAllRecipes()
    {
        return new List<RecipeData>(allRecipes);
    }
    
    // Получить случайный рецепт
    public RecipeData GetRandomRecipe()
    {
        if (allRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, allRecipes.Count);
            return allRecipes[randomIndex];
        }
        
        Debug.LogWarning("RecipeManager: Нет доступных рецептов!");
        return null;
    }
    
    // Найти рецепты, которые можно приготовить с доступными ингредиентами
    public List<RecipeData> GetAvailableRecipes(Dictionary<CharacterType, int> availableIngredients)
    {
        List<RecipeData> availableRecipes = new List<RecipeData>();
        
        foreach (RecipeData recipe in allRecipes)
        {
            if (recipe != null && recipe.HasAllIngredients(availableIngredients))
            {
                availableRecipes.Add(recipe);
            }
        }
        
        return availableRecipes;
    }
    
    // Добавить рецепт в менеджер
    public void AddRecipe(RecipeData recipe)
    {
        if (recipe != null && !allRecipes.Contains(recipe))
        {
            allRecipes.Add(recipe);
            
            if (!string.IsNullOrEmpty(recipe.recipeName))
            {
                recipesByName[recipe.recipeName] = recipe;
            }
            
            Debug.Log($"RecipeManager: Добавлен рецепт '{recipe.recipeName}'");
        }
    }
    
    // Удалить рецепт из менеджера
    public void RemoveRecipe(RecipeData recipe)
    {
        if (recipe != null && allRecipes.Contains(recipe))
        {
            allRecipes.Remove(recipe);
            
            if (!string.IsNullOrEmpty(recipe.recipeName) && recipesByName.ContainsKey(recipe.recipeName))
            {
                recipesByName.Remove(recipe.recipeName);
            }
            
            Debug.Log($"RecipeManager: Удален рецепт '{recipe.recipeName}'");
        }
    }
    
    // Получить количество рецептов
    public int GetRecipeCount()
    {
        return allRecipes.Count;
    }
} 
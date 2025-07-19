using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RecipeIngredient
{
    [Tooltip("Тип ингредиента")]
    public CharacterType ingredientType;
    
    [Tooltip("Количество ингредиента")]
    [Range(1, 10)]
    public int amount = 1;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Основная информация")]
    [Tooltip("Название рецепта")]
    public string recipeName = "Новый рецепт";
    
    [Header("Ингредиенты")]
    [Tooltip("Список ингредиентов для рецепта")]
    public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    
    // Метод для проверки, есть ли все необходимые ингредиенты
    public bool HasAllIngredients(Dictionary<CharacterType, int> availableIngredients)
    {
        foreach (RecipeIngredient ingredient in ingredients)
        {
            if (!availableIngredients.ContainsKey(ingredient.ingredientType) ||
                availableIngredients[ingredient.ingredientType] < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }
    
    // Метод для получения списка ингредиентов в виде строки
    public string GetIngredientsList()
    {
        string ingredientsList = "";
        for (int i = 0; i < ingredients.Count; i++)
        {
            RecipeIngredient ingredient = ingredients[i];
            ingredientsList += $"{ingredient.ingredientType} x{ingredient.amount}";
            
            if (i < ingredients.Count - 1)
            {
                ingredientsList += ", ";
            }
        }
        return ingredientsList;
    }
} 
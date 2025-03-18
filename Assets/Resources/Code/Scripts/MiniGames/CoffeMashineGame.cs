using System.Collections.Generic;
using UnityEngine;

public class CoffeMashineGame : MiniGame
{
    [SerializeField] PlayerManager playerManager;
    private Stack<Ingredient> ingredientStuck = new Stack<Ingredient>();

    public delegate void OnUIEffectEnd(bool success);
    public event OnUIEffectEnd UIEffectEnd;

    public void AddItemInStuck(Ingredient ingredient)
    {
        if (ingredientStuck.Count >= 3)
        {
            Debug.Log("Стек заполнен! Максимум 3 ингредиента.");
            return;
        }

        Debug.Log("Ингредиент добавлен!");
        ingredientStuck.Push(ingredient);
    }

    public void RemoveItemFromStuck()
    {
        if (ingredientStuck.Count > 0)
            ingredientStuck.Pop();
        Debug.Log($"Всего ингредиентов {ingredientStuck.Count}");
    }

    public void CheckRightCombination()
    {
        if (ingredientStuck.Contains(Ingredient.Bone) &&
            ingredientStuck.Contains(Ingredient.Shrimp) &&
            ingredientStuck.Contains(Ingredient.Granat))
        {
            playerManager.RemoveUIEffect();
            UIEffectEnd?.Invoke(true);
            Debug.Log("Успешно сварено кофе!");
            
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            playerManager.UpdateUIEffect(10);
            Debug.Log("Успешно сварена отрава!");
        }

        ingredientStuck.Clear();
    }

    public void OnBoneButtonClicked()
    {
        AddItemInStuck(Ingredient.Bone);
    }

    public void OnShrimpButtonClicked()
    {
        AddItemInStuck(Ingredient.Shrimp);
    }

    public void OnGranatButtonClicked()
    {
        AddItemInStuck(Ingredient.Granat);
    }

    public void OnOtherButtonClicked(string nameButton)
    {
        switch(nameButton)
        {
            case "Fugu":
                AddItemInStuck(Ingredient.Fugu);
                break;
            case "Coffe":
                AddItemInStuck(Ingredient.Coffe);
                break;
            case "Eye":
                AddItemInStuck(Ingredient.Eye);
                break;
            case "Alga":
                AddItemInStuck(Ingredient.Alga);
                break;
            case "Beer":
                AddItemInStuck(Ingredient.Beer);
                break;
            case "Corn":
                AddItemInStuck(Ingredient.Corn);
                break;
            case "Ice":
                AddItemInStuck(Ingredient.Ice);
                break;
            case "Oil":
                AddItemInStuck(Ingredient.Oil);
                break;
            case "Mint":
                AddItemInStuck(Ingredient.Mint);
                break;
            case "Onion":
                AddItemInStuck(Ingredient.Onion);
                break;
            case "Root":
                AddItemInStuck(Ingredient.Root);
                break;
            case "Tea":
                AddItemInStuck(Ingredient.Tea);
                break;
            default:
                break;
        }
    }
}

public enum Ingredient
{
    Fugu = 0,
    Coffe = 1,
    Eye = 2,
    Alga = 3,
    Beer = 4,
    Corn = 5,
    Ice = 6,
    Bone = 7,
    Oil = 8,
    Mint = 9,
    Onion = 10,
    Root = 11,
    Shrimp = 12,
    Tea = 13,
    Granat = 14,
}

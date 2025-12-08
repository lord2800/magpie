namespace MagpieTest.Factories;

using Magpie.Data;
using System.Collections.Generic;

internal static class RecipeFactory
{
    public static Recipe New(uint id = 1, string name = "", List<RecipeIngredient> ingredients = null)
        => new(Id: id, Name: name, Ingredients: ingredients ?? []);
}

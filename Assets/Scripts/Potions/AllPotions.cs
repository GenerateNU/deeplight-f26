using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Asset holding every possible potion definition. Lives in Resources so it can be
/// reached from anywhere, and persists across scenes because it's an asset.
/// </summary>
[CreateAssetMenu(fileName = "AllPotions", menuName = "Potions/All Potions")]
public class AllPotions : ScriptableObject
{
    private static AllPotions instance;

    public static AllPotions Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<AllPotions>("AllPotions");
                if (instance == null)
                {
                    Debug.LogError("No AllPotions asset found at Resources/AllPotions.");
                }
            }
            return instance;
        }
    }

    [Header("All Potions")]
    [SerializeField] private List<Potion> allPotions = new List<Potion>();

    /// <summary>
    /// Returns allPotions list
    /// </summary>
    /// <
    public List<Potion> GetAllPotions() => allPotions;

    /// <summary>
    /// Finds the matching potion definition, or null if there isn't one.
    /// The returned object is the shared definition — do not roll or mutate it.
    /// </summary>
    public Potion GetPotionFromList(Stat stat, Rarity rarity, Operation operation)
    {
        foreach (Potion potion in allPotions)
        {
            if (potion.GetStat() == stat
                && potion.GetRarity() == rarity
                && potion.GetOperation() == operation)
            {
                return potion;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the matching definition and returns a fresh rolled copy ready to be granted.
    /// </summary>
    public Potion CreatePotion(Stat stat, Rarity rarity, Operation operation)
    {
        Potion definition = GetPotionFromList(stat, rarity, operation);
        return definition != null ? definition.CreateRolledInstance() : null;
    }
}

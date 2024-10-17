using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dice_Manager : MonoBehaviour
{
    public static Dice_Manager Instance { get; private set; }

    public List<GameObject> dice_prefabs;
    private List<string> dice_names = new List<string> { "d4", "d6", "d8", "d10", "d12", "d20" };

    public Dictionary<string, GameObject> dice_dictionary;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        foreach (GameObject die in dice_prefabs)
        {
            if (die != null)
                dice_dictionary.Add(die.name, die);
        }
    }

    // Rolls a specified number of dice of each type (xd4 + yd6 + zd8... + modifier) and returns the sum
    int DiceSum(Dictionary<string, int> total_dice, int total_mod)
    {
        int dice_sum = 0;

        foreach (KeyValuePair<string, int> entry in total_dice) 
        {
            for (int i = 0; i < entry.Value; i++)
                dice_sum += ThrowDie(entry.Key);
        }

        return dice_sum;
    }


    // Rolls the specified number of dice of each type against an equal number, and returns the victor
    Object DiceContest(Object challenger, Dictionary<string, int> challenger_dice, int challenger_mod, 
                       Object defender, Dictionary<string, int> defender_dice, int defender_mod)
    {
        int challenger_total = DiceSum(challenger_dice, challenger_mod);
        int defender_total = DiceSum(defender_dice, defender_mod);

        if (challenger_total >= defender_total)
            return challenger;
        else
            return defender;
    }

    // Rolls the specified number of dice of each type against a static value, and returns the total and whether it was sucessful
    bool DiceCheck(Dictionary<string, int> total_dice, int total_mod, int difficulty)
    {
        if (DiceSum(total_dice, total_mod) >= difficulty)
            return true;
        else 
            return false;
    }


    // Rolls the specified number of dice of each type against a table, and returns the object at the index specified
    Object DiceTable(Object[] table, Dictionary<string, int> total_dice, int total_mod, bool indexTotal_isRollTotal, bool indexTotal_isTotalClamped)
    {
        int table_length = table.Length - 1;
        int index;

        if (indexTotal_isRollTotal)
            index = DiceSum(total_dice, total_mod);
        else
        {
            index = 0;

            foreach (KeyValuePair<string, int> entry in total_dice)
            {
                for (int i = 0; i < entry.Value; i++)
                    index += ThrowDie(entry.Key) * ((int) Mathf.Pow(10, i));
            }
        }

        if (indexTotal_isTotalClamped)
            return table[Mathf.Clamp(index, 0, table_length)];
        else
            return table[index % (table_length)];
    }


    int ThrowDie(string die_name)
    {
        GameObject die = (GameObject)Instantiate(dice_dictionary[die_name], new Vector3(0, 3, 0), Quaternion.identity);
        Dice die_script;

        die_script = die.GetComponentInChildren<Dice>();
        die_script.ThrowDice(new Vector3(0, .5f, 1), 5);

        // NEED TO GO INTO THE DICE SCRIPT AND TURN IT INTO AN EVENT THAT IS LISTENED TO HERE.

        return -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dice_Manager : MonoBehaviour
{
    public static Dice_Manager Instance { get; private set; }

    public List<GameObject> dice_prefabs;
    private List<string> dice_names = new List<string> { "d4", "d6", "d8", "d10", "d12", "d20" };
    public Dictionary<string, GameObject> dice_dictionary;

    private Dictionary<Dice, int> dice_activeList;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Dice.onDiceRoll += WaitOnDie;
    }

    private void OnDisable()
    {
        Dice.onDiceRoll -= WaitOnDie;
    }

    void Start()
    {
        dice_dictionary = new Dictionary<string, GameObject>();

        int dice_index = 0;
        foreach (GameObject die in dice_prefabs)
        {
            if (die != null)
                dice_dictionary.Add(dice_names[dice_index], die);

            dice_index++;
        }

        dice_activeList = new Dictionary<Dice, int>();
    }

    // Rolls a specified number of dice of each type (xd4 + yd6 + zd8... + modifier) and returns the sum
    public async Task<int> DiceSum(Dictionary<string, int> total_dice, int total_mod)
    {
        int dice_sum = 0;

        var dice_queue = new List<Task<int>>();
        foreach (KeyValuePair<string, int> entry in total_dice) 
        {
            for (int i = 0; i < entry.Value; i++)
                dice_queue.Add(ThrowDie(entry.Key));
        }

        await Task.WhenAll(dice_queue);

        foreach (var die_result in dice_queue)
            dice_sum += await die_result;

        return dice_sum + total_mod;
    }


    // Rolls the specified number of dice of each type against an equal number, and returns the victor
    public async Task<Object> DiceContest(Object challenger, Dictionary<string, int> challenger_dice, int challenger_mod, 
                                   Object defender, Dictionary<string, int> defender_dice, int defender_mod)
    {
        int challenger_total = await DiceSum(challenger_dice, challenger_mod);
        int defender_total = await DiceSum(defender_dice, defender_mod);

        if (challenger_total >= defender_total)
            return challenger;
        else
            return defender;
    }

    // Rolls the specified number of dice of each type against a static value, and returns the total and whether it was sucessful
    public async Task<bool> DiceCheck(Dictionary<string, int> total_dice, int total_mod, int difficulty)
    {
        if (await DiceSum(total_dice, total_mod) >= difficulty)
            return true;
        else 
            return false;
    }


    // Rolls the specified number of dice of each type against a table, and returns the object at the index specified
    public async Task<Object> DiceTable(Object[] table, Dictionary<string, int> total_dice, int total_mod, bool indexTotal_isRollTotal, bool indexTotal_isTotalClamped)
    {
        int table_length = table.Length - 1;
        int index;

        if (indexTotal_isRollTotal)
            index = DiceSum(total_dice, total_mod).GetAwaiter().GetResult();
        else
        {
            index = 0;

            var dice_queue = new List<Task<int>>();
            foreach (KeyValuePair<string, int> entry in total_dice)
            {
                for (int i = 0; i < entry.Value; i++)
                    dice_queue.Add(ThrowDie(entry.Key));
            }

            await Task.WhenAll(dice_queue);

            int j = 0;
            foreach (var die_result in dice_queue)
            {
                index += (await die_result) * ((int)Mathf.Pow(10, j));
                j++;
            }

        }

        if (indexTotal_isTotalClamped)
            return table[Mathf.Clamp(index, 0, table_length)];
        else
            return table[index % (table_length)];
    }

    // Will roll a single die and then wait for the event invoked by that die to return the value associated with it
    private async Task<int> ThrowDie(string die_name)
    {
        Transform cam = Camera.main.transform;

        GameObject die = (GameObject) Instantiate(dice_dictionary[die_name], cam.position + (cam.forward * 3.0f), Quaternion.identity);
        Dice die_script;

        die_script = die.GetComponentInChildren<Dice>();
        die_script.ThrowDice(cam.forward, 5);

        int die_retVal = -1;

        while (!dice_activeList.ContainsKey(die_script))
            await Task.Yield();

        die_retVal = dice_activeList[die_script];
        dice_activeList.Remove(die_script);

        return die_retVal;
    }

    // Listens for any events invoked by exisiting die prefabs
    void WaitOnDie(Dice die_instance, int die_value)
    {
        dice_activeList.Add(die_instance, die_value);
    }
}

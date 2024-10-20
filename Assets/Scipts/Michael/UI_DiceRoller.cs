using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class UI_DiceRoller : MonoBehaviour
{
    private Dictionary<string, int> dice_counts;
    private int dice_total;

    public List<TextMeshProUGUI> buttons;

    // Start is called before the first frame update
    void Start()
    {
        dice_counts = new Dictionary<string, int>();
    }

    public async void PressRoll()
    {
        int dice_sum = 0;

        string output = "Rolling: ";
        foreach (KeyValuePair<string, int> entry in dice_counts)
        {
            if (entry.Value > 0)
                output += entry.Value.ToString() + entry.Key + ", ";
        }
        output = output.Substring(0, output.Length - 2);
        output += " = ";

        if (dice_total > 0)
        {
            dice_sum = await Dice_Manager.Instance.DiceSum(dice_counts, 0);
            output += dice_sum.ToString();

            Debug.Log(output);
        }

        PressCancel();
    }

    public void PressCancel()
    {
        dice_total = 0;
        dice_counts.Clear();

        foreach (TextMeshProUGUI button in buttons)
        {
            string button_suffix = GetDiceType(button);
            button.text = '0' + button_suffix.ToUpper();
        }
    }

    public void IncreaseTotal(TextMeshProUGUI button)
    {
        dice_total++;

        string dice_type = GetDiceType(button);

        if (dice_counts.ContainsKey(dice_type)) 
        {
            int die_count = dice_counts[dice_type] + 1;
            dice_counts[dice_type] = die_count;
        }
        else
            dice_counts.Add(dice_type, 1);

        button.text = dice_counts[dice_type].ToString() + dice_type.ToUpper();
    }

    public void DecreaseTotal(TextMeshProUGUI button)
    {
        dice_total--;
        if (dice_total <= 0)
            dice_total = 0;

        string dice_type = GetDiceType(button);

        if (dice_counts.ContainsKey(dice_type))
        {
            int die_count = dice_counts[dice_type] - 1;
            dice_counts[dice_type] = die_count;

            if (die_count <= 0)
            {
                dice_counts.Remove(dice_type);
                button.text = '0' + dice_type.ToUpper();
            }
            else
            {
                button.text = dice_counts[dice_type].ToString() + dice_type.ToUpper();
            }
        }
    }

    private string GetDiceType(TextMeshProUGUI button)
    {
        string button_text = button.text;
        return button_text.Substring(button_text.LastIndexOf('D')).ToLower();
    }
}

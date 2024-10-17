using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UI_DiceRoller_Anim : MonoBehaviour
{
    private Animator animator_container;
    private IEnumerable<Animator> animator_counters;
    private int totalDice;

    // Start is called before the first frame update
    void Start()
    {
        animator_container = GetComponent<Animator>();
        animator_counters = GetComponentsInChildren<Animator>().Where(anim => anim.gameObject != gameObject);
    }

    public void PressRoll()
    {
        animator_container.SetTrigger("pressRoll");
        AlterCounterState();
    }

    public void PressCancel()
    {
        animator_container.SetTrigger("pressCancel");
        AlterCounterState();
    }

    public void IncreaseTotal()
    {
        totalDice++;
        animator_container.SetInteger("diceTotal", totalDice);
    }

    public void DecreaseTotal() 
    {
        totalDice--;
        if (totalDice < 0)
            totalDice = 0;

        animator_container.SetInteger("diceTotal", totalDice);
    }

    private void AlterCounterState()
    {
        foreach (Animator animator_counter in animator_counters)
        {
            animator_counter.SetTrigger("changeState");
        }

        totalDice = 0;
        animator_container.SetInteger("diceTotal", totalDice);
    }
}

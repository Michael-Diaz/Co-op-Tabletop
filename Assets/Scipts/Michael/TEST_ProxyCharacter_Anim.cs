using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_ProxyCharacter_Anim : MonoBehaviour
{
    private Animator animator;

    private enum HeldItemState
    {
        Unarmed,
        Magic_Bard,
        Magic_Nature,
        Magic_Holy,
        Magic_Arcane,
        TwoWeaponFighting,
        TwoHanded_Slash,
        TwoHanded_Poke,
        OneHanded_Slash,
        OneHanded_Poke,
        ItemThrow_Long,
        ItemThrow_Short,
        Crossbow,
        Bow
    }

    private HeldItemState currentState;
    private int currentState_int;

    private int heldItem_totalStates;

    private bool isInPain;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        currentState = HeldItemState.Unarmed;
        currentState_int = (int) currentState;

        heldItem_totalStates = Enum.GetNames(typeof(HeldItemState)).Length;

        isInPain = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isStatechanged = false;

        if (Input.GetKeyDown(KeyCode.I))
        {
            currentState_int = Mathf.Clamp(currentState_int - 1, 0, heldItem_totalStates - 1);
            isStatechanged = true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentState_int = Mathf.Clamp(currentState_int + 1, 0, heldItem_totalStates - 1);
            isStatechanged = true;
        }

        if (isStatechanged)
        {
            currentState = (HeldItemState)currentState_int;

            animator.SetInteger("Held Item Enum", currentState_int);
            animator.SetTrigger("Switch State");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            isInPain = !isInPain;
            animator.SetBool("IsHurt", isInPain);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Attack");
        }
    }
}

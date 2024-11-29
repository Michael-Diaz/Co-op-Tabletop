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

    private bool isDeathsDoor;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        currentState = HeldItemState.Unarmed;
        currentState_int = (int) currentState;

        heldItem_totalStates = Enum.GetNames(typeof(HeldItemState)).Length;
    }

    // Update is called once per frame
    void Update()
    {
        bool isStatechanged = false;

        // Move up the list of held-item states
        if (Input.GetKeyDown(KeyCode.I))
        {
            currentState_int = (currentState_int - 1) % heldItem_totalStates;
            if (currentState_int < 0)
                currentState_int += heldItem_totalStates;

            isStatechanged = true;
        }
        // Move down the list of held-item states
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentState_int = (currentState_int + 1) % heldItem_totalStates;

            isStatechanged = true;
        }

        if (isStatechanged)
        {
            currentState = (HeldItemState)currentState_int;

            animator.SetInteger("Held Item Enum", currentState_int);
            animator.SetTrigger("Switch State");
        }

        // Have the player take physical damage
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("<color=#666600>Taking Physical Damage...</color>");
            DamagePlayer(0);
        }
        // Have the player take psychic damage
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("<color=#cc00cc>Taking Psychic Damage...</color>");
            DamagePlayer(1);
        }

        // Heal the player for one state
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("<color=#00ff00>Healing...</color>");
            if (isDead)
            {
                isDead = false;
                animator.SetBool("IsDead", false);
            }
            else if (isDeathsDoor)
            {
                isDeathsDoor = false;
                animator.SetBool("IsHurt", false);
            }
        }

        // Player attacks
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("<color=#dd0000>Attacking...</color>");
            animator.SetTrigger("Attack");
        }

        // Player casts a non-damaging spell
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Debug.Log("<color=#00ffff>Casting...</color>");
            animator.SetTrigger("Cast");
        }
    }

    private void DamagePlayer(int damageType)
    {
        animator.SetInteger("Damage Type Enum", damageType);

        if (isDeathsDoor && !isDead)
        {
            isDead = true;
            animator.SetBool("IsDead", true);
        }
        else if (!isDeathsDoor)
        {
            isDeathsDoor = true;
            animator.SetBool("IsHurt", true);
        }

        animator.SetTrigger("Take Damage");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionContainer : MonoBehaviour
{
    private UI_ActionEconomyMaker actionEconomyMaker;

    private bool action_isSelectable;
    private bool action_isSelected;
    [SerializeField] private GameObject action_indicatorContainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PostInstantiationSetup(UI_ActionEconomyMaker instance, bool selectability)
    {
        actionEconomyMaker = instance;
        action_isSelectable = selectability;
    }

    public void SelectAction()
    {
        if (action_isSelectable)
        {
            if (action_isSelected)
            {
                actionEconomyMaker.ResetActiveAction();

                action_isSelected = false;
            }
            else
            {
                actionEconomyMaker.SetActiveAction((RectTransform)gameObject.transform);

                action_isSelected = true;
            }

            action_indicatorContainer.SetActive(action_isSelected);
        }
    }

    public void DeactivateIndicator()
    {
        action_indicatorContainer.SetActive(false);
    }
}

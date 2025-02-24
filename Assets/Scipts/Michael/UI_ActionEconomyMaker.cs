using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class UI_ActionEconomyMaker : MonoBehaviour
{
    [SerializeField]
    private GameObject action_prefab;

    [SerializeField] private GameObject actionEconomyBar_interior;
    [SerializeField] private float actionEconomyBar_interiorLength;
    private List<RectTransform> actionEconomyBar_activeActions;

    [SerializeField] private GameObject actionCategoryBar_content;
    private List<RectTransform> actionCategoryBar_possibleActions;

    [SerializeField] private TMP_InputField actionCategory_textField;

    [SerializeField] // Delete the serialization, only being used for testing until the action_prefab is clickable
    private RectTransform action_selected;

    // Start is called before the first frame update
    void Start()
    {
        actionEconomyBar_activeActions = new List<RectTransform>();
        actionCategoryBar_possibleActions = new List<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateAction()
    {
        if (string.IsNullOrEmpty(actionCategory_textField.text))
        {
            return;
        }

        RectTransform action_nameMatch = actionCategoryBar_possibleActions.FirstOrDefault(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text == actionCategory_textField.text);
        if (action_nameMatch != null)
        {
            return;
        }

        GameObject action_created;

        action_created = Instantiate(action_prefab, actionCategoryBar_content.transform.position, Quaternion.identity, actionCategoryBar_content.transform);
        action_created.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = actionCategory_textField.text; // This is the text in the category text box

        actionCategory_textField.Select();
        actionCategory_textField.text = "";

        actionCategoryBar_possibleActions.Add(action_created.GetComponent<RectTransform>());
        actionCategoryBar_possibleActions = actionCategoryBar_possibleActions.OrderBy(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text).ToList();
    }

    public void AddToActionEconomy()
    {
        if (action_selected == null)
        {
            return;
        }

        GameObject action_selectedClone;

        action_selectedClone = Instantiate(action_prefab, actionEconomyBar_interior.transform.position, Quaternion.identity, actionEconomyBar_interior.transform);
        action_selectedClone.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = action_selected.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;

        actionEconomyBar_activeActions.Add(action_selectedClone.GetComponent<RectTransform>());
        actionEconomyBar_activeActions = actionEconomyBar_activeActions.OrderBy(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text).ToList();

        ResizeActiveActions();
    }

    public void RemoveFromActionEconomy()
    {
        if (action_selected == null)
        {
            return;
        }

        RectTransform action_selectedMatch = actionEconomyBar_activeActions.FirstOrDefault(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text == action_selected.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

        if (action_selectedMatch != null)
        {
            actionEconomyBar_activeActions.Remove(action_selectedMatch);
            Destroy(action_selectedMatch.gameObject);

            ResizeActiveActions();
        }
    }

    private void ResizeActiveActions()
    {
        int activeAction_count = actionEconomyBar_activeActions.Count;
        float activeAction_newLength = (actionEconomyBar_interiorLength - (2.5f * (activeAction_count - 1))) / (float) activeAction_count;
        float activeAction_newPosition = (activeAction_newLength / 2.0f) - 30.0f;

        for (int i = 0; i < activeAction_count; i++)
        {
            actionEconomyBar_activeActions[i].anchoredPosition = new Vector2(activeAction_newPosition, actionEconomyBar_activeActions[i].anchoredPosition.y);
            actionEconomyBar_activeActions[i].GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(activeAction_newLength, actionEconomyBar_activeActions[i].GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

            activeAction_newPosition += (activeAction_newLength + 2.5f);
        }
    }

    public void DeleteAction()
    {
        // Keep track of the selected action
        // Remove all instances of it from the action Econonomy Bar
        // Destroy each object
        // Destroy the selected action object
        // Turn the variable null

        if (action_selected == null)
        {
            return;
        }

        Debug.Log($"Removing action: {action_selected.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text}");

        List<RectTransform> actionEconomyBar_inactiveActions = actionEconomyBar_activeActions.Where(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text == action_selected.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text).ToList();
        actionEconomyBar_activeActions.RemoveAll(a => a.GetChild(1).GetComponent<TextMeshProUGUI>().text == action_selected.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);

        foreach (RectTransform inactiveAction in actionEconomyBar_inactiveActions)
        {
            Destroy(inactiveAction.gameObject);
        }

        Destroy(action_selected.gameObject);
        action_selected = null;

        ResizeActiveActions();
    }
}

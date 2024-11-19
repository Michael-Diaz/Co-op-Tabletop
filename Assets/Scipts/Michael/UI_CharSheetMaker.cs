using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CharSheetMaker : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_sheet;

    [Header("Page Tracker")]
    [SerializeField] private RectTransform sheet_containerActive;
    [SerializeField] private RectTransform sheet_containerInactive;

    [SerializeField] private RectTransform sheet_containerPrev;
    [SerializeField] private RectTransform sheet_containerNext;
    [SerializeField] private RectTransform sheet_page;

    private int pageNum_current;
    private int pageNum_max;

    [Header("Layout Positions")]
    [SerializeField] private float sheet_containerPrev_verticalX;
    [SerializeField] private float sheet_containerPrev_horizontalX;

    [SerializeField] private float sheet_containerNext_verticalX;
    [SerializeField] private float sheet_containerNext_horizontalX;

    [SerializeField] private float sheet_page_verticalY;
    [SerializeField] private float sheet_page_horizontalY;

    private bool isHorizontal;

    // Start is called before the first frame update
    void Start()
    {
        pageNum_current = pageNum_max = 1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PrevPage()
    {
        pageNum_current--;

        RectTransform activeSheet_old = (RectTransform)sheet_containerActive.GetChild(0);
        RectTransform activeSheet_new = (RectTransform)sheet_containerInactive.GetChild(pageNum_current - 1);

        activeSheet_old.SetParent(sheet_containerInactive);
        activeSheet_old.SetSiblingIndex(pageNum_current - 1);
        activeSheet_old.gameObject.SetActive(false);

        activeSheet_new.SetParent(sheet_containerActive);
        activeSheet_new.gameObject.SetActive(true);

        UpdateSheetDisplay();
    }

    public void NextPage() 
    {
        pageNum_current++;

        RectTransform activeSheet_old = (RectTransform)sheet_containerActive.GetChild(0);
        RectTransform activeSheet_new = (RectTransform)sheet_containerInactive.GetChild(pageNum_current - 2);

        activeSheet_old.SetParent(sheet_containerInactive);
        activeSheet_old.SetSiblingIndex(pageNum_current - 2);
        activeSheet_old.gameObject.SetActive(false);

        activeSheet_new.SetParent(sheet_containerActive);
        activeSheet_new.gameObject.SetActive(true);

        UpdateSheetDisplay();
    }

    public void AddSheet()
    {
        pageNum_current++;
        pageNum_max++;

        RectTransform activeSheet_old = (RectTransform) sheet_containerActive.GetChild(0);
        activeSheet_old.SetParent(sheet_containerInactive);
        activeSheet_old.SetSiblingIndex(pageNum_current - 2);
        activeSheet_old.gameObject.SetActive(false);

        float rotZ = isHorizontal ? -90.0f : 0.0f;
        Quaternion activeSheet_new_rot = Quaternion.Euler(0.0f, 0.0f, (rotZ));

        RectTransform activeSheet_new = Instantiate(prefab_sheet, sheet_containerActive).GetComponent<RectTransform>();
        activeSheet_new.anchoredPosition = sheet_containerActive.anchoredPosition;
        activeSheet_new.rotation = activeSheet_new_rot;
        activeSheet_new.gameObject.name = $"Page #{pageNum_current}: {activeSheet_new.gameObject.name}";

        UpdateSheetDisplay();
    }
    

    public void RemoveSheet()
    {
        if (pageNum_max == 1)
            return;
            
        Destroy(sheet_containerActive.GetChild(0).gameObject);

        pageNum_max--;
        pageNum_current = Mathf.Clamp(pageNum_current--, 1, pageNum_max);

        RectTransform activeSheet_new = (RectTransform) sheet_containerInactive.GetChild(pageNum_current - 1);
        activeSheet_new.SetParent(sheet_containerActive); 
        activeSheet_new.gameObject.SetActive(true);

        UpdateSheetDisplay();
    }
    
    private void UpdateSheetDisplay()
    {
        bool isShowing_pagePrev = false;
        bool isShowing_pageNext = false;

        if (pageNum_max > 1)
        {
            if (pageNum_current - 1 >= 1)
                isShowing_pagePrev = true;
            if (pageNum_current + 1 <= pageNum_max)
                isShowing_pageNext = true;
        }

        sheet_containerPrev.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{pageNum_current - 1}";
        sheet_containerNext.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{pageNum_current + 1}";

        sheet_containerPrev.gameObject.SetActive(isShowing_pagePrev);
        sheet_containerNext.gameObject.SetActive(isShowing_pageNext);

        sheet_page.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{pageNum_current} / {pageNum_max}";
    }

    public void SwapLayout()
    {
        isHorizontal = !isHorizontal;

        float rotZ = isHorizontal ? -90.0f : 0.0f;
        sheet_containerActive.GetChild(0).rotation = Quaternion.Euler(0.0f, 0.0f, (rotZ));

        for (int i = 0; i < sheet_containerInactive.childCount; i++)
        {
            sheet_containerInactive.GetChild(i).rotation = Quaternion.Euler(0.0f, 0.0f, (rotZ));
        }

        float newPos = isHorizontal ? sheet_containerPrev_horizontalX : sheet_containerPrev_verticalX;
        sheet_containerPrev.anchoredPosition = new Vector2(newPos, sheet_containerPrev.anchoredPosition.y);

        newPos = isHorizontal ? sheet_containerNext_horizontalX : sheet_containerNext_verticalX;
        sheet_containerNext.anchoredPosition = new Vector2(newPos, sheet_containerNext.anchoredPosition.y);

        newPos = isHorizontal ? sheet_page_horizontalY : sheet_page_verticalY;
        sheet_page.anchoredPosition = new Vector2(sheet_page.anchoredPosition.x, newPos);
    }

    public void AddSheetElement(GameObject element_newObj)
    {
        Instantiate(element_newObj, sheet_containerActive.GetChild(0)).GetComponent<RectTransform>();
    }

    public void ClearSheetElements()
    {
        for (int i = sheet_containerActive.GetChild(0).childCount - 1; i >= 0; i--)
            Destroy(sheet_containerActive.GetChild(0).GetChild(i).gameObject);
    }
}

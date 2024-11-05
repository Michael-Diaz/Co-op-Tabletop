using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CharSheetMaker : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefab_sheet;

    [SerializeField] private GameObject prefab_descriptor;
    [SerializeField] private GameObject prefab_inputText;
    [SerializeField] private GameObject prefab_inputNumber;

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

    public void AddSheet()
    {
        pageNum_current++;
        pageNum_max++;

        sheet_page.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{pageNum_current} / {pageNum_max}";

        RectTransform activeSheet_old = (RectTransform) sheet_containerActive.GetChild(0);
        activeSheet_old.SetParent(sheet_containerInactive);
        activeSheet_old.gameObject.SetActive(false);

        float rotZ = isHorizontal ? -90.0f : 0.0f;
        Quaternion activeSheet_new_rot = Quaternion.Euler(0.0f, 0.0f, (rotZ));

        RectTransform activeSheet_new = Instantiate(prefab_sheet, sheet_containerActive).GetComponent<RectTransform>();
        activeSheet_new.anchoredPosition = sheet_containerActive.anchoredPosition;
        activeSheet_new.rotation = activeSheet_new_rot;
    }

    public void RemoveSheet()
    {
         
    }

    public void SwapLayout()
    {
        isHorizontal = !isHorizontal;

        float rotZ = isHorizontal ? -90.0f : 0.0f;
        sheet_containerActive.GetChild(0).rotation = Quaternion.Euler(0.0f, 0.0f, (rotZ));

        for (int i = 0; i < sheet_containerInactive.childCount; i++)
        {
            sheet_containerActive.GetChild(i).rotation = Quaternion.Euler(0.0f, 0.0f, (rotZ));
        }

        float newPos = isHorizontal ? sheet_containerPrev_horizontalX : sheet_containerPrev_verticalX;
        sheet_containerPrev.anchoredPosition = new Vector2(newPos, sheet_containerPrev.anchoredPosition.y);

        newPos = isHorizontal ? sheet_containerNext_horizontalX : sheet_containerNext_verticalX;
        sheet_containerNext.anchoredPosition = new Vector2(newPos, sheet_containerNext.anchoredPosition.y);

        newPos = isHorizontal ? sheet_page_horizontalY : sheet_page_verticalY;
        sheet_page.anchoredPosition = new Vector2(sheet_page.anchoredPosition.x, newPos);
    }
}

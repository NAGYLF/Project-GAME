using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MainData.SupportScripts;
public class InteractiveObjectSelector : MonoBehaviour
{
    [SerializeField] public GameObject Content;
    [SerializeField] public GameObject ScrollPanel;
    [SerializeField] public GameObject InGameUI;
    private int selectedIndex = 0;
    public List<GameObject> selectableObjects;
    private List<GameObject> options;
    private void Awake()
    {
        options = new List<GameObject>();
        selectableObjects = new List<GameObject>();
    }
    public void RefressSelector()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }
        options.Clear();
        for (int i = 0; i < selectableObjects.Count; i++)
        {
            GameObject option = CreatePrefab("GameElements/SelectionBarOption");
            option.name = $"{selectableObjects[i].GetInstanceID().ToString()}";
            option.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = selectableObjects[i].GetComponent<Interact>().Title;
            option.name = selectableObjects[i].GetComponent<Interact>().Title;
            option.transform.SetParent(Content.transform,false);
            options.Add(option);
        }
        Selection();
    }
    public void Selection(int nextSelection = 0)
    {
        selectedIndex +=nextSelection;
        if (selectedIndex> options.Count-1)
        {
            selectedIndex= options.Count-1;
        }
        else if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }
        if (options.Count>0)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    options[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.black;
                    options[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    InGameUI.GetComponent<InGameUI>().SelectedObject = selectableObjects[i];
                }
                else
                {
                    options[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.white;
                    options[i].transform.GetChild(0).GetComponent<Image>().color = new Color(60f / 255f, 60f / 255f, 60f / 255f, 255f / 255f);
                }
            }
        }
        ScrollPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f - (float)selectedIndex / (options.Count - 1);
    }

    public void AddInteractObject(GameObject gameObject)
    {
        selectableObjects.Add(gameObject);
        RefressSelector();
    }
    public void RemoveInteractObject(GameObject gameObject)
    {
        selectableObjects.Remove(gameObject);
        RefressSelector();
    }
}

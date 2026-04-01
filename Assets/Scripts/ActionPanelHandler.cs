using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionPanelHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private String actionPanelName;
    
    private VisualElement _actionPanel;
    private List<Button> _buttonList;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(actionPanelName);

        _buttonList = _actionPanel.Children().Select(element => element as Button).ToList();
        
        _buttonList[0].clicked += Fight;
        _buttonList[1].clicked += Items;
        _buttonList[2].clicked += Guard;
        _buttonList[3].clicked += Escape;
    }

    private void OnDestroy()
    {
        _buttonList[0].clicked -= Fight;
        _buttonList[1].clicked -= Items;
        _buttonList[2].clicked -= Guard;
        _buttonList[3].clicked -= Escape;
    }

    public void Enable(PanelsStateEnum state)
    {
        _actionPanel.style.display = DisplayStyle.Flex;
    }

    public void Disable()
    {
        _actionPanel.style.display = DisplayStyle.None;
    }

    private void Fight()
    {
        MainPanelsController.Instance.SelectAbility();
    }
    
    private void Items()
    {
        MainPanelsController.Instance.SelectItem();
    }
    
    private void Guard()
    {
        Debug.Log("Guard");
    }
    
    private void Escape()
    {
        MainPanelsController.Instance.AttemptEscape();
    }
}

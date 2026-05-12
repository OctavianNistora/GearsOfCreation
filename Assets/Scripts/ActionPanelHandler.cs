using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Combat.Abilities;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionPanelHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private String actionPanelName;
    [SerializeField] private BaseAction firstGeneralAbility;
    [SerializeField] private BaseAction secondGeneralAbility;
    
    private VisualElement _actionPanel;
    private List<Button> _buttonList;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(actionPanelName);

        _buttonList = _actionPanel.Children().Select(element => element as Button).ToList();
        
        _buttonList[2].text = firstGeneralAbility.Name;
        _buttonList[3].text = secondGeneralAbility.Name;
        
        _buttonList[0].clicked += Fight;
        _buttonList[1].clicked += Items;
        _buttonList[2].clicked += FirstGeneralAction;
        _buttonList[3].clicked += SecondGeneralAction;
    }

    private void OnDestroy()
    {
        _buttonList[0].clicked -= Fight;
        _buttonList[1].clicked -= Items;
        _buttonList[2].clicked -= FirstGeneralAction;
        _buttonList[3].clicked -= SecondGeneralAction;
    }

    public void Enable()
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
    
    private void FirstGeneralAction()
    {
        var instance = firstGeneralAbility.CreateInstance();
        
        CombatManager.Instance.SelectAbility(instance);
        if (CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
        {
            MainPanelsController.Instance.SelectCharacterAction();
        }
    }
    
    private void Escape()
    {
        MainPanelsController.Instance.AttemptEscape();
    }

    private void SecondGeneralAction()
    {
        var instance = secondGeneralAbility.CreateInstance();
        
        CombatManager.Instance.SelectAbility(instance);
        if (CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
        {
            MainPanelsController.Instance.SelectCharacterAction();
        }
    }
}

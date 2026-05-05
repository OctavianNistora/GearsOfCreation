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
        _buttonList[3].clicked += Rest;
    }

    private void OnDestroy()
    {
        _buttonList[0].clicked -= Fight;
        _buttonList[1].clicked -= Items;
        _buttonList[2].clicked -= Guard;
        _buttonList[3].clicked -= Rest;
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
        var guardAbility = new Guard();
        
        CombatManager.Instance.SelectAbility(guardAbility);
        if (CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
        {
            MainPanelsController.Instance.SelectCharacterAction();
        }
    }
    
    private void Escape()
    {
        MainPanelsController.Instance.AttemptEscape();
    }

    private void Rest()
    {
        var restAbility = new Rest();
        
        CombatManager.Instance.SelectAbility(restAbility);
        if (CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
        {
            MainPanelsController.Instance.SelectCharacterAction();
        }
    }
}

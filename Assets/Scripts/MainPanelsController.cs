using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class MainPanelsController : MonoBehaviour
{
    [SerializeField] private ActionPanelHandler actionHandler;
    [SerializeField] private CharacterPanelHandler characterHandler;
    [SerializeField] private OptionsPanelHandler optionsHandler;
    [SerializeField] private BackButtonHandler backButtonHandler;
    
    public static MainPanelsController Instance { get; private set; }
    
    private readonly Stack<PanelsStateEnum> _stateStack = new();

    private void Start()
    {
        Instance = this;
        
        SelectCharacterAction();

        CombatManager.Instance.OnPlayerChoiceEnd += WaitCombatRoundEnd;
        CombatManager.Instance.OnPlayerChoiceStart += SelectCharacterAction;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnPlayerChoiceEnd -= WaitCombatRoundEnd;
        CombatManager.Instance.OnPlayerChoiceStart -= SelectCharacterAction;
    }

    public void SelectCharacterAction()
    {
        _stateStack.Clear();
        
        ChangeActivePanels(PanelsStateEnum.SelectAction);
    }

    public void SelectAbility()
    {
        ChangeActivePanels(PanelsStateEnum.SelectAbility);
    }

    public void SelectTarget()
    {
        ChangeActivePanels(PanelsStateEnum.SelectTarget);
    }

    public void SelectItem()
    {
        ChangeActivePanels(PanelsStateEnum.SelectItem);
    }

    public void AttemptEscape()
    {
        CombatManager.Instance.AttemptEscape();
    }

    public void GoPreviousState()
    {
        if (_stateStack.Count < 2)
        {
            return;
        }
        
        _stateStack.Pop();
        var previousState = _stateStack.Pop();
        ChangeActivePanels(previousState);
    }

    private void WaitCombatRoundEnd()
    {
        _stateStack.Clear();
        
        backButtonHandler.ChangeButtonDisplay(false);
        
        actionHandler.Disable();
        characterHandler.Disable();
        optionsHandler.Disable();
    }

    private void ChangeActivePanels(PanelsStateEnum state)
    {
        _stateStack.Push(state);
        
        backButtonHandler.ChangeButtonDisplay(_stateStack.Count > 1);
        
        switch (state)
        {
            case PanelsStateEnum.SelectAction:
                actionHandler.Enable();
                characterHandler.Enable(PanelsStateEnum.SelectAction);
                optionsHandler.Disable();
                break;
            case PanelsStateEnum.SelectAbility:
                actionHandler.Disable();
                characterHandler.Disable();
                optionsHandler.Enable(PanelsStateEnum.SelectAbility);
                break;
            case PanelsStateEnum.SelectTarget:
                actionHandler.Disable();
                characterHandler.Enable(PanelsStateEnum.SelectTarget);
                optionsHandler.Disable();
                break;
            case PanelsStateEnum.SelectItem:
                actionHandler.Disable();
                characterHandler.Disable();
                optionsHandler.Enable(PanelsStateEnum.SelectItem);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

public enum PanelsStateEnum
{
    SelectAction,
    SelectAbility,
    SelectTarget,
    SelectItem
}
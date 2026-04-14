using System;
using UnityEngine;

public class MainPanelsController : MonoBehaviour
{
    [SerializeField] private ActionPanelHandler actionHandler;
    [SerializeField] private CharacterPanelHandler characterHandler;
    [SerializeField] private OptionsPanelHandler optionsHandler;
    
    public static MainPanelsController Instance { get; private set; }

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
        actionHandler.Enable(PanelsStateEnum.SelectAction);
        characterHandler.Enable(PanelsStateEnum.SelectAction);
        optionsHandler.Disable();
    }

    public void SelectAbility()
    {
        actionHandler.Disable();
        characterHandler.Disable();
        optionsHandler.Enable(PanelsStateEnum.SelectAbility);
    }

    public void SelectTarget()
    {
        actionHandler.Disable();
        characterHandler.Enable(PanelsStateEnum.SelectTarget);
        optionsHandler.Disable();
    }

    public void SelectItem()
    {
        actionHandler.Disable();
        characterHandler.Disable();
        optionsHandler.Enable(PanelsStateEnum.SelectItem);
    }

    public void AttemptEscape()
    {
        CombatManager.Instance.AttemptEscape();
    }

    private void WaitCombatRoundEnd()
    {
        actionHandler.Disable();
        characterHandler.Disable();
        optionsHandler.Disable();
    }
}

public enum PanelsStateEnum
{
    SelectAction,
    SelectAbility,
    SelectTarget,
    SelectItem
}
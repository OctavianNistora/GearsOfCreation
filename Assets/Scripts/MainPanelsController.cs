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

        CombatManager.Instance.OnRoundStart += WaitRoundEnd;
        CombatManager.Instance.OnRoundEnd += SelectCharacterAction;
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
        Debug.Log("Selecting item");
        
        return;
        actionHandler.Disable();
        characterHandler.Disable();
        optionsHandler.Enable(PanelsStateEnum.SelectItem);
    }

    public void AttemptEscape()
    {
        Debug.Log("Attempting escape");
        
        return;
        WaitRoundEnd();
    }

    private void WaitRoundEnd()
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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionsPanelHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private String optionsPanelName;

    private VisualElement _actionPanel;
    private List<Button> _buttonList;
    private readonly Dictionary<Button, Action> _currentButtonActions = new();

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(optionsPanelName);


        var first = _actionPanel.Children();
        var second = first.Select(element => element.Children()).SelectMany(element => element);
        _buttonList = second.Select(element => element.Children().FirstOrDefault() as Button).ToList();
    }

    public void Enable(PanelsStateEnum state)
    {
        if (state == PanelsStateEnum.SelectItem)
        {
            LoadItems();
        }
        else
        {
            LoadAbilities();
        }
    
        _actionPanel.style.display = DisplayStyle.Flex;
    }

    private void RemovePreviousEvent(int index)
    {
        if (!_currentButtonActions.ContainsKey(_buttonList[index]))
        {
            return;
        }
        
        var action = _currentButtonActions[_buttonList[index]];
        _currentButtonActions.Remove(_buttonList[index]);
        _buttonList[index].clicked -= action;
    }

    private void AddButtonEvent(int index, Action action)
    {
        _currentButtonActions.Add(_buttonList[index], action);
        _buttonList[index].clicked += action;
    }
    
    private void LoadItems()
    {
        var items = PartyManager.Instance.Inventory;
        var itemsRemainingQuantity = items.Select(item =>
        {
            var oldQuantity = item.Quantity;
            var usedQuantity = CombatManager.Instance.GetItemUsedQuantity(item);
            return oldQuantity - usedQuantity;
        }).ToList();
        
        List<BaseCombatItem> itemsWithQuantity = new();
        List<int> quantityList = new();
        for (var j = 0; j < items.Count; j++)
        {
            if (itemsRemainingQuantity[j] <= 0) continue;
            
            itemsWithQuantity.Add(items[j]);
            quantityList.Add(itemsRemainingQuantity[j]);
        }
        
        var i = 0;
        for (; i < itemsWithQuantity.Count && i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.Flex;
            _buttonList[i].text = $"{itemsWithQuantity[i].Name}\tx{quantityList[i]}";
            
            RemovePreviousEvent(i);
            var item = itemsWithQuantity[i];
            AddButtonEvent(i, () => SelectAction(item));
        }

        for (; i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.None;
            
            RemovePreviousEvent(i);
        }
    }

    private void LoadAbilities()
    {
        var abilities = CombatManager.Instance.GetSelectedSource().Abilities;
        var i = 0;

        for (; i < abilities.Count && i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.Flex;
            _buttonList[i].text = abilities[i].Name + (abilities[i].ManaCost > 0 ? $"\tMP: {abilities[i].ManaCost}" : "");
            
            RemovePreviousEvent(i);
            var ability = abilities[i];
            AddButtonEvent(i, () => SelectAction(ability));
        }

        for (; i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.None;
            
            RemovePreviousEvent(i);
        }
    }

    private void SelectAction(BaseAction action)
    {
        var needsContinue = CombatManager.Instance.SelectAbility(action);

        if (needsContinue)
        {
            MainPanelsController.Instance.SelectTarget();
        }
        else if (CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
        {
            MainPanelsController.Instance.SelectCharacterAction();
        }
    }

    public void Disable()
    {
        _actionPanel.style.display = DisplayStyle.None;
    }
}

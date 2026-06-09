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
    private VisualElement _tooltipPanel;
    private Label _tooltipLabel;
    private List<Button> _buttonList;
    private readonly Dictionary<Button, Action> _buttonClickActions = new();
    private readonly Dictionary<Button, Action> _buttonHoverActions = new();

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(optionsPanelName);


        var first = _actionPanel.Children();
        var second = first.Select(element => element.Children()).SelectMany(element => element);
        _buttonList = second.Select(element => element.Children().FirstOrDefault() as Button).ToList();
        _buttonList.ForEach(button =>
        {
            button.RegisterCallback<MouseLeaveEvent>(HideTooltip);
        });
        
        
        _tooltipPanel = root.Q("AbilityTooltip");
        _tooltipPanel.style.display = DisplayStyle.None;
        _tooltipLabel = _tooltipPanel.Children().FirstOrDefault() as Label;
    }

    private void OnDestroy()
    {
        _buttonList.ForEach(button =>
        {
            button.UnregisterCallback<MouseLeaveEvent>(HideTooltip);
        });
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

    private void RemoveButtonClickEvent(int index)
    {
        if (!_buttonClickActions.ContainsKey(_buttonList[index]))
        {
            return;
        }
        
        var action = _buttonClickActions[_buttonList[index]];
        _buttonClickActions.Remove(_buttonList[index]);
        _buttonList[index].clicked -= action;
    }

    private void AddButtonClickEvent(int index, Action action)
    {
        _buttonClickActions.Add(_buttonList[index], action);
        _buttonList[index].clicked += action;
    }

    private void HideTooltip(MouseLeaveEvent even)
    {
        _tooltipPanel.style.display = DisplayStyle.None;
    }
    
    private void ShowTooltip(MouseEnterEvent even, string description)
    {
        _tooltipLabel.text = description;
        _tooltipPanel.style.display = DisplayStyle.Flex;
    }
    
    private void RemoveButtonHoverEvent(int index)
    {
        if (!_buttonHoverActions.ContainsKey(_buttonList[index]))
        {
            return;
        }
        
        _buttonHoverActions.Remove(_buttonList[index]);
        _buttonList[index].UnregisterCallback<MouseEnterEvent, string>(ShowTooltip);
    }
    
    private void AddButtonHoverEvent(int index, string description)
    {
        _buttonHoverActions.Add(_buttonList[index], () => ShowTooltip(new MouseEnterEvent(), description));
        _buttonList[index].RegisterCallback<MouseEnterEvent, string>(ShowTooltip, description);
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
            
            RemoveButtonClickEvent(i);
            RemoveButtonHoverEvent(i);
            var item = itemsWithQuantity[i];
            AddButtonClickEvent(i, () => SelectAction(item));
            AddButtonHoverEvent(i, item.Description);
        }

        for (; i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.None;
            
            RemoveButtonClickEvent(i);
            RemoveButtonHoverEvent(i);
        }
    }

    private void LoadAbilities()
    {
        var user = CombatManager.Instance.GetSelectedSource();
        var abilities = user.Abilities;
        var i = 0;

        for (; i < abilities.Count && i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.Flex;
            _buttonList[i].text = abilities[i].Name + (abilities[i].ManaCost > 0 ? $"\tMP: {abilities[i].ManaCost}" : "");
            
            _buttonList[i].SetEnabled(user.CurrentMana >= abilities[i].ManaCost);
            
            RemoveButtonClickEvent(i);
            RemoveButtonHoverEvent(i);
            var ability = abilities[i];
            AddButtonClickEvent(i, () => SelectAction(ability));
            AddButtonHoverEvent(i, ability.Description);
        }

        for (; i < _buttonList.Count; i++)
        {
            _buttonList[i].style.display = DisplayStyle.None;
            
            RemoveButtonClickEvent(i);
            RemoveButtonHoverEvent(i);
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
        _tooltipPanel.style.display = DisplayStyle.None;
    }
}

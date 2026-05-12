using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterPanelHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private String actionPanelName;

    private PanelsStateEnum _currentState;
    private VisualElement _actionPanel;
    private List<Button> _panelsList;
    private List<CharacterPanelElements> _panelElementsList;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(actionPanelName);

        _panelsList = _actionPanel.Children().Select(element => element as Button).ToList();
        _panelElementsList = new List<CharacterPanelElements>();
        foreach (var button in _panelsList)
        {
            var panelElements = new CharacterPanelElements();

            panelElements.Image = button.Q<Image>("Thumbnail");
            panelElements.Name = button.Q<Label>("Name");
            panelElements.HpProgressBar = button.Q<ProgressBar>("HP_ProgressBar");
            panelElements.ManaProgressBarParent = button.Q<VisualElement>("Mana");
            panelElements.ManaProgressBar = button.Q<ProgressBar>("Mana_ProgressBar");
            
            _panelElementsList.Add(panelElements);
        }
        
        _panelsList[0].clicked += SelectFirst;
        _panelsList[1].clicked += SelectSecond;
        _panelsList[2].clicked += SelectThird;
        _panelsList[3].clicked += SelectForth;
    }

    private void OnDestroy()
    {
        _panelsList[0].clicked -= SelectFirst;
        _panelsList[1].clicked -= SelectSecond;
        _panelsList[2].clicked -= SelectThird;
        _panelsList[3].clicked -= SelectForth;
    }

    public void Enable(PanelsStateEnum state)
    {
        _currentState = state;
        if (_currentState == PanelsStateEnum.SelectTarget)
        {
            if (CombatManager.Instance.GetSelectedAbility().TargetEnemy)
            {
                LoadEntities(CombatManager.Instance.enemies, true, false, false);
            }
            else
            {
                LoadEntities(PartyManager.Instance.Members, false, true, true);
            }
        }
        else
        {
            LoadEntities(PartyManager.Instance.Members, false, true, false, CombatManager.Instance.GetReadyAllies());
            MarkReady();
            SelectNotReady();
        }
        
        _actionPanel.style.display = DisplayStyle.Flex;
    }

    public void Disable()
    {
        _actionPanel.style.display = DisplayStyle.None;
        
        RemoveSelectionClass();
        RemoveReadyClass();
    }

    private void RemoveSelectionClass()
    {
        foreach (var element in _panelsList)
        {
            element.RemoveFromClassList("character-panel-element-selected");
        }
    }

    private void RemoveReadyClass()
    {
        foreach (var element in _panelsList)
        {
            element.RemoveFromClassList("character-panel-element-ready");
        }
    }

    private void SelectNotReady()
    {
        int index = 0;
        for (int i = 0; i < PartyManager.Instance.Members.Count; i++)
        {
            if (!CombatManager.Instance.GetReadyAllies().Contains(PartyManager.Instance.Members[i]))
            {
                CombatManager.Instance.SelectSource(PartyManager.Instance.Members[i]);
                
                index = i;
                break;
            }
        }
        
        _panelsList[index].AddToClassList("character-panel-element-selected");
    }

    private void MarkReady()
    {
        for(int i = 0; i < PartyManager.Instance.Members.Count; i++)
        {
            if (CombatManager.Instance.GetReadyAllies().Contains(PartyManager.Instance.Members[i]))
            {
                _panelsList[i].AddToClassList("character-panel-element-ready");
            }
        }
    }

    private void SelectEntity(int elementNo)
    {
        RemoveSelectionClass();
        
        if (_currentState == PanelsStateEnum.SelectTarget)
        {
            var isSelectingEnemy = CombatManager.Instance.GetSelectedAbility().TargetEnemy;
            var needsContinue = CombatManager.Instance.SelectTarget(isSelectingEnemy
                ? CombatManager.Instance.enemies[elementNo]
                : PartyManager.Instance.Members[elementNo]);

            if (!needsContinue && CombatManager.Instance.GetReadyAllies().Count < PartyManager.Instance.Members.Count)
            {
                MainPanelsController.Instance.SelectCharacterAction();
            }
            return;
        }

        CombatManager.Instance.SelectSource(PartyManager.Instance.Members[elementNo]);
        _panelsList[elementNo].AddToClassList("character-panel-element-selected");
    }

    private void SelectFirst()
    {
        SelectEntity(0);
    }

    private void SelectSecond()
    {
        SelectEntity(1);
    }
    
    private void SelectThird()
    {
        SelectEntity(2);
    }

    private void SelectForth()
    {
        SelectEntity(3);
    }

    private void LoadEntities(IEnumerable<BaseEntity> entities, bool hideDead, bool displayMana, bool selectDead,
        IEnumerable<BaseEntity> unselectableEntities = null)
    {
        using var entityEnumerator = entities.GetEnumerator();
        var i = 0;
        
        for (; i< _panelsList.Count && entityEnumerator.MoveNext(); i++)
        {
            var entity = entityEnumerator.Current;
            if (entity == null)
            {
                continue;
            }
            
            var panelElements = _panelElementsList[i];
            
            if (hideDead && entity.CurrentHp <= 0)
            {
                _panelsList[i].style.display = DisplayStyle.None;
                continue;
            }
            _panelsList[i].style.display = DisplayStyle.Flex;
            
            //panelElements.Image.sprite = enemy.Thumbnail;

            panelElements.Name.text = entity.Name;
            
            panelElements.HpProgressBar.value = (float)entity.CurrentHp / entity.MaxHp * 100;
            panelElements.HpProgressBar.title = $"{entity.CurrentHp}/{entity.MaxHp}";
            
            if (displayMana)
            {
                var ally = (PlayerEntity)entity;
                panelElements.ManaProgressBarParent.style.display = DisplayStyle.Flex;
                panelElements.ManaProgressBar.value = (float)ally.CurrentMana / ally.MaxMana * 100;
                panelElements.ManaProgressBar.title = $"{ally.CurrentMana}/{ally.MaxMana}";
            }
            else
            {
                panelElements.ManaProgressBarParent.style.display = DisplayStyle.None;
            }
            
            unselectableEntities ??= new List<BaseEntity>();
            var isEnabled = selectDead || entity.CurrentHp > 0 && !unselectableEntities.Contains(entity);
            _panelsList[i].SetEnabled(isEnabled);
        }

        for (; i < _panelElementsList.Count; i++)
        {
            _panelsList[i].style.display = DisplayStyle.None;
        }
    }

    private struct CharacterPanelElements
    {
        public Image Image;
        public Label Name;
        public ProgressBar HpProgressBar;
        public VisualElement ManaProgressBarParent;
        public ProgressBar ManaProgressBar;
    }
}

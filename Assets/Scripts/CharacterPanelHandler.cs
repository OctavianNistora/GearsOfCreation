using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterPanelHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private String actionPanelName;

    private VisualElement _actionPanel;
    private List<Button> _panelsList;
    private List<CharacterPanelElements> _panelElementsList;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        _actionPanel = root.Q(actionPanelName);

        _panelsList = _actionPanel.Children().Select(element => element as Button).ToList();
        foreach (var button in _panelsList)
        {
            var panelElements = new CharacterPanelElements();

            panelElements.Image = button.Q<Image>("Thumbnail");
            panelElements.Name = button.Q<Label>("Name");
            panelElements.HpProgressBar = button.Q<ProgressBar>("HP_ProgressBar");
            panelElements.ManaProgressBarParent = button.Q<VisualElement>("Mana");
            panelElements.ManaProgressBar = button.Q<ProgressBar>("Mana_ProgressBar");
        }
    }

    public void Enable(PanelsStateEnum state)
    {
        _actionPanel.style.display = DisplayStyle.Flex;
    }

    public void Disable()
    {
        _actionPanel.style.display = DisplayStyle.None;
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

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class BackButtonHandler : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private String backButtonName;
        [SerializeField] private MainPanelsController mainPanelsController;
        
        private Button _button;
        
        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            _button = root.Q<Button>(backButtonName);
            
            _button.style.display  = DisplayStyle.None;
            _button.clicked += GoBack;
        }

        private void OnDestroy()
        {
            _button.clicked -= GoBack;
        }

        private void GoBack()
        {
            mainPanelsController.GoPreviousState();
        }

        public void ChangeButtonDisplay(bool isDisplayed)
        {
            _button.style.display = isDisplayed ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
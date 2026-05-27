using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI textUI;
    [SerializeField] private PlayerInput playerInput;
    private DialogueLine[] lines;
    private int lineIndex;
    private int currentVisibleCharactersIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private float simpleDelayAmount = 0.05f;
    private float interpunctuationDelayAmount = 0.1f;
    private DialogueCharacter currentDialogueCharacter;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void SetCurrentDialogueCharacter(DialogueCharacter dialogueCharacter)
    {
        currentDialogueCharacter = dialogueCharacter;
    }

    public void StartDialogue(DialogueSequence dialogue)
    {
        dialogueActive = true;
        lines = dialogue.lines;
        lineIndex = 0;
        textUI.gameObject.SetActive(true);
        ShowLine();
    }

    public void ShowLine()
    {
        currentDialogueCharacter.SetTalking(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textUI.text = lines[lineIndex].speaker.characterName + ": " + lines[lineIndex].text;
        textUI.maxVisibleCharacters = lines[lineIndex].speaker.characterName.Length + 2;
        currentVisibleCharactersIndex = 0;

        typingCoroutine = StartCoroutine(TypeLine(lines[lineIndex].text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        
        while (currentVisibleCharactersIndex < text.Length)
        {
            char character = text[currentVisibleCharactersIndex];
            textUI.maxVisibleCharacters++;

            if (character == ';' || character == ',' || character == ':' || character == '-' || character == '.' || character == '?' || character == '!')
            {
                yield return new WaitForSeconds(interpunctuationDelayAmount);
            }
            else
            {
                yield return new WaitForSeconds(simpleDelayAmount);
            }

            currentVisibleCharactersIndex++;
        }

        currentDialogueCharacter.SetTalking(false);

        isTyping = false;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (dialogueActive)
        {
            Advance();
        }
    }

    public void Advance()
    {
        if (isTyping)
        {
            // Finish instantly
            StopCoroutine(typingCoroutine);
            textUI.maxVisibleCharacters = lines[lineIndex].speaker.characterName.Length + 2 + lines[lineIndex].text.Length;
            isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    public void NextLine()
    {
        lineIndex++;

        if (lineIndex < lines.Length)
        {
            ShowLine();
        }
        else
        {
            currentDialogueCharacter.SetTalking(false);

            dialogueActive = false;
            textUI.gameObject.SetActive(false);
            FadeTransition();
        }
    }

    async void FadeTransition()
    {
        await FadeManager.Instance.FadeToBlack();

        await Task.Delay(500);

        playerInput.SwitchCurrentActionMap("Player");
        
        await FadeManager.Instance.FadeToTransparent();
    }
}

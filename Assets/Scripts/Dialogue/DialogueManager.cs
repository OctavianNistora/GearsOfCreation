using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI textUI;
    private DialogueLine[] lines;
    private int lineIndex;
    private int currentVisibleCharactersIndex;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private WaitForSeconds simpleDelay = new WaitForSeconds(0.1f);
    private WaitForSeconds interpunctuationDelay = new WaitForSeconds(0.5f);
    [SerializeField] private float charactersPerSecond = 20;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueSequence dialogue)
    {
        lines = dialogue.lines;
        lineIndex = 0;
        textUI.gameObject.SetActive(true);
        ShowLine();
    }

    public void ShowLine()
    {
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
                yield return interpunctuationDelay;
            }
            else
            {
                yield return simpleDelay;
            }

            currentVisibleCharactersIndex++;
        }

        isTyping = false;
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
            textUI.gameObject.SetActive(false);
        }
    }

    public void ShowText(string message)
    {
        textUI.text = message;
        textUI.gameObject.SetActive(true);
    }

    public void HideText()
    {
        textUI.gameObject.SetActive(false);
    }
}

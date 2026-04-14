using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public CharacterData speaker;
    public string text;
    public AudioClip voiceLine;
}

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "Scriptable Objects/DialogueSequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueLine[] lines;
}

using UnityEngine;

public class DialogueCharacter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void SetTalking(bool isTalking) 
    {
        animator.SetBool("talk", isTalking);
    }
}

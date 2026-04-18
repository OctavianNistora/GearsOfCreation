using UnityEngine;
using UnityEngine.InputSystem;

public enum PlatformerTutorialStep
{
    Move,
    Jump,
    ClimbLedge,
    PickUpWeapon,
    Done
}

public class PlatformerTutorial : MonoBehaviour
{
    public PlatformerTutorialStep currentStep;
    [SerializeField] private NarrationManager narrationManager;
    [SerializeField] private PlayerInput playerInput;

    //temp
    private bool waitingForClimbLedge = false;
    private bool waitingForJump = false;

    void Start()
    {
        ShowStep();
    }

    void Update()
    {
        if (((currentStep == PlatformerTutorialStep.Jump && waitingForJump) || (currentStep == PlatformerTutorialStep.ClimbLedge && waitingForClimbLedge)) 
            && playerInput.actions["Jump"].triggered)
        {
            NextStep();
        }
        else
        {
            if (currentStep == PlatformerTutorialStep.Move && playerInput.actions["Horizontal"].triggered)
            {
                NextStep();
            }
        }
    }

    public void ShowStep()
    {
        switch (currentStep)
        {
            case PlatformerTutorialStep.Move:
                narrationManager.ShowText("Use A and D to move left and right");
                break;
            case PlatformerTutorialStep.Jump:
                narrationManager.ShowText("Press SPACE to jump");
                waitingForJump = true;
                break;
            case PlatformerTutorialStep.ClimbLedge:
                narrationManager.ShowText("That cliff seems a bit too high to jump on... Try to reach it anyway!");
                waitingForClimbLedge = true;
                break;
            case PlatformerTutorialStep.PickUpWeapon:
                narrationManager.ShowText("Might be useful later...");
                Invoke("NextStep",3);
                break;
            case PlatformerTutorialStep.Done:
                Destroy(gameObject);
                break;
        }
    }

    void NextStep()
    {
        currentStep++;
        narrationManager.HideText();
        //ShowStep();
    }
}

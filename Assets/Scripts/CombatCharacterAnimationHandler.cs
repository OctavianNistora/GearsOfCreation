using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CombatCharacterAnimationHandler : MonoBehaviour
{
    private static readonly int Hurt = Animator.StringToHash("Trigger Hurt");
    private static readonly int Death = Animator.StringToHash("Trigger Death");
    private static readonly int Number = Animator.StringToHash("VFX Number");
    private static readonly int VFX = Animator.StringToHash("Trigger VFX");

    [SerializeField]
    private SpriteLibrary characterSpriteLibrary;
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private SpriteLibrary vfxSpriteLibrary;
    [SerializeField]
    private Animator vfxAnimator;

    public void Initialize(SpriteLibraryAsset characterSpriteLibraryAsset = null, SpriteLibraryAsset vfxSpriteLibraryAsset = null)
    {
        if (characterSpriteLibraryAsset)
        {
            characterSpriteLibrary.spriteLibraryAsset = characterSpriteLibraryAsset;
        }
        
        if (vfxSpriteLibraryAsset)
        {
            vfxSpriteLibrary.spriteLibraryAsset = vfxSpriteLibraryAsset;
        }
    }

    public IEnumerator PlayHurtAnimation()
    {
        characterAnimator.SetTrigger(Hurt);
        yield return new WaitForSeconds(1f);
    }
    
    public IEnumerator PlayDeathAnimation()
    {
        characterAnimator.SetTrigger(Death);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public IEnumerator PlayVFXAnimation(int vfxNumber)
    {
        Debug.Log("Playing VFX animation with number: " + vfxNumber);
        vfxAnimator.SetInteger(Number, vfxNumber);
        vfxAnimator.SetTrigger(VFX);
        yield return new WaitForSeconds(1f);
    }
}

using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] ParticleSystem movementParticles;
    [SerializeField] ParticleSystem fallParticles;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [SerializeField] Rigidbody2DPhysicsControl rigidbodyControl;

    float counter;
    public bool isGrounded = false;

    void Update()
    {
        counter += Time.deltaTime;
        if (isGrounded && Mathf.Abs(rigidbodyControl.GetHorizontalVelocity()) > occurAfterVelocity) 
        {
            if (counter > dustFormationPeriod)
            {
                movementParticles.Play();
                counter = 0;
            }
        }
    }

    public void PlayFallParticles()
    {
        fallParticles.Play();
    }
}

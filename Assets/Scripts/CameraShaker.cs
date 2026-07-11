using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private CenterCameraOnPlayer centerCameraOnPlayer;

    public AnimationCurve curve;
    public float duration = 1f;

    public void ShakeCamera()
    {
        centerCameraOnPlayer.enabled = false;

        Vector3 startPosition = camera.transform.position;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration) * 10;
            camera.transform.position = startPosition + Random.insideUnitSphere * strength;
        }

        camera.transform.position = startPosition;
        centerCameraOnPlayer.enabled = true;
    }
}

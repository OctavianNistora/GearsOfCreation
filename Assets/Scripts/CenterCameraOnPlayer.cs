using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CenterCameraOnPlayer : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    private void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        if (camera == null)
        {
            Debug.LogError("No camera found to center on player.");
            return;
        }

        var cameraPositionConstraint = camera.gameObject.GetComponent<PositionConstraint>();
        if (!cameraPositionConstraint)
        {
            cameraPositionConstraint = camera.gameObject.AddComponent<PositionConstraint>();
        }
        
        ConfigureConstraint(cameraPositionConstraint);
    }

    private void ConfigureConstraint(PositionConstraint positionConstraint)
    {
        positionConstraint.constraintActive = true;
        positionConstraint.weight = 1;
        
        positionConstraint.locked = true;
        positionConstraint.translationOffset = Vector3.back;
        positionConstraint.translationAxis = Axis.X | Axis.Y | Axis.Z;
        
        var constraintSourceList = new List<ConstraintSource>(new[]
        {
            new ConstraintSource
            {
                sourceTransform = transform,
                weight = 1
            }
        });
        positionConstraint.SetSources(constraintSourceList);
    }
}

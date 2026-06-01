using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerLedgeClimb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject xRaycastLowReference;
    [SerializeField] private GameObject xRaycastHighReference;
    [SerializeField] private GameObject yRaycastCloseReference;
    [SerializeField] private GameObject yRaycastFarReference;
    [SerializeField] private Rigidbody2DPhysicsControl playerRigidbody2DPhysicsControl;
    [SerializeField] private PlayerInput playerInput;
    
    [Header("Settings")]
    [SerializeField] private List<AbstractConditionEmitter> conditionList;
    [SerializeField] private float xRaycastCount = 7;
    [SerializeField] private float yRaycastCount = 3;
    [SerializeField] private float raycastXDistance = 1;
    [SerializeField] private float raycastYDistance = 2;
    [SerializeField] private float verticalSpeed = 5;
    [SerializeField] private float horizontalSpeed = 5;
    [SerializeField] private float verticalErrorCorrection = 0.05f;
    
    private Dictionary<AbstractConditionEmitter, bool> _conditions = new();
    private Coroutine _climbLedgeCoroutine;

    private void Start()
    {
        foreach (var condition in conditionList)
        {
            _conditions[condition] = false;
            condition.OnConditionChange += UpdateCondition;
        }
    }

    private void OnDestroy()
    {
        foreach (var condition in conditionList)
        {
            condition.OnConditionChange -= UpdateCondition;
        }
    }

    private void UpdateCondition(AbstractConditionEmitter condition, bool value)
    {
        _conditions[condition] = value;

        if (value && AreAllConditionsMet())
        {
            ClimbLedge();
        }
    }

    private bool AreAllConditionsMet()
    {
        return _conditions.Values.ToList().All(condition => condition);
    }

    private void ClimbLedge()
    {
        _animator.SetTrigger("ledge_grab");
        
        float? xPosition = null;

        var hit = Physics2D.Raycast(xRaycastHighReference.transform.position, xRaycastHighReference.transform.right,
            raycastXDistance, LayerMask.GetMask("Terrain"));
        if (hit.collider)
        {
            xPosition = hit.point.x;
        }
        else if (xRaycastCount > 1)
        {
            var rayStep = (xRaycastLowReference.transform.position - xRaycastHighReference.transform.position) / (xRaycastCount - 1);
            
            for (int i = 1; i <= xRaycastCount - 1; i++)
            {
                var rayOrigin = xRaycastHighReference.transform.position + rayStep * i;
                hit = Physics2D.Raycast(rayOrigin, xRaycastHighReference.transform.right,
                    raycastXDistance, LayerMask.GetMask("Terrain"));
                if (hit.collider)
                {
                    xPosition = hit.point.x;
                    break;
                }
            }
        }

        if (!xPosition.HasValue)
        {
            return;
        }

        
        float? yPosition = null;
        
        hit = Physics2D.Raycast(yRaycastCloseReference.transform.position, yRaycastCloseReference.transform.right,
            raycastYDistance, LayerMask.GetMask("Terrain"));
        if (hit.collider)
        {
            yPosition = hit.point.y;
        }
        else if (yRaycastCount > 1)
        {
            var rayStep = (yRaycastFarReference.transform.position - yRaycastCloseReference.transform.position) / (yRaycastCount - 1);
            
            for (int i = 1; i <= yRaycastCount - 1; i++)
            {
                var rayOrigin = yRaycastCloseReference.transform.position + rayStep * i;
                hit = Physics2D.Raycast(rayOrigin, yRaycastCloseReference.transform.right,
                    raycastYDistance, LayerMask.GetMask("Terrain"));
                if (hit.collider)
                {
                    yPosition = hit.point.y;
                    break;
                }
            }
        }
        
        if (!yPosition.HasValue)
        {
            return;
        }

        Debug.Log($"xPosition: {xPosition.Value}, yPosition: {yPosition.Value}");
        Debug.Log("ClimbLedge");
        
        if (_climbLedgeCoroutine != null)
        {
            StopCoroutine(_climbLedgeCoroutine);
        }
        _climbLedgeCoroutine = StartCoroutine(ClimbLedgeCoroutine(xPosition.Value, yPosition.Value));
    }

    private IEnumerator ClimbLedgeCoroutine(float xTarget, float yTarget)
    {
        playerInput.actions.FindAction("Player/Horizontal").Disable();
        playerRigidbody2DPhysicsControl.PausePhysics();
        
        var playerGameObject = playerInput.gameObject;

        var correctedYTarget = yTarget + verticalErrorCorrection;
        var yDifference = correctedYTarget - playerGameObject.transform.position.y;
        while (!Mathf.Approximately(yDifference, 0))
        {
            var translationDistance = Mathf.Min(verticalSpeed * Time.deltaTime, yDifference);
            playerGameObject.transform.Translate(0, translationDistance, 0);
            
            yield return null;
            yDifference = correctedYTarget - playerGameObject.transform.position.y;
        }

        var xDifference = xTarget - playerGameObject.transform.position.x;
        while (!Mathf.Approximately(xDifference, 0))
        {
            float translationDistance;
            translationDistance = Mathf.Sign(xDifference) > 0 ?
                Mathf.Min(horizontalSpeed * Time.deltaTime, xDifference) :
                Mathf.Max(-horizontalSpeed * Time.deltaTime, xDifference);
            playerGameObject.transform.Translate(translationDistance, 0, 0);
            
            yield return null;
            xDifference = xTarget - playerGameObject.transform.position.x;
        }
        
        playerRigidbody2DPhysicsControl.RestartPhysics();
        playerInput.actions.FindAction("Player/Horizontal").Enable();
    }
}

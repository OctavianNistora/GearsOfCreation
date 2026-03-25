using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLedgeClimb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject xRaycastReference;
    [SerializeField] private GameObject yRaycastReference;
    [SerializeField] private Rigidbody2DPhysicsControl playerRigidbody2DPhysicsControl;
    [SerializeField] private PlayerInput playerInput;
    
    [Header("Settings")]
    [SerializeField] private List<AbstractConditionEmitter> conditionList;
    [SerializeField] private float raycastDistance = 5;
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
        RaycastHit2D hit = Physics2D.Raycast(xRaycastReference.transform.position, xRaycastReference.transform.right,
            raycastDistance, LayerMask.GetMask("Terrain"));
        if (!hit.collider)
        {
            return;
        }
        var xPosition = hit.point.x;

        hit = Physics2D.Raycast(yRaycastReference.transform.position, yRaycastReference.transform.right,
            raycastDistance, LayerMask.GetMask("Terrain"));
        if (!hit.collider)
        {
            return;
        }
        var yPosition = hit.point.y;
        
        Debug.Log("ClimbLedge");
        if (_climbLedgeCoroutine != null)
        {
            StopCoroutine(_climbLedgeCoroutine);
        }
        _climbLedgeCoroutine = StartCoroutine(ClimbLedgeCoroutine(xPosition, yPosition));
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

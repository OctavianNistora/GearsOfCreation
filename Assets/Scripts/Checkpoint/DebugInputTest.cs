using UnityEngine;

/// <summary>
/// Temporary script — attach to your player to verify input is being read.
/// Delete this once the checkpoint system works.
/// </summary>
public class DebugInputTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("[DebugInputTest] X key detected on: " + gameObject.name);
        }
    }
}
using UnityEngine;

/// Attach ONE instance of this to any GameObject in a level scene.
/// It immediately unlocks the exit boat so the player can leave
/// without completing the objective.
/// Remove this script when you want the normal lock-behind-objective flow.
public class FreeFlow : MonoBehaviour
{
    private void Start()
    {
        LevelExit exit = FindFirstObjectByType<LevelExit>();
        if (exit != null)
        {
            exit.Unlock();
            Debug.Log("[FreeFlow] Exit boat unlocked at start — objective bypass active.");
        }
        else
        {
            Debug.LogWarning("[FreeFlow] No LevelExit found in scene.");
        }
    }
}

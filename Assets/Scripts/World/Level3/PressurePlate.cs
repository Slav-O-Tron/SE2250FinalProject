using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isCorrectPlate = false;
    public PuzzleManager puzzleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        
        PushableStone stone = other.GetComponent<PushableStone>();
        if (stone == null) return;

        if (isCorrectPlate)
            puzzleManager.OnCorrectPlate();
        else
            puzzleManager.OnWrongPlate();
    }

    private void OnTriggerExit(Collider other)
    {
        PushableStone stone = other.GetComponent<PushableStone>();
        if (stone == null) return;

        if (isCorrectPlate)
            puzzleManager.OnPlateDeactivated();
    }
}
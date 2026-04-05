using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject gate;
    public GameObject zombieSpawner;
    private bool puzzleSolved = false;

    public void OnCorrectPlate()
    {
        Debug.Log("OnCorrectPlate called!");
        if (puzzleSolved) return;
        puzzleSolved = true;
        gate.SetActive(false);
    }

    public void OnWrongPlate()
    {
        if (puzzleSolved) return;
        if (zombieSpawner != null)
            zombieSpawner.SetActive(true);
        Debug.Log("Wrong plate - zombies!");
    }

    public void OnPlateDeactivated()
    {
        if (puzzleSolved) return;
        gate.SetActive(true);
        Debug.Log("Stone removed - gate closed again");
    }
}
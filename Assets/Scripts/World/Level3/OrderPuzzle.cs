using UnityEngine;

public class OrderPuzzle : MonoBehaviour
{
    public int[] correctOrder;
    public GameObject cathedralDoor;
    public GameObject zombieSpawner;

    private int[] playerOrder;
    private int currentStep = 0;
    private bool puzzleSolved = false;

    void Start()
    {
        playerOrder = new int[correctOrder.Length];
    }

    public void ButtonPressed(int buttonID)
    {
        if (puzzleSolved) return;

        playerOrder[currentStep] = buttonID;
        currentStep++;

        if (currentStep >= correctOrder.Length)
            CheckOrder();
    }

    private void CheckOrder()
    {
        for (int i = 0; i < correctOrder.Length; i++)
        {
            if (playerOrder[i] != correctOrder[i])
            {
                WrongOrder();
                return;
            }
        }
        SolvePuzzle();
    }

    private void WrongOrder()
    {
        currentStep = 0;
        playerOrder = new int[correctOrder.Length];
        if (zombieSpawner != null)
            zombieSpawner.SetActive(true);
        Debug.Log("Wrong order - zombies spawned!");
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        if (cathedralDoor != null)
            cathedralDoor.SetActive(false);
        Debug.Log("Puzzle solved - door opens!");
    }
    

}
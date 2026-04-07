using UnityEngine;

public class CrystalFragment: MonoBehaviour, IInteractable
{
    public void OnAction() {
    Debug.Log("Crystal Fragment Collected");
    Destroy(gameObject);
}
}

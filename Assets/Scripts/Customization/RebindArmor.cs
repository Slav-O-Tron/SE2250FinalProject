using System.Collections.Generic;
using UnityEngine;

public class RebindArmor : MonoBehaviour
{
    public SkinnedMeshRenderer armorRenderer;
    public Transform targetRigRoot;   // Drag the PLAYER'S PT_Hips here
    public string rootBoneName = "PT_Hips";

    [ContextMenu("Rebind Now")]
    public void RebindNow()
    {
        if (armorRenderer == null)
            armorRenderer = GetComponent<SkinnedMeshRenderer>();

        if (armorRenderer == null || targetRigRoot == null)
        {
            Debug.LogWarning("Missing armorRenderer or targetRigRoot.");
            return;
        }

        var targetBones = new Dictionary<string, Transform>();
        foreach (Transform t in targetRigRoot.GetComponentsInChildren<Transform>(true))
        {
            if (!targetBones.ContainsKey(t.name))
                targetBones.Add(t.name, t);
        }

        Transform[] oldBones = armorRenderer.bones;
        Transform[] newBones = new Transform[oldBones.Length];

        for (int i = 0; i < oldBones.Length; i++)
        {
            if (oldBones[i] != null && targetBones.TryGetValue(oldBones[i].name, out Transform match))
                newBones[i] = match;
            else
                newBones[i] = oldBones[i];
        }

        armorRenderer.bones = newBones;

        if (targetBones.TryGetValue(rootBoneName, out Transform rootMatch))
            armorRenderer.rootBone = rootMatch;

        Debug.Log("Armor rebound to player skeleton.");
    }

    void Start()
    {
        RebindNow();
    }
}
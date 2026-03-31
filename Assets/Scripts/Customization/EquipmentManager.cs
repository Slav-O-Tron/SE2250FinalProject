using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Drag the PLAYER'S PT_Hips here")]
    public Transform playerRigRoot;

    private Dictionary<EquipmentSlot, GameObject> equippedObjects =
        new Dictionary<EquipmentSlot, GameObject>();

    private Dictionary<EquipmentSlot, ItemData> equippedItems =
        new Dictionary<EquipmentSlot, ItemData>();

    public void ToggleEquip(ItemData item)
    {
        if (item == null || !item.isEquipable)
            return;

        if (equippedItems.ContainsKey(item.equipmentSlot) &&
            equippedItems[item.equipmentSlot] == item)
        {
            Unequip(item.equipmentSlot);
        }
        else
        {
            Equip(item);
        }
    }

    public void Equip(ItemData item)
    {
        if (item == null || item.equipmentPrefab == null)
            return;

        Unequip(item.equipmentSlot);

        GameObject newPiece = Instantiate(item.equipmentPrefab, transform);
        RebindArmor(newPiece);

        equippedObjects[item.equipmentSlot] = newPiece;
        equippedItems[item.equipmentSlot] = item;

        Debug.Log("Equipped: " + item.itemName);
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (equippedObjects.ContainsKey(slot) && equippedObjects[slot] != null)
        {
            Destroy(equippedObjects[slot]);
        }

        equippedObjects.Remove(slot);
        equippedItems.Remove(slot);
    }

    private void RebindArmor(GameObject armorObject)
    {
        if (playerRigRoot == null)
            return;

        Dictionary<string, Transform> targetBones = new Dictionary<string, Transform>();

        foreach (Transform t in playerRigRoot.GetComponentsInChildren<Transform>(true))
        {
            if (!targetBones.ContainsKey(t.name))
                targetBones.Add(t.name, t);
        }

        SkinnedMeshRenderer[] renderers =
            armorObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer smr in renderers)
        {
            Transform[] newBones = new Transform[smr.bones.Length];

            for (int i = 0; i < smr.bones.Length; i++)
            {
                Transform oldBone = smr.bones[i];

                if (oldBone != null && targetBones.TryGetValue(oldBone.name, out Transform match))
                    newBones[i] = match;
                else
                    newBones[i] = oldBone;
            }

            smr.bones = newBones;

            if (targetBones.TryGetValue("PT_Hips", out Transform rootBone))
                smr.rootBone = rootBone;
        }
    }
}
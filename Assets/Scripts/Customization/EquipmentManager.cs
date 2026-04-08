using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Armor")]
    public Transform playerRigRoot;   // Drag PT_Hips here

    [Header("Weapon Attach Points")]
    public Transform rightHandSocket; // Drag PT_Right_Hand_Weapon_slot here
    public Transform leftHandSocket;  // Optional

    private Dictionary<EquipmentSlot, GameObject> equippedObjects =
        new Dictionary<EquipmentSlot, GameObject>();

    private Dictionary<EquipmentSlot, ItemData> equippedItems =
        new Dictionary<EquipmentSlot, ItemData>();

    private void Start()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        foreach (ItemData item in pd.savedEquipped)
        {
            if (item != null)
                Equip(item);
        }
    }

    private void OnDestroy()
    {
        PlayerData pd = PlayerData.GetOrCreate();
        pd.savedEquipped.Clear();
        foreach (ItemData item in equippedItems.Values)
        {
            if (item != null)
                pd.savedEquipped.Add(item);
        }
    }

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
        {
            Debug.LogWarning("Equip failed: item or equipmentPrefab is missing.");
            return;
        }

        Unequip(item.equipmentSlot);

        // Weapon
        if (item.equipmentSlot == EquipmentSlot.Weapon)
        {
            if (rightHandSocket == null)
            {
                Debug.LogWarning("Right hand socket is missing.");
                return;
            }

            GameObject newWeapon = Instantiate(item.equipmentPrefab, rightHandSocket);
            newWeapon.transform.localPosition = item.equipPositionOffset;
            newWeapon.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
            newWeapon.transform.localScale = item.equipScaleOffset;

            BindWeaponOwnership(newWeapon);

            equippedObjects[item.equipmentSlot] = newWeapon;
            equippedItems[item.equipmentSlot] = item;

            Debug.Log("Equipped weapon: " + item.itemName);
            return;
        }

        // Armor / other skinned equipment
        GameObject newPiece = Instantiate(item.equipmentPrefab, transform);

        RebindArmorObject(newPiece);

        newPiece.transform.localPosition = item.equipPositionOffset;
        newPiece.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
        newPiece.transform.localScale = item.equipScaleOffset;

        equippedObjects[item.equipmentSlot] = newPiece;
        equippedItems[item.equipmentSlot] = item;

        Debug.Log("Equipped: " + item.itemName);
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (slot == EquipmentSlot.Weapon)
        {
            PlayerCombat combatOwner = GetComponentInParent<PlayerCombat>();
            if (combatOwner != null)
            {
                combatOwner.weaponHitbox = null;
            }
        }

        if (equippedObjects.ContainsKey(slot) && equippedObjects[slot] != null)
        {
            Destroy(equippedObjects[slot]);
        }

        equippedObjects.Remove(slot);
        equippedItems.Remove(slot);
    }

    private void RebindArmorObject(GameObject armorObject)
    {
        if (playerRigRoot == null)
        {
            Debug.LogWarning("Player rig root is missing.");
            return;
        }

        Dictionary<string, Transform> targetBones = new Dictionary<string, Transform>();

        foreach (Transform t in playerRigRoot.GetComponentsInChildren<Transform>(true))
        {
            if (!targetBones.ContainsKey(t.name))
                targetBones.Add(t.name, t);
        }

        SkinnedMeshRenderer[] renderers =
            armorObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No SkinnedMeshRenderer found on " + armorObject.name);
            return;
        }

        foreach (SkinnedMeshRenderer smr in renderers)
        {
            Transform[] oldBones = smr.bones;
            Transform[] newBones = new Transform[oldBones.Length];

            for (int i = 0; i < oldBones.Length; i++)
            {
                if (oldBones[i] != null && targetBones.TryGetValue(oldBones[i].name, out Transform match))
                    newBones[i] = match;
                else
                    newBones[i] = oldBones[i];
            }

            smr.bones = newBones;

            if (targetBones.TryGetValue("PT_Hips", out Transform rootBone))
                smr.rootBone = rootBone;
        }
    }

    private void BindWeaponOwnership(GameObject weaponObject)
    {
        if (weaponObject == null)
        {
            return;
        }

        Player playerOwner = GetComponentInParent<Player>();
        PlayerCombat combatOwner = GetComponentInParent<PlayerCombat>();
        WeaponHitbox[] hitboxes = weaponObject.GetComponentsInChildren<WeaponHitbox>(true);

        foreach (WeaponHitbox hitbox in hitboxes)
        {
            hitbox.InitializeOwner(playerOwner, combatOwner);
        }

        if (combatOwner != null)
        {
            combatOwner.weaponHitbox = hitboxes.Length > 0 ? hitboxes[0] : null;
        }
    }
}
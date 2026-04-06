using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPotion", menuName = "Inventory/Attack Potion")]
public class AttackPotion : ItemData
{
    public int attackBoostAmount = 5;
    public float boostDuration = 10f;

    public override void Use(Player player)
    {
        player.StartCoroutine(ApplyBoost(player));
    }

    private IEnumerator ApplyBoost(Player player)
    {
        player.attackDamage += attackBoostAmount;
        yield return new WaitForSeconds(boostDuration);
        player.attackDamage -= attackBoostAmount;
    }
}
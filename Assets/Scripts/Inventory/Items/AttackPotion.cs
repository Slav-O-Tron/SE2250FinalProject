using UnityEngine;

public class AttackPotion : Item
{
    public int attackBoostAmount = 5;
    public float boostDuration = 10f;

    public override void Use(Player player)
    {
        if (player != null)
        {
            player.StartCoroutine(ApplyAttackBoost(player));
        }

        quantity--;
        if (quantity <= 0)
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator ApplyAttackBoost(Player player)
    {
        player.attackDamage += attackBoostAmount;
        Debug.Log("Attack boosted by " + attackBoostAmount);

        yield return new WaitForSeconds(boostDuration);

        player.attackDamage -= attackBoostAmount;
        Debug.Log("Attack boost ended");
    }
}
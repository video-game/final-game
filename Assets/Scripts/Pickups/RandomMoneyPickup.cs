using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoneyPickup : Pickup {
    public int maxAmount = 5;
    public int minAmount = 1;

	protected override void Effect()
    {
        AudioManager.Instance.PlayAudioClip("CoinPickup");
        int amount = Random.Range(1, (maxAmount + 1)/GameManager.Instance.player.Count);
        player.item.ChangeMoney(amount);
    }
}

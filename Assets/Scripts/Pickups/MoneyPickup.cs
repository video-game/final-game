using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : Pickup {
    public int amount = 10;

	protected override void Effect()
    {
        AudioManager.Instance.PlayAudioClip("CoinPickup");
        player.item.ChangeMoney(amount);
    }
}

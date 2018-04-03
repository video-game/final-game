using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : Pickup {

	protected override void Effect()
    {
        player.item.ChangeMoney(10);
    }
}

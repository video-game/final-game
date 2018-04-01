using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyPickup : Pickup {

	protected override void Effect()
    {
        GameManager.Instance.resourceHud.UpdateMoney(10);
    }
}

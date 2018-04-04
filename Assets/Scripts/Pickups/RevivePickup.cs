using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivePickup : Pickup
{
	protected override void Effect()
	{
        GameManager.Instance.sharedItems.ChangeRevives(1);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthIncrease : Pickup
{
	protected override void Effect()
	{
		foreach (var p in GameManager.Instance.player)
		{
			p.MaxHealth += 20;
			p.ChangeHealth(20);
		}
	}
}

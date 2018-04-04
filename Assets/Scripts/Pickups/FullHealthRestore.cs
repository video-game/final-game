using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealthRestore : Pickup
{
	protected override void Effect()
	{
		foreach (var p in GameManager.Instance.player)
			p.ChangeHealth(p.MaxHealth);
	}
}

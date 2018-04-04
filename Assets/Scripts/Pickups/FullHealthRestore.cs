using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealthRestore : Pickup
{
	protected override void Effect()
	{
		player.ChangeHealth(9999);
	}
}

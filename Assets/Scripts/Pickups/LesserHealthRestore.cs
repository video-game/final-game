using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserHealthRestore : Pickup
{
	protected override void Effect()
	{
		player.ChangeHealth(30);
	}
}

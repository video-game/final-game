﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePickup : Pickup
{
	protected override void Effect()
	{
		foreach (var p in GameManager.Instance.player)
			p.GrantExperience(50);
	}
}

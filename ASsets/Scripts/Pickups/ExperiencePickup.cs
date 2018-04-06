using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePickup : Pickup
{
	public int Amount = 50;

	protected override void Effect()
	{
        AudioManager.Instance.PlayAudioClip("PlayerRevive");
		foreach (var p in GameManager.Instance.player)
			p.GrantExperience(50);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEgg : MonoBehaviour
{
	[SerializeField]
	private GameObject spiderPrefab;
	[SerializeField]
	private Sprite brokenEggTexture;
	private bool hatched;
	private SpriteRenderer spriteRenderer;
	private Transform parent;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		parent =  GameObject.Find("Game World/Enemies").transform;
	}

	public void Hatch()
	{
		if (!hatched)
		{
			if (brokenEggTexture != null)
				spriteRenderer.sprite = brokenEggTexture;
			
			if (spiderPrefab != null)
				foreach (var player in GameManager.Instance.player)
					Instantiate(spiderPrefab, transform.position, spiderPrefab.transform.rotation, parent);

			hatched = true;
		}
	}
}

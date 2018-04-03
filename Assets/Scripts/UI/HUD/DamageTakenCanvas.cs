using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTakenCanvas : MonoBehaviour
{
	[SerializeField]
	private GameObject damageTakenTextPrefab;

	// Detach from parent before it's destroyed so text doesn't disappear.
	// First I tried to set parent to null in OnDestroy but that wouldn't work.
	// Has to be called manually from parent.
	public void Orphan()
	{
		transform.SetParent(null);
	}

	public void InitializeDamageText(int value)
	{
		var clone = Instantiate(
			damageTakenTextPrefab,
			damageTakenTextPrefab.transform.localPosition,
			damageTakenTextPrefab.transform.localRotation,
			transform
		);

		var text = clone.GetComponent<TextMeshProUGUI>();
		text.color = (value > 0) ? Color.green : Color.red;
		text.text = value.ToString();

		clone.GetComponent<RectTransform>().localRotation = Quaternion.identity;

		Destroy(clone.gameObject, 2);
	}

    //For showing level up, spawns slightly above the other text
    public void InitializeLevelUpText()
    {
        Vector3 pos = damageTakenTextPrefab.transform.localPosition;
        pos = new Vector3(pos.x, pos.y, pos.z + 0.5f);
        var clone = Instantiate(
            damageTakenTextPrefab,
            pos,
            damageTakenTextPrefab.transform.localRotation,
            transform
        );

        var text = clone.GetComponent<TextMeshProUGUI>();
        text.color = Color.magenta;
        text.text = "Level Up!";

        clone.GetComponent<RectTransform>().localRotation = Quaternion.identity;

        Destroy(clone.gameObject, 3);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHud : MonoBehaviour {
    int revives;
    
    int money;

    TMPro.TextMeshProUGUI reviveText;
    TMPro.TextMeshProUGUI moneyText;

    public void Init(int mon, int rev)
    {
        GameManager.Instance.resourceHud = this;

        var texts = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        reviveText = texts[0];
        moneyText = texts[1];

        UpdateMoney(mon);
        UpdateRevives(rev);
    }

    public bool UpdateMoney(int amount)
    {
        if(money + amount >= 0 || amount > 0)
        {
            money += amount;
            money = Mathf.Clamp(money, 0, 999);
            moneyText.text = money.ToString();
            return true;
        }
        return false;
    }

    public bool UpdateRevives(int amount)
    {
        if (revives + amount >= 0 || amount > 0)
        {
            revives += amount;
            revives = Mathf.Clamp(revives, 0, 20);
            reviveText.text = revives.ToString();
            return true;
        }
        return false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

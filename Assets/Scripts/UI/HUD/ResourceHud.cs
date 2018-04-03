using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHud : MonoBehaviour {

    public TMPro.TextMeshProUGUI reviveText;
    public TMPro.TextMeshProUGUI moneyText;

    public void Init(int mon, int rev)
    {
        GameManager.Instance.sharedItems.OnMoneyChange += UpdateMoney;
        GameManager.Instance.sharedItems.OnReviveChange += UpdateRevives;
        UpdateMoney(mon);
        UpdateRevives(rev);
    }

    public void UpdateMoney(int amount)
    {
        moneyText.text = amount.ToString();
    }

    public void UpdateRevives(int amount)
    {
        reviveText.text = amount.ToString();
    }

}

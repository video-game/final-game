using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellVillager : Villager {

    public int cost;

    [SerializeField]
    private GameObject acceptImage;
    [SerializeField]
    private GameObject declineImage;

    private GameObject originalImage;

    private void Start()
    {
        originalImage = image.gameObject;
    }
    public override void Interaction()
    {
        if(!itemDropped)
        {
            if (GameManager.Instance.sharedItems.ChangeMoney(cost))
            {
                AudioManager.Instance.PlayAudioClip("Buy");
                Drop();
                StartCoroutine(BuyCoroutine(acceptImage));
            }
            else
            {
                StartCoroutine(BuyCoroutine(declineImage));
            }
        }
    }
    private IEnumerator BuyCoroutine(GameObject gObject)
    {
        itemDropped = true;
        var clone = Instantiate(gObject, transform.Find("UIElements"));
        originalImage.SetActive(false);
        yield return new WaitForSeconds(1);
        Destroy(clone);
        if(someoneInRange)
            originalImage.SetActive(true);
        itemDropped = false;
    }

}

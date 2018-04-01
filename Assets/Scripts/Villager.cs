using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Villager : MonoBehaviour {

    private SpriteRenderer sRenderer;
    [SerializeField]
    private Sprite[] directions;
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private GameObject itemDrop;
    private bool itemDropped;

    [SerializeField]
    private Sprite interactImage;

    private bool someoneInRange;

    [SerializeField]
    private Transform dialogueBox;
    [SerializeField]
    private Transform image;
	// Use this for initialization
	void Awake () {
        someoneInRange = false;
        itemDropped = false;
        sRenderer = GetComponent<SpriteRenderer>();

        dialogueBox = transform.Find("UIElements").Find("DialogueBox");
        image = transform.Find("UIElements").Find("Image");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos = GetClosestPlayer();

        if (InRange(playerPos))
        {
            //Look towards nearest player
            Vector3 directionVector = playerPos - transform.position;
            sRenderer.sprite = directions[(int)Utilities.VectorToDirection(directionVector.x, directionVector.z)];
            if(!someoneInRange)
            {
                StartCoroutine(DialogueFadeIn());
            }
        }
        else
        {
            //not really sure if it's bad to set the sprite every frame for villager (i'm hoping no)
            sRenderer.sprite = directions[3];
            if (someoneInRange)
            {
                StartCoroutine(DialogueFadeOut());
            }
        }
	}

    private IEnumerator DialogueFadeIn()
    {
        someoneInRange = true;
        StopCoroutine(DialogueFadeOut());

        yield return new WaitForSeconds(0.2f);
        dialogueBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        image.gameObject.SetActive(true);
    }
    private IEnumerator DialogueFadeOut()
    {
        someoneInRange = false;
        StopCoroutine(DialogueFadeIn());

        yield return new WaitForSeconds(0.2f);
        image.gameObject.SetActive(false);
        dialogueBox.gameObject.SetActive(false);
    }
    protected Vector3 GetClosestPlayer()
    {
        Vector3 min = new Vector3(0, 0, 0);
        float distance = Mathf.Infinity;

        for (int i = 0; i < GameManager.Instance.player.Count; i++)
        {
            if (!GameManager.Instance.player[i].Alive)
            {
                continue;
            }

            float temp = Vector3.Distance(GameManager.Instance.player[i].transform.position, transform.position);
            if (temp < distance)
            {
                min = GameManager.Instance.player[i].transform.position;
                distance = temp;
            }
        }

        return min;
    }

    public virtual void Interact()
    {
        if(itemDrop != null && !itemDropped)
        {
            itemDropped = true;
            GameObject drop = Instantiate(itemDrop);
            drop.transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z - 1);
        }
        if(interactImage != null)
        {
            image.GetComponent<Image>().sprite = interactImage;
        }
    }

    public bool InRange(Vector3 playerPos)
    {
        return Vector3.Distance(transform.position, playerPos) < viewDistance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    public string musicName;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("tigger entered!");
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("music is going to play now");
            AudioManager.Instance.PlayMusicClip(musicName);
        }
    }
}

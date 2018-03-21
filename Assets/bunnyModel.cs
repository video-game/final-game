using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bunnyModel : MonoBehaviour {

    //For the animation event feature in its animator
    public void shakeCamera()
    {
        Camera.main.GetComponent<CameraEffects>().ShakeCamera(0.1f, 0.05f);
    }
}

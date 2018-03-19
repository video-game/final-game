using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {
    private bool shaking = false;

    public void ShakeCamera(float duration, float magnitude)
    {
        shaking = true;
        StartCoroutine(CameraShakeRoutine(duration, magnitude));
    }
    IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        Vector3 leftPos = new Vector3(transform.localPosition.x - magnitude, transform.localPosition.y, transform.localPosition.z);
        Vector3 rightPos = new Vector3(transform.localPosition.x + magnitude, transform.localPosition.y, transform.localPosition.z);
        bool posToggle = false;
        while (duration > 0)
        {
            if(posToggle)
            {
                transform.localPosition = rightPos;
            }
            else
            {
                transform.localPosition = leftPos;
            }
            posToggle = !posToggle;
            yield return new WaitForSeconds(0.05f);
            duration -= 0.05f;
        }
        transform.localPosition = Vector3.zero;
    }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {

    private DynamicCamera dCamera;
    private void Awake()
    {
        dCamera = GetComponent<DynamicCamera>();
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShakeRoutine(duration, magnitude));
    }
    IEnumerator CameraShakeRoutine(float duration, float magnitude)
    {
        bool posToggle = false;
        while (duration > 0)
        {
            dCamera.offset = posToggle ? new Vector3(-magnitude, 0, 0) : new Vector3(magnitude, 0, 0);
            posToggle = !posToggle;
            yield return new WaitForSeconds(0.03f);
            duration -= 0.03f;
        }

        dCamera.offset = Vector3.zero;
    }

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
    }
}

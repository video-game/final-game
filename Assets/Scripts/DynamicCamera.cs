using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCamera : MonoBehaviour {

    //camera position offset
    public Vector3 offset;
    public Vector3 velocity;
    public float smoothTime;

    private Camera cam;

    public float minSize;
    public float maxSize;

    public bool camZoom;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if(GameManager.Instance.player.Count != 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, GetCenterPoint(), ref velocity, smoothTime);
            transform.position = transform.position + offset;
            if (camZoom)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, GetOptimalCameraSize(), Time.deltaTime * 2);
            }
        }
    }

    private Bounds GetBounds()
    {
        Bounds bounds = new Bounds(GameManager.Instance.player[GameManager.Instance.player.Count - 1].transform.position, Vector3.zero);
        for (int i = 0; i < GameManager.Instance.player.Count; i++)
        {
            if (!GameManager.Instance.player[i].Alive)
            {
                continue;
            }
            bounds = new Bounds(GameManager.Instance.player[i].transform.position, Vector3.zero);
            break;
        }
        return bounds;
    }

    private Vector3 GetCenterPoint()
    {
        Bounds bounds = GetBounds();
        for (int i = 0; i < GameManager.Instance.player.Count; i++)
        {
            if (!GameManager.Instance.player[i].Alive)
            {
                continue;
            }
            bounds.Encapsulate(GameManager.Instance.player[i].transform.position);
            
        }

        return bounds.center;
    }

    private float GetOptimalCameraSize()
    {
        Bounds bounds = GetBounds();
        for (int i = 0; i < GameManager.Instance.player.Count; i++)
        {
            if (!GameManager.Instance.player[i].Alive)
            {
                continue;
            }
            bounds.Encapsulate(GameManager.Instance.player[i].transform.position);  
        }

        return Mathf.Clamp(Mathf.Max(bounds.size.x, bounds.size.y * 1.78f ), minSize, maxSize);
    }
}

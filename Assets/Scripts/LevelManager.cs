using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMB<LevelManager> {

    public List<Transform> StartLocation;

    void OnDrawGizmos()
    {
        for (int i = 0; i < StartLocation.Count; i++)
        {
            Gizmos.DrawSphere(StartLocation[i].position, 0.25f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public int enemyCount;
        public GameObject enemy;
    }
    
    public EnemySpawnInfo[] enemies;

    public Texture debugTexture;
	// Use this for initialization
	void Start () {
        Spawn();
	}
	
	// Update is called once per frame
	void Update () {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0.3F);
        Gizmos.DrawCube(transform.position, new Vector3(transform.localScale.x, 0.2f, transform.localScale.z));
        //Gizmos.DrawGUITexture(new Rect(transform.position.x, transform.position.y, dimensions.x, dimensions.y), debugTexture);
    }
    bool PositionObstructed(Vector3 position)
    {
        RaycastHit hit;
        
        if (Physics.SphereCast(new Vector3(position.x , 10, position.z), 1f, Vector3.down * 25, out hit))
        {
            if(hit.collider.name == "Mesh Container")
                return true;
        }
        return false;
    }

    public void Clear()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Spawn()
    {
        Vector3 scale = transform.localScale;
        foreach(var enemyType in enemies)
        {
            int adjustedEnemyCount = enemyType.enemyCount * GameManager.Instance.playerCount;
            for (int i = 0; i < adjustedEnemyCount; i++)
            {
                Vector3 randPos = new Vector3(transform.position.x + Random.Range(-scale.x / 2f, scale.x / 2f), 0, transform.position.z + Random.Range(-scale.z / 2f, scale.z / 2f));
                while (PositionObstructed(randPos))
                {
                    randPos = new Vector3(transform.position.x + Random.Range(-scale.x / 2f, scale.x / 2f), 0, transform.position.z + Random.Range(-scale.z / 2f, scale.z / 2f));
                }

                Debug.DrawRay(randPos, Vector3.up * 20, Color.red, 20);
                GameObject clone = Instantiate(enemyType.enemy);
                clone.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(randPos);
                clone.transform.SetParent(transform);
            }
        }
    }
}

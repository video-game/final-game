using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : PlayerProjectile {
    [SerializeField]
    private Rigidbody smallProjectile;

    private bool exploded = false;
    protected override void Explode()
    {
        exploded = true;
        base.Explode();
        // Fire projectiles in 8 directions (Left, Down, Right, Up, and between)
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                var direction = new Vector3(x, 0, z).normalized;
                var clone = Instantiate(smallProjectile, transform.position, smallProjectile.transform.rotation);
                clone.GetComponent<DemoProjectile>().init(direction, gameObject);
            }
        }
    }
    private void OnDestroy()
    {
        if(!exploded)
            Explode();
    }
}

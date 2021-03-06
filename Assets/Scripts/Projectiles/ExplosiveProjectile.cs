﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : PlayerProjectile {
    [SerializeField]
    private Rigidbody smallProjectile;

    private bool exploded = false;

    [SerializeField]
    private string explodeSound = "ClusterExplosion";
    protected override void Explode()
    {
        exploded = true;
        AudioManager.Instance.PlayAudioClip(explodeSound);
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
                clone.GetComponent<Projectile>().Init(direction, gameObject);
                clone.GetComponent<Projectile>().shooter = shooter;
            }
        }
    }
    private void OnDestroy()
    {
        if(!exploded)
            Explode();
    }
}

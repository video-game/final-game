using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class RangedAbilityHitDetector : AbilityHitDetector
{
    Rigidbody rb;
    SpriteRenderer sr;

    public RangedAbility Ability { get { return (RangedAbility)ability; } }

    public CoroutineSlave HitSlave;

    public void Init(RangedAbility a, string t, Vector3 direction, float lt)
    {
        base.Init(a, t, lt);
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.freezeRotation = true;
        rb.useGravity = false;
        string tagNLayer = Ability.UsedBy.tag == "Player" ? "PlayerProjectile" : "EnemyProjectile"; ;
        transform.tag = tagNLayer;
        gameObject.layer = LayerMask.NameToLayer(tagNLayer);
        Vector3 temp = ((Player)Ability.UsedBy).ProjectileSpawn.position;
        transform.position = new Vector3(temp.x, 0.01f, temp.z);
        sr.sprite = Ability.projectileSprite;
        sr.sortingLayerName = "Projectile";
        GetComponent<SphereCollider>().radius = .125f;
        HitSlave = new GameObject().AddComponent<CoroutineSlave>();
        HitSlave.gameObject.transform.parent = transform;


        Fire(direction);
    }

    public void Fire(Vector3 direction)
    {
        //Set the direction according to original velocity + accuracy
        float spread = Ability.spread / 2f;
        float randomAngle = Random.Range(-spread, +spread);
        Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        direction = randomRotation * direction;
        direction = new Vector3(direction.x, 0, direction.z);
        direction.Normalize();
        rb.velocity = direction * Ability.speed;

        velocity = rb.velocity;
        Debug.Log("set " + velocity);

        float rotation = Quaternion.LookRotation(direction).eulerAngles.y;
        //rotate the projectile towards its direction
        transform.rotation = Quaternion.Euler(new Vector3(90, rotation, 0));
    }

}

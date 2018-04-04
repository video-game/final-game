using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAbilityHitDetector : AbilityHitDetector
{
    public MeleeAbility Ability { get { return (MeleeAbility)ability; } }

    public void Init(MeleeAbility a, string t, Vector3 direction)
    {
        base.Init(a, t);
 
        Vector3 temp = ((Player)Ability.UsedBy).transform.position;
        transform.position = new Vector3(temp.x, 0.01f, temp.z);
        Swing(direction);
    }

    public void Swing(Vector3 direction)
    {
        direction = Quaternion.AngleAxis(-Ability.degrees / 2, Vector3.up) * direction;
        int rayCount = (int)(Ability.degrees / 10f);

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 distance = direction * (.5f + Ability.distance);
            Debug.DrawRay(transform.position, distance, Color.blue, .05f);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, distance, out hit, (.5f + Ability.distance)))
            {
                if (hit.collider != null && hit.collider.tag == targetTag)
                {
                    Debug.Log("hit");
                    hit.collider.GetComponent<Unit>().ChangeHealth(-5);
                    Ability.OnHit(this, hit.collider.gameObject);
                    break;
                }
            }

            direction = Quaternion.AngleAxis(Ability.degrees / rayCount, Vector3.up) * direction;
        }
    }
}

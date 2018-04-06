using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Ability/Effect/Dash", fileName = "New Dash Effect")]
public class DashEffect : Effect {

    public float dashSpeed = 12;
    public float dashDuration = 0.15f;

    Vector3 dashVelocity;

    NavMeshAgent agent;

    Transform transform;

    Unit unit;

    public override void Trigger(Ability ability) {
        agent = ability.UsedBy.gameObject.GetComponent<NavMeshAgent>();
        transform = ability.UsedBy.transform;
        dashVelocity = agent.velocity.normalized * dashSpeed;

        unit = ability.UsedBy;
        unit.dashingVelocity = dashVelocity;

        ability.slave.StartCoroutine(DashCoroutine());

        Debug.Log("dash");
    }

    private IEnumerator DashCoroutine()
    {
        unit.dashing = true;
        TrailRenderer tRenderer = transform.Find("Model").GetComponent<TrailRenderer>();
        tRenderer.time = 0.3f;
        tRenderer.enabled = true;
        agent.ResetPath();
        yield return new WaitForSeconds(dashDuration);
        tRenderer.time = 0.3f;
        unit.dashing = false;

        yield return new WaitForSeconds(0.1f);
        tRenderer.enabled = false;
    }

}

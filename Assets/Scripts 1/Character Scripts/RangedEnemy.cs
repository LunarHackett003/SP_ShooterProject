using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] LayerMask layermask;
    float shotTimer = 2;
    public Weapon weapon;
    [SerializeField] float viewDistance = 30;
   

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void EnemyBehaviour()
    {
        if(weapon)
            weapon.UpdateTracers();

        CheckLineOfSight();
        switch (state)
        {
            case States.CHASE:
                Move();
                break;
            case States.ATTACK:
                transform.rotation = Quaternion.LookRotation(target.transform.position - (transform.position + Vector3.down), Vector3.up);
                shotTimer -= Time.deltaTime;
                weapon.SetFireInput(shotTimer <= 0);
                if (shotTimer <= 0)
                {
                    Debug.Log("EnemyFired");
                    shotTimer = 2;
                }
                break;
            case States.PATROL:
                break;
            case States.RETREAT:
                break;
            default:
                break;
        }
    }

    void CheckLineOfSight()
    {
        if (Physics.Linecast(transform.position, target.transform.position,out RaycastHit hit, layermask, QueryTriggerInteraction.Ignore))
        {
            if (hit.rigidbody && hit.rigidbody.TryGetComponent(out Character player)&&hit.distance<=viewDistance)
            {
                Debug.Log("found");
                agent.enabled = false;
                state = States.ATTACK;
            }
            else
            {
                agent.enabled = true;
                state = States.CHASE;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{


    // Update is called once per frame
    void Update()
    {
        
    }
    

    protected override void EnemyBehaviour()
    {
        base.EnemyBehaviour();
        if (IsAlive)
        {
            switch (state)
            {
                case States.PATROL:

                    break;
                case States.CHASE:
                    Move();
                    break;
                case States.ATTACK:
                    transform.rotation = Quaternion.LookRotation(target.transform.position - (transform.position + Vector3.down), Vector3.up);
                    break;
            }
        animator.SetBool("Attacking", state == States.ATTACK);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            state = States.ATTACK;
            firing = true;
            agent.enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        state = States.CHASE;
        agent.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Character
{
    protected NavMeshAgent agent;
    [SerializeField] protected Slider healthBarRef;
    protected bool firing;
    public GameObject target;
    public enum States
    {
        PATROL,
        CHASE,
        ATTACK,
        RETREAT
    }
    public States state = States.PATROL;
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = MoveSpeed;
        healthBarRef.maxValue = maxHealth;
        healthBarRef.value = maxHealth;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void FixedUpdate()
    {
        EnemyBehaviour();
    }
    public override void Move()
    {
        agent.SetDestination(target.transform.position);
    }
   

    public override void Die()
    {
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddRelativeTorque(Random.onUnitSphere * 50);
        Destroy(gameObject, 2);
        animator.enabled = false;
    }

    protected virtual void EnemyBehaviour()
    {
        
    }
    public override void UpdateHealth(float healthChange, Vector3 damagePosition)
    {
        base.UpdateHealth(healthChange, damagePosition);
        healthBarRef.value = CurrentHealth;
    }
}

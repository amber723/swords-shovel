using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour
{
    public AttackDefinition demoAttack;

    CharacterStats stats;
    Animator animator;
    NavMeshAgent agent;

    GameObject attackTarget;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void SetDestination(Vector3 destination)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = destination;
    }

    public void AttackTarget(GameObject target)
    {
        //var attack = demoAttack.CreateAttack(stats, target.GetComponent<CharacterStats>());

        //var attackables = target.GetComponentsInChildren(typeof(IAttackable));

        //foreach (IAttackable attackable in attackables)
        //{
        //    attackable.OnAttack(gameObject, attack);
        //}

        var weapon = stats.GetCurrentWeapon();

        if (weapon != null)
        {
            //interrupt any running PursueNAttack operations and allow us to
            //switch target on the fly.
            StopAllCoroutines();

            agent.isStopped = false;
            attackTarget = target;

            StartCoroutine(PursueNAttackTarget());
        }
    }

    //running towards the target unitl it's within range.
    IEnumerator PursueNAttackTarget()
    {
        agent.isStopped = false;
        var weapon = stats.GetCurrentWeapon();

        while (Vector3.Distance(transform.position, attackTarget.transform.position) 
            > weapon.Range)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        //make sure facing the target
        transform.LookAt(attackTarget.transform);
        animator.SetTrigger("Attack");
    }

    //To be called by the animation controller
    public void Hit()
    {
        //Have our weapon attack the attack target
        if (attackTarget != null)
        {
            stats.GetCurrentWeapon().ExecuteAttack(gameObject, attackTarget);
        }
    }
}

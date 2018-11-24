using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour
{
    public AttackDefinition demoAttack;
    public Aoe aoeStompAttack;

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

        #region Watch for ActiveSkill Keypresses 

        //Check for a ActiveSkill key to be pressed
        if (Input.GetKeyDown(KeyCode.Q))
            DoStomp(transform.position);

        if (Input.GetKeyDown(KeyCode.W))
        { }

        if (Input.GetKeyDown(KeyCode.E))
        { }

        if (Input.GetKeyDown(KeyCode.R))
        { }

        if (Input.GetKeyDown(KeyCode.S))
        { }

        #endregion
    }

    public void SetDestination(Vector3 destination)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = destination;
    }

    public void DoStomp(Vector3 destination)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        StartCoroutine(GoToTargetAndStomp(destination));
    }

    private IEnumerator GoToTargetAndStomp(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > aoeStompAttack.Range)
        {
            agent.destination = destination;
            yield return null;
        }
        agent.isStopped = true;
        animator.SetTrigger("Stomp");
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

    public void Stomp()
    {
        aoeStompAttack.Fire(gameObject, gameObject.transform.position, 
            LayerMask.NameToLayer("PlayerSpells"));
    }
}

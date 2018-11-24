using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public float patrolTime = 15;   // time in seconds to wait before seeking a new patrol destination
    public float aggroRange = 10;   // distance in scene units below which the NPC will increase speed and seek the player
    public Transform[] waypoints;   // collection of waypoints which define a patrol area
    public AttackDefinition attack; // the attack the NPC inflicts on our player.

    public Transform SpellHotSpot;

    int index;                  // the current waypoint index in the waypoints array
    float speed, agentSpeed;    // current agent speed and NavMeshAgent component speed
    Transform player;           // reference to the player object transform

    Animator animator;          // reference to the animator component
    NavMeshAgent agent;         // reference to the NavMeshAgent

    float timeOfLastAttack;     //store the time the NPC attacked the player

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) { agentSpeed = agent.speed; }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        index = Random.Range(0, waypoints.Length);

        //Repeatedly run the Tick() every half second.
        InvokeRepeating("Tick", 0, 0.5f);

        if (waypoints.Length > 0)
        {
            //time(the 2nd param) is a delay for the start of this cycle.
            InvokeRepeating("Patrol", Random.Range(0, patrolTime), patrolTime);
        }

        //use minvalue instead of 0, since we dont want our game thinking the 
        //NPC attacked the player when the game started.
        timeOfLastAttack = float.MinValue;
    }

    void Update()
    {
        speed = Mathf.Lerp(speed, agent.velocity.magnitude, Time.deltaTime * 10);
        animator.SetFloat("Speed", speed);

        float timeSinceLastAttack = Time.time - timeOfLastAttack;
        bool attackOnCooldown = timeSinceLastAttack < attack.Cooldown;

        agent.isStopped = attackOnCooldown;

        if (player)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position,
                player.transform.position);
            bool attackInRange = distanceFromPlayer < attack.Range;

            if (!attackOnCooldown && attackInRange)
            {
                transform.LookAt(player.transform);
                timeOfLastAttack = Time.time;
                animator.SetTrigger("Attack");
            }
        }
    }

    public void Hit()
    {
        if (!player)
            return;

        if (attack is Weapon)
        {
            ((Weapon) attack).ExecuteAttack(gameObject, player.gameObject);
        }
        else if (attack is Spell)
        {
            ((Spell) attack).Cast(gameObject, SpellHotSpot.position,
                player.transform.position, LayerMask.NameToLayer("EnemySpells"));
        }
    }

    //The second timer to control how frequently the NPC pursues a different waypoint
    void Patrol()
    {
        index = index == waypoints.Length - 1 ? 0 : index + 1;
    }

    //one timer will be used for the NPC to check it's status
    //Check if it should be in patrol mode or pursuing the player
    void Tick()
    {
        agent.destination = waypoints[index].position;
        agent.speed = agentSpeed / 2;

        if (player != null && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            agent.destination = player.position;
            agent.speed = agentSpeed;
        }
    }

    //This is a built-in method which executes when 
    //the object is being rendered in the Scene view.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}

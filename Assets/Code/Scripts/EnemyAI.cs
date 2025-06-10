using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour {
    [Header("AI Settings")]
    [Tooltip("The target the enemy will chase or flee from. Assign the Player's Transform here.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("The speed of the NavMeshAgent.")]
    [SerializeField] private float moveSpeed = 3.5f;
    
    [Tooltip("The distance the enemy will try to run away from the player when fleeing.")]
    [SerializeField] private float fleeDistance = 15.0f;

    [Tooltip("How often the AI updates its destination (in seconds). Lower values are more responsive but less performant.")]
    [SerializeField] private float updateRate = 0.5f;

    [Header("Animation")]
    [Tooltip("Animator controlling the enemy's animations.")]
    [SerializeField] private Animator animator;
    private MeshRenderer meshRenderer;

    private NavMeshAgent agent;
    private bool isFleeing = false;
    private float updateTimer;
    
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else
            {
                Debug.LogError("EnemyAI Error: Player Transform is not assigned and no GameObject with the tag 'Player' was found. Please assign the player or tag the player object.");
                this.enabled = false;
            }
        }
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
    }
    
    private void OnEnable() {
        if (agent != null) agent.speed = moveSpeed;
        updateTimer = 0f;
    }

    private void Update() {
        if (playerTransform == null) return;
        
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateRate) {
            updateTimer = 0f;
            if (isFleeing) Flee();
            else Chase();
        }
    }

    private void Chase()
    {
        if (agent.isOnNavMesh) agent.SetDestination(playerTransform.position);
        Debug.Log(Vector3.Distance(transform.position, playerTransform.position));
        if (Vector3.Distance(transform.position, playerTransform.position) < 170f)
        {
            Debug.Log("Quit");
            Application.Quit();
        }
        animator.SetTrigger("Forward");
    }

    private void Flee() {
        Vector3 directionAwayFromPlayer = transform.position - playerTransform.position;
        Vector3 fleeTargetPosition = transform.position + directionAwayFromPlayer.normalized * fleeDistance;
        
        if (NavMesh.SamplePosition(fleeTargetPosition, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas))
            if (agent.isOnNavMesh) agent.SetDestination(hit.position);
        animator.SetTrigger("Back");
    }

    public void StartFleeing() {
        isFleeing = true;
        Debug.Log(gameObject.name + " is now fleeing!");
    }
    public void StopFleeing() {
        isFleeing = false;
        Debug.Log(gameObject.name + " is no longer fleeing!");
    }
}
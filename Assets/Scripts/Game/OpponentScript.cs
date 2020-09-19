using UnityEngine.AI;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class OpponentScript : MonoBehaviour
{
    NavMeshAgent agent;
    ThirdPersonCharacter character;
    public GameObject circle;
    public float maxDistance = 10f;
    bool circleReached = true;
    Vector3 circleTarget;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        character = GetComponent<ThirdPersonCharacter>();
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    void Patroling()
    {
        
        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            agent.SetDestination(circle.transform.position);
    }


    private void Update()
    {
        if (circleReached)
        {
            circleTarget = RandomNavmeshLocation(maxDistance);
            circleReached = false;
        }
        else
        {
            circle.transform.position = Vector3.MoveTowards(circle.transform.position, circleTarget, 0.01f);
            if (Vector3.Distance(circle.transform.position, circleTarget) < 0.05f)
            {
                circleReached = true;
                print("hello");
            }
        }
          

        Patroling();

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }
}

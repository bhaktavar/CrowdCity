using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class Patrol : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public ThirdPersonCharacter character;
    public float captureRange;
    public bool Captured = false;
    public GameObject master = null;
    int multiplier;
    //public GameObject currPrefab;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        destPoint = Random.Range(0, points.Length);
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;
        int temp = destPoint;
        //Selecting random points in PatrolPoints array to keep moving AI agents
        while (temp == destPoint)
        {
            destPoint = Random.Range(0, points.Length);
        }
    }

    public void Patroling()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();

        
    }

    public void ChasePlayer()
    {
        agent.SetDestination(master.transform.position);
        agent.autoBraking = true;
        transform.LookAt(master.transform);
        agent.stoppingDistance = 3f + (0.5f * multiplier);
        //agent.agentTypeID = 1;
    }

    void Update()
    {
        //inCaptureRange = Physics.CheckSphere(transform.position, captureRange);

        var arr = Physics.OverlapSphere(transform.position, captureRange);

        if (!Captured)
        {
            foreach (Collider coll in arr)
            {
                if (coll.tag == "Player")
                {
                    Captured = true;
                    master = coll.gameObject;
                    master.GetComponent<PlayerMovement>().PlayerScore++;
                    multiplier = master.GetComponent<PlayerMovement>().PlayerScore;
                    multiplier++;
                    break;
                }
                else if (coll.tag == "PlayerFollower")
                {
                    if (coll.gameObject.GetComponent<Patrol>().Captured == true)
                    {
                        Captured = true;
                        master = coll.gameObject.GetComponent<Patrol>().master;
                        master.GetComponent<PlayerMovement>().PlayerScore++;
                        multiplier = master.GetComponent<PlayerMovement>().PlayerScore;
                        multiplier++;
                        break;
                    }
                }
            }
        }
        
        if (!Captured)
        {
            Patroling();
        }
        else
        {
            ChasePlayer();
        }
        //Control animation of AI using TPS standard assets
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
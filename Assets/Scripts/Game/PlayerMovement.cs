using UnityEngine.AI;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerMovement : MonoBehaviour
{
    public Transform circle;
    ThirdPersonCharacter character;
    Rigidbody rb;
    
    NavMeshAgent agent;
    public int PlayerScore = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        character = GetComponent<ThirdPersonCharacter>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if(Physics.Raycast(ray, out hit))
        //    {
        //        agent.SetDestination(hit.point);
        //    }
        //}

        agent.SetDestination(circle.position);

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

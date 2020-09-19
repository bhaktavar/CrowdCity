using UnityEngine.AI;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections;

public class FollowerScript : MonoBehaviour
{
    NavMeshAgent agent;
    ThirdPersonCharacter character;
    public float maxDistance = 10f;
    public bool Captured = false;
    public GameObject master = null;
    public float captureRange = 3f;
    int multiplier;
    public GameObject heart;
    AudioSource ting;

    private void Start()
    {
        ting = GetComponent<AudioSource>();
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
            agent.SetDestination(RandomNavmeshLocation(maxDistance));
    }

    void ChasePlayer()
    {
        agent.SetDestination(master.transform.position);
        agent.autoBraking = true;
        transform.LookAt(master.transform);
        agent.stoppingDistance = 0.05f + (0.01f * multiplier);
        agent.speed = 0.8f;
    }

    void getCaptured(GameObject master)
    {
        master.GetComponent<ScoreScript>().PlayerScore++;
        multiplier = master.GetComponent<ScoreScript>().PlayerScore;
        multiplier++;
    }

    private void Update()
    {

        var arr = Physics.OverlapSphere(transform.position, captureRange);

        if (!Captured)
        {
            foreach (Collider coll in arr)
            {
                if (coll.tag == "Player")
                {
                    Captured = true;
                    showHeart();
                    master = coll.gameObject;
                    getCaptured(master);
                    break;
                }
                else if (coll.tag == "PlayerFollower")
                {
                    if (coll.gameObject.GetComponent<FollowerScript>().Captured == true)
                    {
                        Captured = true;
                        showHeart();
                        master = coll.gameObject.GetComponent<FollowerScript>().master;
                        getCaptured(master);
                        break;
                    }
                }
            }
            Patroling();
        }

        else
        {
            ChasePlayer();
            foreach (Collider coll in arr)
            {
                if (coll.tag == "Player")
                {
                    if (master.GetComponent<ScoreScript>().PlayerScore < coll.gameObject.GetComponent<ScoreScript>().PlayerScore)
                    {
                        showHeart();
                        master.GetComponent<ScoreScript>().PlayerScore--;
                        master = coll.gameObject;
                        getCaptured(master);
                        break;
                    }
                }
                else if (coll.tag == "PlayerFollower")
                {
                    if (coll.gameObject.GetComponent<FollowerScript>().Captured == true)
                    {
                        showHeart();
                        master.GetComponent<ScoreScript>().PlayerScore--;
                        if (master.GetComponent<ScoreScript>().PlayerScore < coll.gameObject.GetComponent<FollowerScript>().master.GetComponent<ScoreScript>().PlayerScore)
                        {
                            master = coll.gameObject.GetComponent<FollowerScript>().master;
                            getCaptured(master);
                            break;
                        }
                    }
                }
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }

    void showHeart()
    {
        heart.SetActive(true);
        ting.Play();
        StartCoroutine(heartTimer());
        
    }

    IEnumerator heartTimer()
    {
        yield return new WaitForSeconds(2f);
        heart.SetActive(false);
    }
}

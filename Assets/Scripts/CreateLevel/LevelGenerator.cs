using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{

    string[] names = { "jayden", "braden", "aiden", "kaden", "hunter", "hayden", "bentley", "tristan" };

    public float width = 10;
    public float height = 10;
    public NavMeshSurface surface;
    public GameObject wall;
    public GameObject[] obstacles;
    public Material[] mats;
    Transform center;
    public GameObject agentPrefab, playerPrefab;
    public int NoOfFollowers = 30, NoOfPlayers = 4;
    GameObject agents, players;
    public GameObject mainPlayer, canvas, scoreCanvas, player1Score, timer;
    public List<int> scores;
    public List<GameObject> playerList;
    public List<Text> ScoreText;
    bool ScoringStarted = false;
    //public GameObject player;

    private bool playerSpawned = false;

    private void Awake()
    {
        var go = GameObject.Find("MainMenu");
        if (go != null)
        {
            NoOfPlayers = go.GetComponent<MainMenu>().n1 - 1;
            NoOfFollowers = go.GetComponent<MainMenu>().n2;
            Destroy(go);
        }
        
    }
    // Use this for initialization
    public void generateLevel()
    {
        //get center of the mesh and assign its position to navmesh builder
        center = GetComponent<CreateFloor>().centee;
        surface.gameObject.transform.position = new Vector3(center.position.x, 0.01f, center.position.z);
        //GenerateLevel();
        MakeCity();
        //make NavMesh on Generated Road with obstacles 
        surface.BuildNavMesh();
        
        SpawnAgents(NoOfFollowers);
        playerList.Add(mainPlayer);
        scores.Add(mainPlayer.transform.GetChild(1).GetComponent<ScoreScript>().PlayerScore);
        SpawnPlayers(NoOfPlayers);
        //spawn player in middle of the scene
        mainPlayer.transform.position = center.position;

        //disable scene building objects
        center.gameObject.SetActive(false);
        canvas.SetActive(false);

        //enable and generate game UI, start Timer
        scoreCanvas.SetActive(true);
        GenerateScoreCard();
        mainPlayer.SetActive(true);
        timer.GetComponent<Timer>().StartTimer();
        ScoringStarted = true;
        
    }

    // Create a grid based level
    void MakeCity()
    {
        int i = 1, j = 1;
        for (float x = 0; x <= width; x += .3f)
        {
            for (float y = 0; y <= height; y += .3f)
            {
                var r = Random.Range(3, 7);
                if ((i+j) % r == 0)
                {
                    // Spawn a wall
                    Vector3 pos = new Vector3(x - width / 2f, 0.01f, y - height / 2f);
                    var go = Instantiate(wall, pos + center.position, Quaternion.identity, transform);
                    go.transform.GetChild(0).GetComponent<Renderer>().material = mats[r - 3];
                }
                j++;
            }
            i++;
        }
    }

    //Spawn Agents and Players randomly on the navmesh using NavMeshHit
    void SpawnAgents(int n)
    {
        agents = new GameObject("agents");
        var x = width;
        if (height < width) x = height;

        for (int i = 0; i < n; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * x;
            randomDirection += center.transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, x, 1))
            {
                finalPosition = hit.position;
            }
            Instantiate(agentPrefab, finalPosition, Quaternion.identity, agents.transform);
        }
    }

    void SpawnPlayers(int m)
    {
        players = new GameObject("players");

        var x = width;
        if (height < width) x = height;

        for (int i = 0; i < m; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * x;
            randomDirection += center.transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, x, 1))
            {
                finalPosition = hit.position;
            }
            var player = Instantiate(playerPrefab, finalPosition, Quaternion.identity, players.transform);
            player.name = names[i];
            playerList.Add(player);
            scores.Add(player.transform.GetChild(1).GetComponent<ScoreScript>().PlayerScore);
        }
    }

    void GenerateScoreCard()
    {
        ScoreText.Add(player1Score.GetComponent<Text>());
        var pos = player1Score.transform.position;
        for(int i = 1; i<playerList.Count; i++)
        {
            var go = Instantiate(player1Score, scoreCanvas.transform);
            pos = new Vector3(pos.x, pos.y - 100, pos.z);
            go.transform.position = pos;
            ScoreText.Add(go.GetComponent<Text>());
        }
    }

    private void Update()
    {
        if (ScoringStarted)
        {
            for(int i = 0; i<playerList.Count; i++)
            {
                scores[i] = playerList[i].transform.GetChild(1).GetComponent<ScoreScript>().PlayerScore;
                ScoreText[i].text = playerList[i].name + " : "+ scores[i].ToString();
            }
        }
    }
}

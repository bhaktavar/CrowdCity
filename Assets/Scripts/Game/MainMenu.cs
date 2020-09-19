using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public InputField NoOfPlayers, NoOfAgents;
    public int n1 = 4, n2 = 40;
    public GameObject instructions;
    public bool toggle = false;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartGame()
    {
        n1 = int.Parse(NoOfPlayers.text);
        n2 = int.Parse(NoOfAgents.text);
        SceneManager.LoadScene("HelloAR");
    }

    public void ShowInstructions()
    {
        toggle = !toggle;
        instructions.SetActive(toggle);
    }
}

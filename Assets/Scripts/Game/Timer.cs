using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Timer : MonoBehaviour
{
    public float timeStart;
    public float maxTime = 30f;
    public Text textBox;
    public Text resultText;
    public GameObject winnerUI;
    public LevelGenerator GO;
    bool timerActive = false;

    // Use this for initialization
    void Start()
    {
        textBox.text = timeStart.ToString("F0");
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timeStart += Time.deltaTime;
            textBox.text = timeStart.ToString("F0");
        }

        if (timeStart >= maxTime)
        {
            EndGame();
        }
    }
    public void StartTimer()
    {
        timerActive = !timerActive;
    }

    public void EndGame()
    {
        winnerUI.SetActive(true);
        Time.timeScale = 0f;
        int maxValue = GO.scores.ToArray().Max();
        if (GO.scores.IndexOf(maxValue) == 0)
        {
            resultText.text = "YOU WIN!!";
        }
        else
        {
            resultText.text = "YOU LOSE!!";
        }
    }
    public void Replay()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("HelloAR");
    }
}

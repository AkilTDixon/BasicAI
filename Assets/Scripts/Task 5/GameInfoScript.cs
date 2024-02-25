using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameInfoScript : MonoBehaviour
{
    public static GameInfoScript Instance { get; private set; }

    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public List<GameObject> heroObjects;
    [SerializeField] public GameObject playerObject;
    [SerializeField] public List<GameObject> guardianHolders;
    [SerializeField] public GameObject guardianObject;
    [SerializeField] public GameObject GameOverHolder;
    [SerializeField] public GameObject VictoryHolder;
    [SerializeField] public GameObject ButtonsHolder;



    private int prisonerSavedCounter = 0;
    private int prisonerTakenCounter = 0;
    private int activePrisoners = 5;
    public int score = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        if (timerText)
            timerText.text = (Time.time).ToString("F2");
        if (scoreText)
        scoreText.text = score.ToString();
    }
    void Update()
    {
        if (activePrisoners <= 0)
        {   if (prisonerSavedCounter > prisonerTakenCounter)
                GameOver();
            else
                Victory();
        }
        if (timerText)
            timerText.text = (Time.time).ToString("F2");
    }
    public void GuardianDestroyed()
    {
        score++;
        scoreText.text = score.ToString();
    }

    //task 8
    public void GameOver(string scenename)
    {
        SceneManager.LoadScene(scenename);


    }
    public void GameOver()
    {
        Time.timeScale = 0;
        GameOverHolder.SetActive(true);
        ButtonsHolder.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }
    public void Victory()
    {
        Time.timeScale = 0;
        VictoryHolder.SetActive(true);
        ButtonsHolder.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Continue(string sceneName)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        GameOverHolder.SetActive(false);
        VictoryHolder.SetActive(false);
        ButtonsHolder.SetActive(false);
        Time.timeScale = 1;


        score = 0;
        prisonerSavedCounter = 0;
        prisonerTakenCounter = 0;
        activePrisoners = 5;

    SceneManager.LoadScene(sceneName);
        

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    //Task 9
    public void PrisonerSaved(GameObject prisoner)
    {
        for (int i = 0; i < heroObjects.Count; i++)
        {
            if (SceneManager.GetActiveScene().name == "Task 9")
                heroObjects[i].GetComponent<HeroScript9>().saved(prisoner);
            else
                heroObjects[i].GetComponent<HeroScript10>().saved(prisoner);

        }
        prisonerSavedCounter++;
        activePrisoners--;
        score += 25;
        scoreText.text = score.ToString();
    }
    public void HeroCaptured(GameObject hero)
    {
        heroObjects.Remove(hero);
        if (SceneManager.GetActiveScene().name == "Task 9")
            hero.GetComponent<AIBaseScript9>().enabled = false;
        else
            hero.GetComponent<AIBaseScript10>().enabled = false;
        Destroy(hero);
        if (heroObjects.Count == 0)
        {
            if (SceneManager.GetActiveScene().name == "Task 9")
                GameOver("Task 9");
            else
                GameOver();
        }
    }

    public void HeroCaptured2(GameObject hero)
    {
        HeroScript10 hs = hero.GetComponent<HeroScript10>();
        hero.transform.position = hs.BasePoint.transform.position;
        Vector3 temp = hero.transform.position;
        temp.y = 5.72f;
        hero.transform.position = temp;

        hs.agent.pathCounter = 0;
        hs.pathPoints.Clear();
        hs.foundPath = false;
        hs.GetClosestPrisoner();
    }

    public void PrisonerTaken()
    {
        activePrisoners--;
        prisonerTakenCounter++;
    }
    public void HeroCapturedByPlayer(GameObject hero)
    {
        HeroScript10 hs = hero.GetComponent<HeroScript10>();
        hero.transform.position = hs.BasePoint.transform.position;

        Vector3 temp = hero.transform.position;
        temp.y = 5.72f;
        hero.transform.position = temp;

        hs.agent.pathCounter = 0;
        hs.pathPoints.Clear();
        hs.foundPath = false;
        hs.GetClosestPrisoner();
        SpawnGuardian();

        score -= 10;
        scoreText.text = score.ToString();
    }
    public void SpawnGuardian()
    {
        int r = Random.Range(0, guardianHolders.Count);
        if (guardianHolders.Count > 0)
        {
            guardianHolders[r].GetComponent<SpawnScript>().SpawnGuardian(guardianObject);
        }
    }
}

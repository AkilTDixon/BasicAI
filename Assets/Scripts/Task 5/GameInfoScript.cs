using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameInfoScript : MonoBehaviour
{
    public static GameInfoScript Instance { get; private set; }

    [SerializeField] public TextMeshProUGUI scoreText;


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
        scoreText.text = score.ToString();
    }
    public void GuardianDestroyed()
    {
        score++;
        scoreText.text = score.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayingPanle : MonoBehaviour
{

    [SerializeField] TMP_Text scoreText;

    public void Init()
    {
        SetScoreText(GameManager.instance.gamePlayManager.score);
    }

    
    void Update()
    {
        
    }

    public void SetScoreText(int _value)
    {
        scoreText.text = "Score: " + _value;
    }
}

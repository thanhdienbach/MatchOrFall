using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{

    public int score;
    public void Init()
    {
        score = 0;
    }

    void AddScore(int _value)
    {
        score += _value;
        GameManager.instance.uIManager.playingPanle.SetScoreText(_value);
    }
}

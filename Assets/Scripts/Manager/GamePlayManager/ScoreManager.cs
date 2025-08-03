using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] PlayingPanle playingPanle;
    [SerializeField] Transform canvasTransform;

    [Header("Score variable")]
    public int currentScore;
    public int clearRowScore = 10;
    public int clearnumberScore = 10;
    [SerializeField] GameObject scorePopupPrefabs;

    [Header("Combo score variable")]
    public int bonus1Score = 5;
    public int bonus2Score = 10;
    public int extraScore = 100;
    public int bonus1Hope = 1;
    public int bonus2Hope = 1;
    public int extraHope = 3;

    public void Init()
    {
        currentScore = 0;

        playingPanle = GameManager.instance.uIManager.playingPanle;
    }

    public void AddScoreHandle(int _value, Button _cell1, Button _cell2)
    {
        currentScore += _value;
        playingPanle.SetScoreText(currentScore);
        ShowScoreAtMidTowPoint(_value, _cell1, _cell2);
    }
    public void ShowScoreAtMidTowPoint(int _value, Button _cell1, Button _cell2)
    {
        Vector2 midWorldPoint = CalulatePoitToShowScore(_cell1, _cell2);
        ShowScoreAt(midWorldPoint, _value);
    }
    Vector2 CalulatePoitToShowScore(Button _cell1, Button _cell2)
    {
        Vector2 screenPos1 = _cell1.GetComponent<RectTransform>().position;
        Vector2 screenPos2 = _cell2.GetComponent<RectTransform>().position;

        return (screenPos1 + screenPos2) / 2f;
    }
    void ShowScoreAt(Vector2 _screenPosition, int _score)
    {
        GameObject scorePopup = Instantiate(scorePopupPrefabs);
        scorePopup.GetComponent<ScorePopup>().ShowScore(_score, _screenPosition, canvasTransform);
    }


    public void AddScoreCountUpModeHandle(int _value, Button _cell1, Button _cell2)
    {
        currentScore += _value;
        playingPanle.SetScoreText(currentScore);
        ShowScoreAndCountUpScoreAtMidTowPoint(_value, _cell1, _cell2);
    }

    public void ShowScoreAndCountUpScoreAtMidTowPoint(int _value, Button _cell1, Button _cell2)
    {
        Vector2 midWorldPoint = CalulatePoitToShowScore(_cell1, _cell2);
        ShowScoreAtAndCountUpScore(midWorldPoint, _value);
    }
    void ShowScoreAtAndCountUpScore(Vector2 _screenPosition, int _score)
    {
        GameObject scorePopup = Instantiate(scorePopupPrefabs);
        scorePopup.GetComponent<ScorePopup>().ShowScoreWithCountUpScore(_score, _screenPosition, canvasTransform);
    }

}



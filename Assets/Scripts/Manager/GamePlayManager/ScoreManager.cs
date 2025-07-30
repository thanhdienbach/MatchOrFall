using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] PlayingPanle playingPanle;
    [SerializeField] Transform canvasTransform;

    [Header("Score variable")]
    public int currentScore;
    [SerializeField] int addScore;
    [SerializeField] GameObject scorePopupPrefabs;

    public void Init()
    {
        currentScore = 0;
        addScore = 0;

        playingPanle = GameManager.instance.uIManager.playingPanle;
    }

    public void AddScoreHandle(int _value, Cell _cell1, Cell _cell2)
    {
        currentScore += _value;
        playingPanle.SetScoreText(currentScore);

        Vector2 midWorldPoint = CalulatePoitToShowScore(_cell1, _cell2);
        ShowScoreAt(midWorldPoint, _value);

        addScore = 0;
    }
    Vector2 CalulatePoitToShowScore(Cell _cell1, Cell _cell2)
    {
        Vector2 screenPos1 = _cell1.button.GetComponent<RectTransform>().position;
        Vector2 screenPos2 = _cell2.button.GetComponent<RectTransform>().position;

        return (screenPos1 + screenPos2) / 2f;
    }
    void ShowScoreAt(Vector2 _screenPosition, int _score)
    {
        GameObject scorePopup = Instantiate(scorePopupPrefabs);
        scorePopup.GetComponent<ScorePopup>().ShowScore(_score, _screenPosition, canvasTransform);
    }
}

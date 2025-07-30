using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayingPanle : MonoBehaviour
{

    [SerializeField] TMP_Text scoreText;

    [SerializeField] TMP_Text hopeText;
    [SerializeField] Button hopeButton;

    [SerializeField] TMP_Text addNumbersText;
    [SerializeField] Button addNumbersButton;

    public void Init()
    {
        
    }

    public void SetScoreText(int _value)
    {
        scoreText.text = "Score: " + _value;
    }
    public void SetHopeText(int _value, bool _isZero)
    {
        hopeText.text = _value.ToString();
        if(_isZero)
        {
            hopeButton.interactable = false;
        }
        else
        {
            hopeButton.interactable = true;
        }
    }
    public void SetAddNumberText(int _value, bool _isZero)
    {
        addNumbersText.text = _value.ToString();
        if(_isZero)
        {
            addNumbersButton.interactable = false;
        }
        else
        {
            addNumbersButton.interactable = true;
        }
    }
}

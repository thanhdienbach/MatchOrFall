using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayingPanle : MonoBehaviour
{

    public List<Cell> clearedNumbers = new List<Cell>();
    public Dictionary<int, int> countNumber = new Dictionary<int, int>();
    public GridLayoutGroup clearNumbersGridLayoutGroup;
    [SerializeField] ScoreManager scoreManager;

 
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
    public void DuplicateCountNumber()
    {
        for (int i = 0; i < countNumber.Count; i++)
        {
            countNumber[i + 1] *= 2;
        }
    }

    public void UpdateCountNumbers(int _number1, int _number2)
    {
        countNumber[_number1]--;
        countNumber[_number2]--;
        if (countNumber[_number1] == 0)
        {
            
        }
        if (countNumber[_number2] == 0 && _number1 != _number2)
        {
            Debug.Log("Clear number:" + _number2);
        }
    }

}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanle : MonoBehaviour
{
    public TMP_Text roundText;
    public TMP_Text scoreText;
    public TMP_Text comboText;

    public TMP_Text roundHighestText;
    public TMP_Text scoreHighestText;
    public TMP_Text comboHighestText;

    public Image roundMedal;
    public Image scoreMedal;
    public Image comboMedal;

    public Button backButton;
    public Button playAgainButton;


    public void AnimationScore(TMP_Text _text, int _value)
    {
        int currentValue = 0;

        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            _text.text = currentValue.ToString();
        }, _value, 1.0f);
        
    }
}

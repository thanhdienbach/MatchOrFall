using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI.Extensions;


[System.Serializable]
public class Cell
{
    public Button button;
    public int number;
    public TMP_Text text;
    public Vector2Int cellPosition;
    public bool isMatched = false;
    public ParticleSystem matchingEffect;

    public Cell(Button _button)
    {
        button = _button;
    }

    public Cell(Button _button, int _number, TMP_Text _text, Vector2Int _cellPosition, bool _isMatched)
    {
        button = _button;
        number = _number;
        text = _text;
        cellPosition = _cellPosition;
        isMatched = _isMatched;
    }

    public void PlayMatchingEffect()
    {
        if (matchingEffect != null)
        {
            matchingEffect.Play();
        }
    }

}

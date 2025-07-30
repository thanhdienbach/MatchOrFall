using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text text;

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponent<TMP_Text>();
    }
    void Start()
    {
        
    }

    public void ShowScore(int _score, Vector3 _screenPosition, Transform parentCanvas)
    {
        text.text = "+" + _score;
        transform.SetParent(parentCanvas, false);
        transform.position = _screenPosition;

        canvasGroup.alpha = 0;
        transform.localScale = Vector3.one;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1.0f, 0.3f));
        sequence.Join(transform.DOMoveY(transform.position.y + 50f, 0.6f));
        sequence.AppendInterval(0.3f);
        sequence.Append(canvasGroup.DOFade(0f, 0.3f));
        sequence.OnComplete(() => Destroy(gameObject));
    }
}

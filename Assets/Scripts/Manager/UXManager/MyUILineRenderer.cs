using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class MyUILineRenderer : MonoBehaviour
{
    [SerializeField] UILineRenderer uILineRenderer;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] float lineLiveTime = 0.5f;
    [SerializeField] float nextTimeCalcleLine;
    


    public void Init()
    {
        uILineRenderer = GetComponent<UILineRenderer>();
        mainCanvas = GetComponentInParent<Canvas>();
        uILineRenderer.LineThickness = 15f;
        uILineRenderer.color = ColorPalette.buleGreen;
    }
    public void DrawnLine(List<Cell> _matchedCells)
    {
        Vector2[] positions = new Vector2[_matchedCells.Count];

        for (int i = 0; i < _matchedCells.Count; i++)
        {
            RectTransform rectTransform = _matchedCells[i].button.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform, rectTransform.position, null, out positions[i]);
        }

        uILineRenderer.Points = positions;
        nextTimeCalcleLine = Time.time + lineLiveTime;
    }

    public void DrawnLineByPosition(Vector2 _positionA, Vector2 _positionB)
    {
        uILineRenderer.Points = new Vector2[]{_positionA, _positionB};
        nextTimeCalcleLine = Time.time + lineLiveTime;
    }

    public void ClearLine()
    {
        uILineRenderer.Points = new Vector2[0];
    }

    public void Update()
    {
        if (nextTimeCalcleLine < Time.time)
        {
            uILineRenderer.Points = new Vector2[0];
        }
    }
}

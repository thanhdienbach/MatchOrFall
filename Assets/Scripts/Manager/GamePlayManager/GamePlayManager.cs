using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class GamePlayManager : MonoBehaviour
{

    public int score;
    [SerializeField] CellClickHandler firstCell;
    [SerializeField] CellClickHandler secondCell;
    [SerializeField] int maxSum;
    [SerializeField] List<Cell> cells;

    [SerializeField] GridAnimation gridAnimation;


    public void Init()
    {
        score = 0;
        maxSum = GameManager.instance.boardManager.gridConfig.maxValueNumber + 1;
        gridAnimation = GameManager.instance.uIManager.gridAnimation;
        cells = GameManager.instance.boardManager.cells;
    }

    void AddScore(int _value)
    {
        score += _value;
        GameManager.instance.uIManager.playingPanle.SetScoreText(score);
    }

    public void OnCellClick(CellClickHandler _clicker)
    {
        HightLightCell(_clicker.cell);

        if (firstCell == null)
        {
            firstCell = _clicker;
        }
        else if (secondCell == null && _clicker != firstCell)
        {
            secondCell = _clicker;
            if (CheckPair(firstCell.cell.number, secondCell.cell.number))
            {
                if (RightWay(firstCell.cell, secondCell.cell))
                {
                    // Update point 
                    // Xứ lý 2 ô 
                    //Resset trạng thái

                    HandlingWhenMatching(firstCell.cell, secondCell.cell);
                    Debug.Log("Finded");
                }
                else
                {
                    HandlingWhenNotMatching(firstCell.cell, secondCell.cell);
                }
            }
            else
            {
                ReLightCell(firstCell.cell);
                firstCell = secondCell;
                secondCell = null;
            }
        }
    }

    void HightLightCell(Cell _cell)
    {
        _cell.button.GetComponent<Image>().color = ColorPalette.lightBlue;
    }
    void ReLightCell(Cell _cell)
    {
        _cell.button.GetComponent<Image>().color = Color.white;
    }

    bool CheckPair(int _value1, int _value2)
    {
        return _value1 == _value2 || _value1 + _value2 == maxSum;
    }

    bool RightWay(Cell _cell1, Cell _cell2)
    {
        if (Adjacent(_cell1, _cell2))
        {
            return true;
        }
        else if (ClearPath(_cell1, _cell2))
        {
            return true;
        }
        else if(ClearPathDifferentRow(_cell1, _cell2))
        {
            return true;
        }
        return false;
    }
    bool Adjacent(Cell _cell1, Cell _cell2)
    {
        return Math.Abs(_cell1.cellPosition.x - _cell2.cellPosition.x) <= 1 && Math.Abs(_cell1.cellPosition.y - _cell2.cellPosition.y) <= 1;
    }
    bool ClearPath(Cell _cell1, Cell _cell2)
    {
        int dx = Math.Sign(_cell2.cellPosition.x - _cell1.cellPosition.x);
        int dy = Math.Sign(_cell2.cellPosition.y - _cell1.cellPosition.y);

        if (!(dx == 0 || dy == 0 || Mathf.Abs(_cell2.cellPosition.x - _cell1.cellPosition.x) == Mathf.Abs(_cell2.cellPosition.y - _cell1.cellPosition.y)))
        {
            return false;
        }

        Vector2Int position = _cell1.cellPosition + new Vector2Int(dx, dy);

        while (position != _cell2.cellPosition)
        {
            Cell currentCell = GetCellAt(position);
            if (currentCell != null && !currentCell.isMatched)
            {
                return false;
            }
            position += new Vector2Int(dx, dy);
        }
        return true;
    }
    Cell GetCellAt(Vector2Int _position)
    {
        foreach(var cell in cells)
        {
            if (cell.cellPosition == _position)
            {
                return cell;
            }
        }
        return null;
    }
    bool ClearPathDifferentRow(Cell _cell1, Cell _cell2)
    {
        int index1 = cells.IndexOf(_cell1);
        int index2 = cells.IndexOf(_cell2);
        int dx = Math.Sign(index2 - index1);

        int currentIndex = index1 + dx;

        while (currentIndex > 0 && currentIndex < cells.Count)
        {
            
            Cell curentCell = cells[currentIndex];

            if (curentCell == _cell2)
            {
                return true;
            }
            if (!curentCell.isMatched)
            {
                return false;
            }

            currentIndex += dx;
            
        }
        return false;
    }
    void HandlingWhenMatching(Cell _cell1, Cell _cell2)
    {
        ReLightCell(_cell1);
        ReLightCell(_cell2);
        _cell1.button.interactable = false;
        _cell2.button.interactable = false;
        _cell1.isMatched = true;
        _cell2.isMatched = true;
        firstCell = secondCell = null;
        AddScore(1); // Hệ thống tính điểm
    }
    void HandlingWhenNotMatching(Cell _cell1, Cell _cell2)
    {
        gridAnimation.Ring();
        ReLightCell(_cell1);
        ReLightCell(_cell2);
        firstCell = secondCell = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;
using static UnityEditor.PlayerSettings;

public class GamePlayManager : MonoBehaviour
{

    public int score;
    public int hopeNumber;
    [SerializeField] CellClickHandler firstCell;
    [SerializeField] CellClickHandler secondCell;
    [SerializeField] int maxSum;
    [SerializeField] List<Cell> cells;

    [SerializeField] GridAnimation gridAnimation;
    [SerializeField] List<Vector3> linePositions;
    [SerializeField] bool isSingleLine;

    [SerializeField] UIManager uIManager;
    [SerializeField] Canvas canvas;

    [SerializeField] PlayingPanle playingPanle;

    public void Init()
    {
        score = 0;
        hopeNumber = 300;
        maxSum = GameManager.instance.boardManager.gridConfig.maxValueNumber + 1;
        gridAnimation = GameManager.instance.uIManager.gridAnimation;
        cells = GameManager.instance.boardManager.cells;

        uIManager = GameManager.instance.uIManager;
        canvas = uIManager.GetComponent<Canvas>();
        playingPanle = uIManager.playingPanle;
        playingPanle.SetScoreText(score);
        playingPanle.SetHopeText(hopeNumber, false);
    }

    void AddScore(int _value)
    {
        score += _value;
        uIManager.playingPanle.SetScoreText(score);
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
                    HandlingWhenMatching(firstCell.cell, secondCell.cell);
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
        isSingleLine = true;

        return Math.Abs(_cell1.cellPosition.x - _cell2.cellPosition.x) <= 1 && Math.Abs(_cell1.cellPosition.y - _cell2.cellPosition.y) <= 1;
    }
    bool ClearPath(Cell _cell1, Cell _cell2)
    {
        isSingleLine = true;

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
        isSingleLine = false;

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
        HandleCell(_cell1, _cell2);

        if (isSingleLine)
        {
            DrawnOneLine(_cell1, _cell2);
        }
        else
        {
            DrawnDoubleLine(_cell1, _cell2, gridAnimation.GetComponent<RectTransform>());
        }
        
        AddScore(1); // Hệ thống tính điểm
    }
    void HandleCell(Cell _cell1, Cell _cell2)
    {
        _cell1.PlayMatchingEffect();
        _cell2.PlayMatchingEffect();
        BlurCell(_cell1, _cell2);
        _cell1.button.interactable = false;
        _cell2.button.interactable = false;
        _cell1.isMatched = true;
        _cell2.isMatched = true;
        firstCell = secondCell = null;
    }
    void BlurCell(Cell _cell1, Cell _cell2)
    {
        ReLightCell(_cell1);
        ReLightCell(_cell2);
        BlurText(_cell1);
        BlurText(_cell2);
    }
    void BlurText(Cell _cell)
    {
        Color curentColor = _cell.button.GetComponentInChildren<TMP_Text>().color;
        curentColor.a = 0.5f;
        _cell.button.GetComponentInChildren<TMP_Text>().color = curentColor;
    }
    void DrawnOneLine(Cell cell1, Cell cell2)
    {
        List<Cell> cells = new List<Cell>();
        cells.Add(cell1);
        cells.Add(cell2);
        uIManager.uILineRenderer1.DrawnLine(cells);
    }
    void DrawnDoubleLine(Cell _cell1, Cell _cell2, RectTransform _gridParentTransform)
    {
        Vector2 cell1Position = GetLocalCenter(_cell1.button.transform as RectTransform);
        Vector2 cell2Position = GetLocalCenter(_cell2.button.transform as RectTransform);

        float rightEdgeX = _gridParentTransform.rect.width / 2;
        float leftEdgeX = -_gridParentTransform.rect.width / 2;

        Vector2 rightExit = new Vector2(rightEdgeX, cell1Position.y);
        Vector2 leftEnter = new Vector2(leftEdgeX, cell2Position.y);

        uIManager.uILineRenderer1.DrawnLineByPosition(cell1Position, rightExit);
        uIManager.uILineRenderer2.DrawnLineByPosition(leftEnter, cell2Position);
    }
    Vector2 GetLocalCenter(RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, rectTransform.position, null, out Vector2 localPosition);
        return localPosition;
    }
    void HandlingWhenNotMatching(Cell _cell1, Cell _cell2)
    {
        gridAnimation.Ring();
        ReLightCell(_cell1);
        ReLightCell(_cell2);
        firstCell = secondCell = null;
    }


    public void HopeButtonClickEvent()
    {
        if (HasPairNumberCanMatching())
        {
            // Tô 2 ô có thể matching;
            UpdateHope(-1);
        }
        else
        {
            // Rung nút nhấn
        }
    }
    bool HasPairNumberCanMatching()
    {
        foreach (var cell  in cells)
        {
            if (cell.isMatched)
            {
                continue;
            }
            if (!cell.button.interactable)
            {
                continue;
            }
            if (!cell.isMatched)
            {
                Debug.Log($"Cell đang sét{cell.cellPosition}");
                if (HasPathInList(cell))
                {
                    Debug.Log(HasPathInList(cell));
                    return true;
                }
                else if (HasPathInBoard(cell, cells, maxSum))
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool HasPathInBoard(Cell _cell, List<Cell> _cells, int _maxSum)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int (-1 ,-1),
            new Vector2Int (-1 ,0),
            new Vector2Int (-1 ,1),
            new Vector2Int (0 , -1),
            new Vector2Int (0 , 1),
            new Vector2Int (1 , -1),
            new Vector2Int (1, 0),
            new Vector2Int (1, 1)
        };

        foreach (var direction in directions)
        {
            Vector2Int position = _cell.cellPosition + direction;

            while (IsOnGrid(position))
            {
                Cell nextCell = GetCellAt(position);
                Debug.Log(nextCell.cellPosition);
                if (nextCell == null)
                {
                    // ()
                    break;
                }
                if (nextCell.isMatched == true)
                {
                    position += direction;
                    continue;
                }
                if (_cell.number != nextCell.number && _cell.number + nextCell.number != maxSum && nextCell.isMatched == false)
                {
                    break;
                }
                if (_cell.number == nextCell.number || _cell.number + nextCell.number == maxSum)
                {
                    Debug.Log($"Tìm thấy match từ {_cell.cellPosition} đến {nextCell.cellPosition}");
                    _cell.button.image.color = ColorPalette.lightBlue;
                    nextCell.button.image.color = ColorPalette.lightBlue;
                    return true;
                }

                position += direction;
            }
        }

        return false ;
    }
    bool IsOnGrid(Vector2Int _position)
    {
        Debug.Log($"{_position}");
        return _position.x > 0 && _position.y > 0 && _position.x < cells.Count / GameManager.instance.boardManager.cols && _position.y <= GameManager.instance.boardManager.cols;
    }
    bool HasPathInList(Cell _cell)
    {
        Debug.Log("Find with list");
        for (int i = cells.IndexOf(_cell) + 1; i < cells.Count; i++)
        {
            if (cells[i].isMatched)
            {
                Debug.Log(cells[i].isMatched);
                Debug.Log(cells[i].cellPosition);
                continue;
            }

            if (!cells[i].isMatched && (cells[i].number != _cell.number && cells[i].number + _cell.number != maxSum))
            {
                return false;
            }

            if (cells[i].number == _cell.number || cells[i].number + _cell.number == maxSum)
            {
                cells[i].button.image.color = ColorPalette.lightBlue;
                _cell.button.image.color = ColorPalette.lightBlue;
                return true;
            }
        }
        return false;
    }
    void UpdateHope(int _value)
    {
        hopeNumber += _value;
        playingPanle.SetHopeText(hopeNumber, hopeNumber == 0);
    }
}

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

    [Header("Component variable")]
    [SerializeField] GridAnimation gridAnimation;
    [SerializeField] UIManager uIManager;
    [SerializeField] Canvas canvas;
    [SerializeField] PlayingPanle playingPanle;
    [SerializeField] BoardManager boardManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] ScoreManager scoreManager;

    [Header("Match cell variable")]
    [SerializeField] CellClickHandler firstCell;
    [SerializeField] CellClickHandler secondCell;
    [SerializeField] List<Cell> cells;
    [SerializeField] int maxSum;

    [Header("Score variable")]
    [SerializeField] int addScore;

    [Header("Line renderer variable")]
    [SerializeField] bool isSingleLine;

    [Header("UI variable")]
    public int hopeNumber;
    public int addNunbersNumber;

    [Header("Board manager variable")]
    [SerializeField] int rowCanClear1 = 0;
    [SerializeField] int rowCanClear2 = 0;

    public void Init()
    {

        hopeNumber = 10;
        addNunbersNumber = 5;
        maxSum = GameManager.instance.boardManager.gridConfig.maxValueNumber + 1;

        scoreManager = GetComponent<ScoreManager>();
        scoreManager.Init();

        uIManager = GameManager.instance.uIManager;
        canvas = uIManager.GetComponent<Canvas>();
        boardManager = GameManager.instance.GetComponentInChildren<BoardManager>();
        playingPanle = uIManager.playingPanle;
        playingPanle.SetScoreText(scoreManager.currentScore);
        playingPanle.SetHopeText(hopeNumber, false);
        playingPanle.SetAddNumberText(addNunbersNumber, false);
        audioManager = GameManager.instance.audioManager;
        gridAnimation = GameManager.instance.uIManager.gridAnimation;
        cells = GameManager.instance.boardManager.cells;

    }

    public void OnCellClick(CellClickHandler _clicker)
    {
        HightLightCell(_clicker.cell);
        
        if (firstCell == null)
        {
            audioManager.PlayNumberTouch(_clicker.cell.number);
            firstCell = _clicker;
        }
        else if (secondCell == null && _clicker != firstCell)
        {
            secondCell = _clicker;
            if (CheckPair(firstCell.cell.number, secondCell.cell.number))
            {
                if (RightWay(firstCell.cell, secondCell.cell))
                {
                    HandlingWhenMatching(firstCell.cell, secondCell.cell);
                }
                else
                {
                    audioManager.PlayNotMatchingCellOneShot();
                    HandlingWhenNotMatching(firstCell.cell, secondCell.cell);
                }
            }
            else
            {
                audioManager.PlayNumberTouch(_clicker.cell.number);
                ReLightCell(firstCell.cell);
                firstCell = secondCell;
                secondCell = null;
            }
        }
    }

    void HightLightCell(Cell _cell)
    {
        _cell.button.GetComponent<Image>().color = ColorPalette.buleMint;
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
            audioManager.PlayMatchingCell1OneShot();
            return true;
        }
        else if (ClearPath(_cell1, _cell2))
        {
            audioManager.PlayMatchingCell2OneShot();
            return true;
        }
        else if(ClearPathDifferentRow(_cell1, _cell2))
        {
            audioManager.PlayerMatchingCellInListOneShot();
            return true;
        }
        return false;
    }
    bool Adjacent(Cell _cell1, Cell _cell2)
    {
        isSingleLine = true;
        addScore = 1;

        return Math.Abs(_cell1.cellPosition.x - _cell2.cellPosition.x) <= 1 && Math.Abs(_cell1.cellPosition.y - _cell2.cellPosition.y) <= 1;
    }
    bool ClearPath(Cell _cell1, Cell _cell2)
    {
        isSingleLine = true;
        addScore = 4;

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
        addScore = 4;

        Cell temp = _cell2;
        _cell2 = _cell1;
        _cell1 = temp;

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
        
        if (IsCleanedRows(_cell1, _cell2))
        {
            if (rowCanClear1 > 0)
            {
                HandleClearRow(rowCanClear1);
            }
            if (rowCanClear2 > 0)
            {
                HandleClearRow(rowCanClear2);
            }
        }

        scoreManager.AddScoreHandle(addScore, firstCell.cell, secondCell.cell);
        firstCell = secondCell = null;
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
        if (firstCell != null)
        {
            firstCell.cell.button.image.color = Color.white;
            firstCell = null;
        }
        
        if (HasPairNumberCanMatching())
        {
            audioManager.PlayHopeOneShot();
            UpdateHope(-1);
        }
        else
        {
            audioManager.PlayNotMatchingCellOneShot();
            gridAnimation.Ring();
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
                if (nextCell == null)
                {
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
                    _cell.button.image.color = ColorPalette.statYellow;
                    nextCell.button.image.color = ColorPalette.statYellow;
                    return true;
                }

                position += direction;
            }
        }

        return false ;
    }
    bool IsOnGrid(Vector2Int _position)
    {
        return _position.x > 0 && _position.y > 0 && _position.x < cells.Count / GameManager.instance.boardManager.cols && _position.y <= GameManager.instance.boardManager.cols;
    }
    bool HasPathInList(Cell _cell)
    {
        for (int i = cells.IndexOf(_cell) + 1; i < cells.Count; i++)
        {
            if (cells[i].isMatched)
            {
                continue;
            }

            if (!cells[i].isMatched && (cells[i].number != _cell.number && cells[i].number + _cell.number != maxSum))
            {
                return false;
            }

            if (cells[i].number == _cell.number || cells[i].number + _cell.number == maxSum)
            {
                cells[i].button.image.color = ColorPalette.statYellow;
                _cell.button.image.color = ColorPalette.statYellow;
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

    public void AddNumbersButtonCleckEvent()
    {
        boardManager.GenerateCell();
        addNunbersNumber -= 1;
        playingPanle.SetAddNumberText(addNunbersNumber, addNunbersNumber == 0);
    }
    void AddNumbersNumber(int _value, bool _isZero)
    {
        addNunbersNumber += _value;
        playingPanle.SetAddNumberText(addNunbersNumber, addNunbersNumber == 0);
    }

    /// <summary>
    /// Check rows have cell matching. If all cell are matching => Dellet all cells in row 
    /// </summary>
    /// <param name="_cell1"></param> 
    /// <param name="_cell2"></param>
    /// <returns></returns>
    bool IsCleanedRows(Cell _cell1, Cell _cell2)
    {
        rowCanClear1 = 0;
        rowCanClear2 = 0;
        int row1 = _cell1.cellPosition.x;
        int row2 = _cell2.cellPosition.x;
        Debug.Log(row1);
        Debug.Log(row2);
        if (row1 != row2)
        {
            Debug.Log("Case double check");
            bool isClearRow1 = IsClearRow(row1);
            if (isClearRow1)
            {
                rowCanClear1 = row1;
            }
            bool isClearRow2 = IsClearRow(row2);
            if (isClearRow2)
            {
                rowCanClear2 = row2;
            }
            return isClearRow1 || isClearRow2;
        }
        else if (row1 == row2)
        {
            bool isClearRow1 = IsClearRow(row1);
            if (isClearRow1)
            {
                rowCanClear1 = row1;
            }
            return isClearRow1;
        }

        return false;
    }
    bool IsClearRow(int _row)
    {
        Debug.Log($"Check clearow at row: " +_row);
        int countMatchingCell = 0;
        Vector2Int startPosition = new Vector2Int(_row, 1);
        int startIndex = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].cellPosition == startPosition)
            {
                startIndex = i;
                Debug.Log($"Start index: " + i);
                break;
            }
        }

        for (int i = startIndex; i < startIndex + boardManager.cols; i++)
        {
            Debug.Log($"Current index: " + i + cells[i].isMatched);
            if (cells[i].isMatched || cells[i].number == 0)
            {
                countMatchingCell += 1;
                Debug.Log($"Matching cell: {countMatchingCell} at {cells[i].cellPosition}");
            }
        }

        return countMatchingCell == boardManager.cols;
    }

    void HandleClearRow(int _rowNumber)
    {
        boardManager.AddOneRow();
        List<Cell> cellToremove = new List<Cell>();
        foreach (var cell in cells)
        {
            if (cell.cellPosition.x == _rowNumber)
            {
                cellToremove.Add(cell);
                Destroy(cell.button.gameObject);
            }
            else if (cell.cellPosition.x > _rowNumber)
            {
                cell.cellPosition.x -= 1;
            }
        }
        foreach (var cell in cellToremove)
        {
            cells.Remove(cell);
        }
    }
}

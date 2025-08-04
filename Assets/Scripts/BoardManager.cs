using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;
using System.Collections;

public class BoardManager : MonoBehaviour
{

    [Header("Component")]
    public GridConfig gridConfig;
    [SerializeField] GridLayoutGroup playingBoardGridLayoutGroup;
    [SerializeField] PlayingPanle playingPanle;
    [SerializeField] AudioManager audioManager;
    [SerializeField] GamePlayManager gamePlayManager;

    [Header("Gameobject")]
    public List<Cell> cells = new List<Cell>();
    [SerializeField] GameObject cellPrefabs;
    [SerializeField] Transform gridParent;

    [Header("Grid size")]
    public int cols;
    [SerializeField] int startFilledCell;
    [SerializeField] int maxNumber;
    

    [Header("Cell size")]
    public float cellFullPlace;
    [SerializeField] float cellPlace;
    [SerializeField] float spaceOfCell = 1.0f;

    [Header("Add numbers variable")]
    [SerializeField] List<Cell> addNumbers;
    [SerializeField] int maxRowCanShowInBoard = 12; // Todo: Caculate to scale board
    [SerializeField] List<int> randomNumbers = new List<int>();
    public Vector2Int[] directions = new Vector2Int[]
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

    [Header("Other")]
    public float nextTimeCanCheckClearRound;
    public float pendingTime = 10.0f;

    public void Init()
    {
        nextTimeCanCheckClearRound = pendingTime + Time.deltaTime;
        cols = gridConfig.cols;
        startFilledCell = gridConfig.startFilledCell;
        maxNumber = gridConfig.maxValueNumber;

        audioManager = GameManager.instance.audioManager;
        playingPanle = GameManager.instance.uIManager.playingPanle;
        gamePlayManager = GameManager.instance.gamePlayManager;

        SetUpLayOutGrid();
        GeneratePlayingBoard();
        StartCoroutineFillNumberWithDifficuiltValue(1);
        GenerateClearNumbersBoard();
    }

    public void ResetNewRound()
    {
        foreach (var cell in cells)
        {
            Destroy(cell.button.gameObject);
        }
        cells.Clear();

        GeneratePlayingBoard();

        for (int i = 0; i < gridConfig.maxValueNumber; i++)
        {
            playingPanle.clearedNumbers[i].number = i + 1;
            playingPanle.clearedNumbers[i].button.GetComponent<CanvasGroup>().alpha = 1;
            playingPanle.countNumber[i + 1] = 0;
        }
        StartCoroutineFillNumberWithDifficuiltValue(1);
    }

    void SetUpLayOutGrid()
    {
        playingBoardGridLayoutGroup.constraintCount = cols;

        cellFullPlace = gridParent.GetComponent<RectTransform>().rect.width / playingBoardGridLayoutGroup.constraintCount;
        cellPlace = cellFullPlace - spaceOfCell;

        playingBoardGridLayoutGroup.cellSize = new Vector2(cellPlace, cellPlace);
        playingBoardGridLayoutGroup.spacing = new Vector2(spaceOfCell, spaceOfCell);

    }
    public void GeneratePlayingBoard()
    {
        int rows;
        if ((startFilledCell / cols) < 12)
        {
            rows = 12;
        }
        else
        {
            rows = startFilledCell / cols;
        }
        for (int i = 0; i < cols * rows; i++) // (startFilledCell / cols + 4) * cols
        {
            AddEmptyCell(i, cells, playingBoardGridLayoutGroup);
        }
    }
    void GenerateClearNumbersBoard()
    {
        playingPanle.clearNumbersGridLayoutGroup.constraintCount = gridConfig.maxValueNumber;

        Vector2 spaceing = playingPanle.clearNumbersGridLayoutGroup.spacing;
        RectOffset padding = playingPanle.clearNumbersGridLayoutGroup.padding;
        float spacesingx = (900 - gridConfig.maxValueNumber * playingPanle.clearNumbersGridLayoutGroup.cellSize.x) / (gridConfig.maxValueNumber);
        spaceing.x = spacesingx;
        padding.left = (int)spacesingx / 2;

        playingPanle.clearNumbersGridLayoutGroup.spacing = spaceing;
        playingPanle.clearNumbersGridLayoutGroup.padding = padding;

        for (int i = 0; i < gridConfig.maxValueNumber; i++)
        {
            AddEmptyCell(i,playingPanle.clearedNumbers, playingPanle.clearNumbersGridLayoutGroup);
            playingPanle.clearedNumbers[i].number = i + 1;
            playingPanle.clearedNumbers[i].button.GetComponentInChildren<TMP_Text>(true).text = (i + 1).ToString();
            playingPanle.countNumber[i + 1] = 0;
        }
    }
    public void AwakeClearNumbersBoard()
    {
        for (int i = 0; i < gridConfig.maxValueNumber;i++)
        {
            playingPanle.clearedNumbers[i].button.GetComponent<CanvasGroup>().alpha = 1.0f;
        }
    }
    public void GetValueToClearNumberList()
    {
        foreach (var cell in cells)
        {
            if (cell.number == 0)
            {
                break;
            }
            playingPanle.countNumber[cell.number]++;
        }
    }
    public void StartCoroutineFillNumberWithDifficuiltValue(int _value)
    {
        StartCoroutine(FillNumberWithDifficuiltValue(_value));
    }
    public IEnumerator FillNumberWithDifficuiltValue(int _difficuiltValue)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (i < startFilledCell)
            {
                for (int j = 0; j < _difficuiltValue; j++)
                {
                    int countCheckLoop = 0;

                    int randomNumber = Random.Range(1, maxNumber + 1);
                    cells[i].number = randomNumber;
                    PlayButtonSwapEffect(cells[i], randomNumber);
                    cells[i].button.interactable = true;
                    // cells[i].button.GetComponentInChildren<TMP_Text>().text = randomNumber.ToString();

                    foreach (var direction in directions)
                    {
                        Vector2Int position = cells[i].cellPosition + direction;
                        if (gamePlayManager.IsOnGrid(position))
                        {
                            Cell cell = gamePlayManager.GetCellAt(position);
                            if (cell.number != cells[i].number && cell.number + cells[i].number != maxNumber + 1)
                            {
                                countCheckLoop++;
                            }
                        }
                        else
                        {
                            countCheckLoop++;
                        }
                    }
                    if (countCheckLoop == 8)
                    {
                        j = _difficuiltValue;
                    }
                }
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            else
            {
                cells[i].button.GetComponentInChildren<TMP_Text>().text = "";
                cells[i].button.interactable = false;
            }
        }
        GetValueToClearNumberList();
    }
    void PlayButtonSwapEffect(Cell cell, int _number)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(cell.button.GetComponentInChildren<CanvasGroup>().DOFade(0f, 0.25f));
        sequence.AppendCallback(() => { cell.button.GetComponentInChildren<TMP_Text>().text = _number.ToString(); });

        sequence.Append(cell.button.GetComponentInChildren<CanvasGroup>().DOFade(1f, 0.25f));
    }
    /// <summary>
    /// Duplicate cells have isMatching = false and update countnumber(Dictionary)
    /// </summary>
    public void DuplicateCellsNotMatching()
    {
        // Find index of cell will add first value and clone numbers on board to list
        int indexOfCell = 0;
        if (cells[cells.Count - 1].number > 0)
        {
            indexOfCell = cells.Count;
        }
        foreach (var cell in cells)
        {
            if (cell.button.interactable)
            {
                addNumbers.Add(cell);
            }

            if (!cell.button.interactable && cell.number == 0)
            {
                indexOfCell = cells.IndexOf(cell);
                break;
            }
        }
        
        // Pass numbers from list to new cells
        for (int i = 0; i < addNumbers.Count; i++)
        {
            // Pass number to empty cell if board have empty cell
            if (indexOfCell < cells.Count)
            {
                cells[indexOfCell].button.interactable = true;
                cells[indexOfCell].number = addNumbers[i].number;
                cells[indexOfCell].button.GetComponentInChildren<TMP_Text>().text = addNumbers[i].number.ToString();
                ++indexOfCell;
            }
            // Creat new empty cell and pass number if board full cell
            else
            {
                AddEmptyCell(indexOfCell, cells, playingBoardGridLayoutGroup);
                cells.Last().button.interactable = true;
                cells.Last().number = addNumbers[i].number;
                cells.Last().button.GetComponentInChildren<TMP_Text>().text = addNumbers[i].number.ToString();
                ++indexOfCell;
            }
        }
        addNumbers.Clear();

        // Creat empty cells to fill last row of board
        int missingCellNumberInBoard = cols -  cells.Last().cellPosition.y;
        for (int i = 0; i < missingCellNumberInBoard; ++i)
        {
            AddEmptyCell(cells.Count, cells, playingBoardGridLayoutGroup);
        }
    }

    public void AddOneRowToPlayingBoard()
    {
        for (int i = 0; i < cols; ++i)
        {
            AddEmptyCell(cells.Count, cells, playingBoardGridLayoutGroup);
        }
    }
    void AddEmptyCell(int _indexOfCellOnList, List<Cell> _listDestination, GridLayoutGroup _gridLayoutGroupHanldeCell)
    {
        GameObject buttonObject = Instantiate(cellPrefabs, _gridLayoutGroupHanldeCell.transform);
        Button button = buttonObject.GetComponent<Button>();

        Cell cell = new Cell(button);

        int cellPositionX = _indexOfCellOnList / cols + 1;
        int cellPositionY = _indexOfCellOnList % cols + 1;

        Vector2Int cellPosition = new Vector2Int(cellPositionX, cellPositionY);
        cell.cellPosition = cellPosition;
        cell.button.GetComponent<CellClickHandler>().cell = cell;
        cell.button.interactable = false;
        _listDestination.Add(cell);
    }

    public void HandleClearRow(int _rowNumber)
    {

        if (cells.Last().cellPosition.x <= maxRowCanShowInBoard)
        {
            AddOneRowToPlayingBoard();
        }
        
        List<Cell> cellToRemove = new List<Cell>();
        foreach (var cell in cells)
        {
            if (cell.cellPosition.x == _rowNumber)
            {
                cellToRemove.Add(cell);
            }
            else if (cell.cellPosition.x > _rowNumber)
            {
                cell.cellPosition.x -= 1;
            }
        }

        ClearRow(cellToRemove);

        foreach (var cell in cellToRemove)
        {
            cells.Remove(cell);
        }
    }
    void ClearRow(List<Cell> _cells)
    {
        foreach (var cell in _cells)
        {
            ClearCell(cell);
        }
    }
    void ClearCell(Cell cell)
    {
        DG.Tweening.Sequence sequene = DOTween.Sequence();

        RectTransform rectTransform = cell.button.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = cell.button.GetComponent<CanvasGroup>();

        sequene.Append(rectTransform.DOShakeAnchorPos
            (
            duration: 0.3f,
            strength: new Vector2(10f, 10f),
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: true
            ));

        sequene.Append(rectTransform.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuad));
        sequene.Join(canvasGroup.DOFade(0f, 0.3f));

        sequene.OnComplete(() =>
        {
            audioManager.PlayClearRowOneShot();
            Destroy(cell.button.gameObject);
        });
    }

}

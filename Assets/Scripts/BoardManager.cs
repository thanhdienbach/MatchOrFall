using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting;

public class BoardManager : MonoBehaviour
{

    [Header("Component")]
    [SerializeField] GridConfig gridConfig;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Gameobject")]
    public List<Cell> cells = new List<Cell>();
    [SerializeField] GameObject cellPrefabs;
    [SerializeField] Transform gridParent;

    [Header("Grid size")]
    [SerializeField] int cols;
    [SerializeField] int startFilledCell;
    [SerializeField] int maxNumber;

    [Header("Cell size")]
    public float cellFullPlace;
    [SerializeField] float cellPlace;
    [SerializeField] float spaceOfCell = 1.0f;
    
    


    public void Init()
    {
        cols = gridConfig.cols;
        startFilledCell = gridConfig.startFilledCell;
        maxNumber = gridConfig.maxValueNumber;
        

        gridLayoutGroup = UIManager.instance.GetComponentInChildren<GridLayoutGroup>();

        SetUpLayOutGrid();
        GenerateGrid();
        FillRandomNumber(CreatRandomPairNumbers());
    }


    void SetUpLayOutGrid()
    {
        gridLayoutGroup.constraintCount = cols;

        cellFullPlace = gridParent.GetComponent<RectTransform>().rect.height / gridLayoutGroup.constraintCount;
        cellPlace = cellFullPlace - spaceOfCell;

        gridLayoutGroup.cellSize = new Vector2(cellPlace, cellPlace);
        gridLayoutGroup.spacing = new Vector2(spaceOfCell, spaceOfCell);

    }
    void GenerateGrid()
    {
        for (int i = 0; i < (startFilledCell / cols + 4) * cols; i++)
        {
            GameObject buttonObject = Instantiate(cellPrefabs, gridParent);
            Button button = buttonObject.GetComponent<Button>();

            Cell cell = new Cell(button);
            int cellPositionY;
            if ((i + 1) % cols == 0)
            {
                cellPositionY = cols;
            }
            else
            {
                cellPositionY = (i + 1) % cols;
            }
            Vector2Int cellPosition = new Vector2Int((i + 1) / cols + 1, cellPositionY);
            cell.cellPosition = cellPosition;
            cells.Add(cell);
        }
    }
    void FillRandomNumber(List<int> _numbers)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (i < startFilledCell)
            {
                cells[i].button.GetComponentInChildren<TMP_Text>().text = _numbers[i].ToString();
            }
            else
            {
                cells[i].button.GetComponentInChildren<TMP_Text>().text = "";
            }
        }
    }
    List<int> CreatRandomPairNumbers()
    {
        
        List<int> numbers = new List<int>();
        

        for (int i = 0; i < startFilledCell; i += 2)
        {
            int randomNumber;
            if (i / 2 < maxNumber)
            {
                randomNumber = i / 2 + 1;
            }
            else
            {
                randomNumber = Random.Range(1, maxNumber + 1);
            }

            int randomComdition = Random.Range(0, 2);

            numbers.Add(randomNumber);

            if (randomComdition == 0)
            {
                numbers.Add(randomNumber);
            }
            else
            {
                numbers.Add((maxNumber + 1) - randomNumber);
            }
        }

        // Shuffle(numbers);

        return numbers;
    }
    void Shuffle(List<int> _numbers)
    {
        for (int i = 0; i < _numbers.Count; i++)
        {
            int randomIndex = Random.Range(0, _numbers.Count + 1);
            (_numbers[i], _numbers[randomIndex]) = (_numbers[randomIndex], _numbers[i]);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;
using static UnityEditor.PlayerSettings;
using DG.Tweening;

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
    [SerializeField] GameObject inputHandlePanle;
    [SerializeField] GameOverPanle gameOverPanle;

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
    [SerializeField] Cell startCellInClearRow1;
    [SerializeField] Cell endCellInClearRow1;
    [SerializeField] Cell startCellInClearRow2;
    [SerializeField] Cell endCellInClearRow2;

    [Header("Round variable")]
    public int round = 1;

    [Header("Hight score variable")]
    [SerializeField] int hightScore = 0;

    [Header("Combo variable")]
    [SerializeField] float countDownTime = 5.0f; // Time will reset combo
    [SerializeField] float elapsed = 0;
    [SerializeField] float currentComboSroce = 0;
    [SerializeField] float comboScore = 0;
    [SerializeField] float nextTimeStartCountDownComboScore = float.MaxValue;
    [SerializeField] int countCombo = 0;

    [Header("Game state variable")]
    [SerializeField] bool handlescore = false;

    [Header("Highest variable")]
    [SerializeField] int highestRound = 0;
    [SerializeField] int highestScore = 0;
    [SerializeField] int highestCombo = 0;
    public void Init()
    {

        hopeNumber = 100;
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
        inputHandlePanle = uIManager.inputHandlePanle;
        playingPanle.comboSlider.maxValue = round * 100;
        gameOverPanle = uIManager.gameOverPanle;
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
                    StartCoroutine(HandlingWhenMatching(firstCell.cell, secondCell.cell));
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
    public bool Adjacent(Cell _cell1, Cell _cell2)
    {
        isSingleLine = true;
        addScore = 1 * round;

        return Math.Abs(_cell1.cellPosition.x - _cell2.cellPosition.x) <= 1 && Math.Abs(_cell1.cellPosition.y - _cell2.cellPosition.y) <= 1;
    }
    bool ClearPath(Cell _cell1, Cell _cell2)
    {
        isSingleLine = true;
        addScore = 4 * round;

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
    public Cell GetCellAt(Vector2Int _position)
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
        addScore = 4 * round;

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
    IEnumerator HandlingWhenMatching(Cell _cell1, Cell _cell2)
    {
        // Lock input
        inputHandlePanle.SetActive(true);
        handlescore = true;

        // Handle clear cell
        HandleCell(_cell1, _cell2);
        if (isSingleLine)
        {
            DrawnOneLine(_cell1, _cell2);
        }
        else
        {
            DrawnDoubleLine(_cell1, _cell2, gridAnimation.GetComponent<RectTransform>());
        }
        
        // Handle clear rows
        if (IsCleanedRows(_cell1, _cell2))
        {
            yield return new WaitForSeconds(0.6f);
            if (rowCanClear1 > 0 && rowCanClear2 > 0)
            {
                scoreManager.AddScoreHandle(scoreManager.clearRowScore * round, startCellInClearRow1.button, endCellInClearRow1.button);
                currentComboSroce = scoreManager.clearRowScore * round + currentComboSroce;
                scoreManager.AddScoreHandle(scoreManager.clearRowScore * round, startCellInClearRow2.button, endCellInClearRow2.button);
                currentComboSroce = scoreManager.clearRowScore * round + currentComboSroce;
                comboScore = currentComboSroce;
                nextTimeStartCountDownComboScore = countDownTime * 2 + Time.time;
                playingPanle.UpdateSlider(currentComboSroce);
                if (rowCanClear1 > rowCanClear2)
                {
                    audioManager.PlayCountDownClearRowOneShot();
                    boardManager.HandleClearRow(rowCanClear1);
                    boardManager.HandleClearRow(rowCanClear2);
                }
                else
                {
                    audioManager.PlayCountDownClearRowOneShot();
                    boardManager.HandleClearRow(rowCanClear2);
                    boardManager.HandleClearRow(rowCanClear1);
                }
            }
            else
            {
                scoreManager.AddScoreHandle(scoreManager.clearRowScore * round, startCellInClearRow1.button, endCellInClearRow1.button);
                currentComboSroce = scoreManager.clearRowScore * round + currentComboSroce;
                comboScore = currentComboSroce;
                nextTimeStartCountDownComboScore = countDownTime + Time.time;
                playingPanle.UpdateSlider(currentComboSroce);
                audioManager.PlayCountDownClearRowOneShot();
                if (rowCanClear1 > 0)
                {
                    boardManager.HandleClearRow(rowCanClear1);
                }
                if (rowCanClear2 > 0)
                {
                    boardManager.HandleClearRow(rowCanClear2);
                }
            }
        }

        // Handle clear number
        playingPanle.UpdateCountNumbers(_cell1.number, _cell2.number);
        if (playingPanle.countNumber[_cell1.number] == 0)
        {
            yield return new WaitForSeconds(1.2f);
            ClearNumberHandle((_cell1));
        }
        if (playingPanle.countNumber[_cell2.number] == 0 && _cell1.number != _cell2.number)
        {
            yield return new WaitForSeconds(0.2f);
            ClearNumberHandle((_cell2));
        }

        // unlock input
        inputHandlePanle.SetActive(false);
        handlescore = false;

        if (currentComboSroce > 1)
        {
            handlescore = true;
            CheckComboBonus();
        }

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
        scoreManager.AddScoreHandle(addScore, _cell1.button, _cell2.button);
        currentComboSroce = addScore + currentComboSroce;
        comboScore = currentComboSroce;
        nextTimeStartCountDownComboScore = countDownTime + Time.time;
        playingPanle.UpdateSlider(currentComboSroce);
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
        Vector2 cell1Position = new Vector2();
        Vector2 cell2Position = new Vector2();
        if (_cell1.cellPosition.x >  _cell2.cellPosition.x)
        {
            cell1Position = GetLocalCenter(_cell2.button.transform as RectTransform);
            cell2Position = GetLocalCenter(_cell1.button.transform as RectTransform);
        }
        else
        {
             cell1Position = GetLocalCenter(_cell1.button.transform as RectTransform);
             cell2Position = GetLocalCenter(_cell2.button.transform as RectTransform);
        }
        

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
    void ClearNumberHandle(Cell _cell)
    {
        Cell clearNumberCell = playingPanle.clearedNumbers[_cell.number - 1];
        clearNumberCell.button.GetComponent<CanvasGroup>().alpha = 0.4f;
        scoreManager.AddScoreHandle(scoreManager.clearnumberScore * round, clearNumberCell.button, clearNumberCell.button);
        currentComboSroce = scoreManager.clearnumberScore * round + currentComboSroce;
        comboScore = currentComboSroce;
        nextTimeStartCountDownComboScore = countDownTime + Time.time;
        playingPanle.UpdateSlider(currentComboSroce);
        audioManager.PlayClearNumberOneShot();
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
        
        if (HasPairNumberCanMatching(true))
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
    bool HasPairNumberCanMatching(bool isPlayerCheck)
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
                if (HasPathInList(cell, isPlayerCheck))
                {
                    return true;
                }
                else if (HasPathInBoard(cell, cells, maxSum, isPlayerCheck))
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool HasPathInBoard(Cell _cell, List<Cell> _cells, int _maxSum, bool isPlayerCheck)
    {
        Vector2Int[] directions = boardManager.directions;

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
                    if (isPlayerCheck)
                    {
                        _cell.button.image.color = ColorPalette.statYellow;
                        nextCell.button.image.color = ColorPalette.statYellow;
                    }
                    return true;
                }

                position += direction;
            }
        }

        return false ;
    }
    public bool IsOnGrid(Vector2Int _position)
    {
        return _position.x > 0 && _position.y > 0 && _position.x < cells.Count / GameManager.instance.boardManager.cols && _position.y <= GameManager.instance.boardManager.cols;
    }
    bool HasPathInList(Cell _cell, bool isPlayerCheck)
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
                if (isPlayerCheck)
                {
                    cells[i].button.image.color = ColorPalette.statYellow;
                    _cell.button.image.color = ColorPalette.statYellow;
                }
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
        boardManager.DuplicateCellsNotMatching();
        playingPanle.DuplicateCountNumber();
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

        if (row1 != row2)
        {
            bool isClearRow1 = IsClearRow(row1, 1);
            if (isClearRow1)
            {
                rowCanClear1 = row1;
            }
            bool isClearRow2 = IsClearRow(row2, 2);
            if (isClearRow2)
            {
                rowCanClear2 = row2;
            }
            return isClearRow1 || isClearRow2;
        }
        else if (row1 == row2)
        {
            bool isClearRow1 = IsClearRow(row1, 1);
            if (isClearRow1)
            {
                rowCanClear1 = row1;
            }
            return isClearRow1;
        }

        return false;
    }
    bool IsClearRow(int _row, byte _loop)
    {
        int countMatchingCell = 0;
        Vector2Int startPosition = new Vector2Int(_row, 1);
        int startIndex = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].cellPosition == startPosition)
            {

                if (_loop == 1)
                {
                    startIndex = i;
                    startCellInClearRow1 = cells[i];
                    endCellInClearRow1 = cells[i + boardManager.cols - 1];
                    break;
                }
                else
                {
                    startIndex = i;
                    startCellInClearRow2 = cells[i];
                    endCellInClearRow2 = cells[i + boardManager.cols - 1];
                    break;
                }
            }
        }

        for (int i = startIndex; i < startIndex + boardManager.cols; i++)
        {
            if (cells[i].isMatched || cells[i].number == 0)
            {
                countMatchingCell += 1;
            }
        }
        return countMatchingCell == boardManager.cols;
    }


    private void Update()
    {
        SetComboStatus();
        if (handlescore == false && boardManager.nextTimeCanCheckClearRound < Time.time)
        {
            CheckGameStatus();
        }
    }

    void SetComboStatus()
    {
        if (Time.time > nextTimeStartCountDownComboScore)
        {
            CountDownComboScore();
        }
        else if (Time.time < nextTimeStartCountDownComboScore)
        {
            elapsed = 0;
        }

        if (comboScore == 0 || currentComboSroce == 0)
        {
            foreach (var button in playingPanle.bonusButtonList)
            {
                button.interactable = false;
            }
        }
    }

    void CountDownComboScore()
    {
        elapsed += Time.deltaTime;
        currentComboSroce = Mathf.Lerp(comboScore, 0, elapsed / countDownTime);
        playingPanle.UpdateSlider(currentComboSroce);
    }
    void CheckComboBonus()
    {
        List<int> ints = new List<int>() { 20, 40, 60, 80, 100};
        int point = 0;
        bool isMaxCombo = false;

        for (int i = 0; i < ints.Count; i++)
        {
            if (currentComboSroce >  ints[i] * round)
            {
                point++;
            }
        }

        for (int i = 0; i < point; i++)
        {
            if (!playingPanle.bonusButtonList[i].interactable)
            {
                playingPanle.bonusButtonList[i].interactable = true;
                switch(i)
                {
                    case 0:
                        scoreManager.AddScoreHandle(scoreManager.bonus1Score * round, playingPanle.bonusButtonList[0], playingPanle.bonusButtonList[0]);
                        handlescore = false;
                        break;
                    case 1:
                        scoreManager.AddScoreHandle(scoreManager.bonus2Score * round, playingPanle.bonusButtonList[1], playingPanle.bonusButtonList[1]);
                        handlescore = false;
                        break;
                    case 2:
                        UpdateHope(scoreManager.bonus1Hope);
                        scoreManager.ShowScoreAtMidTowPoint(scoreManager.bonus1Hope, playingPanle.bonusButtonList[2], playingPanle.bonusButtonList[2]);
                        handlescore = false;
                        break;
                    case 3:
                        UpdateHope(scoreManager.bonus2Hope);
                        scoreManager.ShowScoreAtMidTowPoint(scoreManager.bonus2Hope, playingPanle.bonusButtonList[3], playingPanle.bonusButtonList[3]);
                        handlescore = false;
                        break;
                    case 4:
                        isMaxCombo = true;
                        countCombo++;
                        StartCoroutine(MaxComboBonus());
                        break;
                }
            }
        }
        if (!isMaxCombo)
        {
            handlescore = false;
        }
    }
    IEnumerator MaxComboBonus()
    {
        inputHandlePanle.SetActive(true);
        comboScore = 0;
        currentComboSroce = 0;

        scoreManager.ShowScoreAndCountUpScoreAtMidTowPoint(scoreManager.extraScore * round, playingPanle.bonusButtonList[3], playingPanle.bonusButtonList[4]);
        yield return new WaitForSeconds(2);
        scoreManager.currentScore += scoreManager.extraScore * round;
        playingPanle.SetScoreText(scoreManager.currentScore);

        for (int i = 0; i < scoreManager.extraHope; i++)
        {
            yield return new WaitForSeconds(0.5f);
            scoreManager.ShowScoreAtMidTowPoint(1, playingPanle.hopeButton, playingPanle.hopeButton);
            UpdateHope(1);
            audioManager.PlayClearNumberOneShot();
        }

        inputHandlePanle.SetActive(false);
        handlescore = false;
    }
    void CheckGameStatus()
    {
        boardManager.nextTimeCanCheckClearRound = boardManager.pendingTime + Time.time;
        handlescore = true;

        int countClearNumber = 0;
        for (int i = 0; i < playingPanle.countNumber.Count; i++)
        {
            if (playingPanle.countNumber[i + 1] == 0)
            {
                countClearNumber++;
            }
            if (countClearNumber == maxSum - 1)
            {
                ClearGroundHandle();
            }
        }

        if (addNunbersNumber == 0 && !HasPairNumberCanMatching(false))
        {
            StartCoroutine(OverGameHandle());
        }
    }
    void ClearGroundHandle()
    {
        inputHandlePanle.SetActive(true);

        ResetRound();

        ResetComboScore();

        boardManager.StartCoroutineFillNumberWithDifficuiltValue(round);
        boardManager.AwakeClearNumbersBoard();

        ResetAddNumbersNumber();

        inputHandlePanle.SetActive(false);
    }
    void ResetComboScore()
    {
        playingPanle.comboSlider.maxValue = round * 100;
        comboScore = 0;
        currentComboSroce = 0;
        playingPanle.comboSlider.value = 0;
    }
    void ResetRound()
    {
        round += 1;
        playingPanle.roundText.text = round.ToString();
    }
    void ResetAddNumbersNumber()
    {
        addNunbersNumber += 5 - addNunbersNumber;
        playingPanle.SetAddNumberText(addNunbersNumber,false);
    }
    IEnumerator OverGameHandle()
    {
        inputHandlePanle.SetActive(true);

        SetTranfarentPlayingPanle(0);


        gameOverPanle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        gameOverPanle.AnimationScore(gameOverPanle.roundText, round);
        yield return new WaitForSeconds(0.25f);

        gameOverPanle.AnimationScore(gameOverPanle.scoreText, scoreManager.currentScore);
        yield return new WaitForSeconds(0.25f);

        gameOverPanle.AnimationScore(gameOverPanle.comboText, countCombo);
        yield return new WaitForSeconds(0.25f);

        if(round > highestRound)
        {
            highestRound = round;
            gameOverPanle.roundHighestText.text = highestRound.ToString();
            gameOverPanle.roundMedal.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.25f);

        if (scoreManager.currentScore > highestScore)
        {
            highestScore = scoreManager.currentScore;
            gameOverPanle.scoreHighestText.text = highestScore.ToString();
            gameOverPanle.scoreMedal.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.25f);

        if (countCombo > highestCombo)
        {
            highestCombo = countCombo;
            gameOverPanle.comboHighestText.text = highestCombo.ToString();
            gameOverPanle.comboMedal.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.25f);

        gameOverPanle.backButton.gameObject.SetActive(true);
        gameOverPanle.playAgainButton.gameObject.SetActive(true);
    }
    void SetTranfarentPlayingPanle(float _value)
    {
        DG.Tweening.Sequence sequene = DOTween.Sequence();

        sequene.Append(playingPanle.canvasGroup.DOFade(_value, 1.0f));
    }
    public void OutGame()
    {
        Application.Quit();
    }
    public void RePlay()
    {
        inputHandlePanle.SetActive(false);
        gameOverPanle.comboMedal.gameObject.SetActive(false);
        gameOverPanle.scoreMedal.gameObject.SetActive(false);
        gameOverPanle.roundMedal.gameObject.SetActive(false);

        gameOverPanle.gameObject.SetActive(false);

        SetTranfarentPlayingPanle(1);

        round = 1;
        playingPanle.roundText.text = round.ToString();
        hopeNumber = 40;
        addNunbersNumber = 5;
        scoreManager.currentScore = 0;
        playingPanle.SetHopeText(hopeNumber, false);
        playingPanle.SetAddNumberText(addNunbersNumber, false);
        playingPanle.SetScoreText(scoreManager.currentScore);
        playingPanle.SetHightScoreText(highestScore);

        boardManager.ResetNewRound();
    }
}

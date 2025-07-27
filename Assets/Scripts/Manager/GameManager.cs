using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region instance
    public static GameManager instance;
    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnDisable()
    {
        instance = null;
    }
    #endregion

    public UIManager uIManager;
    public BoardManager boardManager;
    public GamePlayManager gamePlayManager;


    void Start()
    {
        Init();
    }
    void Init()
    {
        boardManager = GetComponentInChildren<BoardManager>();
        uIManager = gameObject.GetComponentInChildren<UIManager>();
        gamePlayManager = GetComponentInChildren<GamePlayManager>();

        uIManager.Init();

        boardManager.Init();

        gamePlayManager.Init();
    }

}

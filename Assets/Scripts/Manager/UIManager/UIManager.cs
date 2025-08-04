using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region instance
    public static UIManager instance;
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

    public PlayingPanle playingPanle;
    public GridAnimation gridAnimation;
    public MyUILineRenderer uILineRenderer1;
    public MyUILineRenderer uILineRenderer2;
    public GameObject inputHandlePanle;
    public GameOverPanle gameOverPanle;

    public void Init()
    {
        playingPanle = GetComponentInChildren<PlayingPanle>();
        playingPanle.Init();

        gridAnimation = GetComponentInChildren<GridAnimation>();
        gridAnimation.Init();

        
        uILineRenderer1.Init();
        uILineRenderer2.Init();

        inputHandlePanle.SetActive(false);

        gameOverPanle = GetComponentInChildren<GameOverPanle>();
        gameOverPanle.gameObject.SetActive(false);
    }

}

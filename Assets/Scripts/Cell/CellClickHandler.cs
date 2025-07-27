using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellClickHandler : MonoBehaviour
{
    
    [SerializeField] GamePlayManager gamePlayManager;
    public Cell cell;

    private void Start()
    {
        gamePlayManager = GameManager.instance.gamePlayManager;
    }

    public void OnClick()
    {
        gamePlayManager.OnCellClick(this);
    }
}

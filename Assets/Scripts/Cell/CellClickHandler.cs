using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

[System.Serializable]
public class CellClickHandler : MonoBehaviour
{
    
    [SerializeField] GamePlayManager gamePlayManager;
    public Cell cell;

    private void Start()
    {
        gamePlayManager = GameManager.instance.gamePlayManager;
        cell.matchingEffect = GetComponentInChildren<ParticleSystem>(true);
    }

    public void OnClick()
    {
        gamePlayManager.OnCellClick(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource cellAudioSource;
    [SerializeField] AudioSource gameAudioSource;

    [SerializeField] AudioClip evenNumberTouch;
    [SerializeField] AudioClip oddNumberTouch;
    [SerializeField] AudioClip notMatching;

    [SerializeField] AudioClip matchingCell1;
    [SerializeField] AudioClip matchingCell2;
    [SerializeField] AudioClip matchingCellInList;

    [SerializeField] AudioClip hope;

    public void Init()
    {
        
    }


    public void PlayNumberTouch(int _value)
    {
        if (_value % 2 == 0)
        {
            PlayEvenNumberTouchOneShot();
        }
        else
        {
            PlayOddNumberTouchOneShot();
        }
    }
    void PlayEvenNumberTouchOneShot()
    {
        cellAudioSource.PlayOneShot(evenNumberTouch);
    }
    void PlayOddNumberTouchOneShot()
    {
        cellAudioSource.PlayOneShot(oddNumberTouch);
    }

    public void PlayNotMatchingCellOneShot()
    {
        gameAudioSource.PlayOneShot(notMatching);
    }

    public void PlayerMatchingCellInListOneShot()
    {
        gameAudioSource.PlayOneShot(matchingCellInList);
    }

    public void PlayerMatchingCellOneShot()
    {
        int randomClip = Random.Range(0, 2);
        if (randomClip == 0)
        {
            PlayMatchingCell1OneShot();
        }
        else
        {
            PlayMatchingCell2OneShot();
        }
    }
    public void PlayMatchingCell1OneShot()
    {
        gameAudioSource.PlayOneShot(matchingCell1);
    }
    public void PlayMatchingCell2OneShot()
    {
        gameAudioSource.PlayOneShot(matchingCell2);
    }
    public void PlayHopeOneShot()
    {
        gameAudioSource.PlayOneShot(hope);
    }


}

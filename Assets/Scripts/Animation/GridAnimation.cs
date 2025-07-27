using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimation : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] string ringString = "Ring";

    public void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void Ring()
    {
        animator.Play(ringString);
    }
}

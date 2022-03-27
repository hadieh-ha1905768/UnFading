using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Emotions : MonoBehaviour
{
    public Animator animator;

    void Start () {
        animator = GetComponent<Animator>();
        animator.SetInteger("emotionsInt", 0);
    }
    public void emotionsChange() {
        animator.SetInteger("emotionsInt", Random.Range(0, 6));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallHandler : MonoBehaviour
{
    Animator animator;

    bool goingUp = true;
    bool goingDown;

    void Start()
    {
        animator = GetComponent<Animator>();

        goingUp = true;
    }
    
    void Update()
    {
        UpdateAnimations();
    }

    public void Gone()
    {
        Destroy(gameObject);
    }

    public void GoDown()
    {
        goingDown = true;
    }

    void UpdateAnimations()
    {
        animator.SetBool("goingUp", goingUp);
        animator.SetBool("goingDown", goingDown);
    }
}

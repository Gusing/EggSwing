using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallHandler : MonoBehaviour
{
    Animator animator;

    bool goingUp = true;
    bool goingDown;

    protected FMOD.Studio.EventInstance soundGoUp;
    protected FMOD.Studio.EventInstance soundGoDown;

    void Start()
    {
        soundGoUp = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Wall_up");
        soundGoDown = FMODUnity.RuntimeManager.CreateInstance("event:/Object/Wall_down");

        animator = GetComponent<Animator>();

        goingUp = true;

        soundGoUp.start();
    }
    
    void Update()
    {
        UpdateAnimations();
    }

    public void CompletelyUp()
    {
        soundGoUp.setParameterValue("End", 1);
    }

    public void Gone()
    {
        soundGoDown.setParameterValue("End", 1);
        Destroy(gameObject);
    }

    public void GoDown()
    {
        soundGoDown.start();
        goingDown = true;
    }

    void UpdateAnimations()
    {
        animator.SetBool("goingUp", goingUp);
        animator.SetBool("goingDown", goingDown);
    }
}

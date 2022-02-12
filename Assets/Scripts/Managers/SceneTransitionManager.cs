using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public Animator Animator;

    public float TransitionStartTime = 0.5f;
    public float TransitionEndTime = 0.5f;

    private SceneTransitionState State = SceneTransitionState.Initial;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator RunTransitionStart()
    {
        if (State == SceneTransitionState.Start)
        {
            yield break;
        }

        State = SceneTransitionState.Start;
        Animator.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionStartTime);
    }

    public IEnumerator RunTransitionEnd()
    {
        if (State == SceneTransitionState.End)
        {
            yield break;
        }

        State = SceneTransitionState.End;
        Animator.SetTrigger("End");
        yield return new WaitForSeconds(TransitionEndTime);
    }
}

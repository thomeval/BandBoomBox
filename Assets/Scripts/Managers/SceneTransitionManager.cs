using System.Collections;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public Animator Animator;

    public float TransitionStartTime = 0.5f;
    public float TransitionEndTime = 0.5f;
    public bool TransitionInProgress = false;
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
        TransitionInProgress = true;
        State = SceneTransitionState.Start;
        Animator.SetTrigger("Start");
        yield return new WaitForSeconds(TransitionStartTime);
    }

    public IEnumerator RunTransitionEnd()
    {
        // Bug: Attempting to set TransitionInProgress after the yield return statement will not work.
        TransitionInProgress = false;
        if (State == SceneTransitionState.End)
        {
            yield break;
        }

        State = SceneTransitionState.End;
        Animator.SetTrigger("End");
        yield return new WaitForSeconds(TransitionEndTime);

    }
}

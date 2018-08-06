using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameFlow_LoadScene : StateMachineBehaviour
{

    private AsyncOperation sceneAO;
    public string Scene;
    private GameFlow GameFlow;
    private SliderScript Slider;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameFlow = animator.GetComponent<GameFlow>();
        Scene = GameFlow.CurrentLoadingScene != null ? GameFlow.CurrentLoadingScene : Scene;
        sceneAO = SceneManager.LoadSceneAsync(Scene);
        Slider = GameFlow.LoadingMask.GetComponent<SliderScript>();
        Slider.Rest();
        GameFlow.LoadingMask.SetActive(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Slider.Value = sceneAO.progress / 0.9f;
        Debug.Log(Slider.Value);
        if (!Slider.IsDone) return;
        animator.Play(Scene);
        GameFlow.LoadingMask.SetActive(false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    //Function that can receive animation events
    //Functions to play fade in/out animations

    //Track the Animation Component
    //Track the AnimationClips for fade in/out
    //decorator
    [SerializeField] Animation _mainMenuAnimator;
    [SerializeField] AnimationClip _fadeOutAnimation;
    [SerializeField] AnimationClip _fadeInAnimation;

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start() {
        //register for this event,
        //AddListener accepts a function that matches the signature of the event
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    public void OnFadeOutComplete() {
        //Debug.Log("FadeOut Complete");
        OnMainMenuFadeComplete.Invoke(true);
    }

    public void OnFadeInComplete() {
        //Debug.Log("FadeIn Complete");
        OnMainMenuFadeComplete.Invoke(false);
        UIManager.Instance.SetDummyCameraActive(true);
    }

    void HandleGameStateChanged(GameManager.GameState curState, GameManager.GameState prevState) {

        if (prevState == GameManager.GameState.PREGAME && curState 
            == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }
            
        if (prevState != GameManager.GameState.PREGAME && curState
            == GameManager.GameState.PREGAME)
        {
            FadeIn();
        }
    }

    //Trigger animation
    public void FadeIn() {
        //make sure it's not playing
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeInAnimation;
        _mainMenuAnimator.Play();
    }

    public void FadeOut() {
        UIManager.Instance.SetDummyCameraActive(false);

        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeOutAnimation;
        _mainMenuAnimator.Play();
    }
}

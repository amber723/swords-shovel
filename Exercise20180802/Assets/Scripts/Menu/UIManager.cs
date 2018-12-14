using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Camera _dummyCamera;
    [SerializeField] private GameObject _unitFrame;

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        _mainMenu.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    /*This EventHandler help us bubble up the event data from the MainMenu without
     * having every system needing to know ab the MainMenu.
     */
    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        OnMainMenuFadeComplete.Invoke(fadeOut);
    }

    void HandleGameStateChanged(GameManager.GameState curState,
        GameManager.GameState prevState)
    {
        _pauseMenu.gameObject.SetActive(curState == GameManager.GameState.PAUSED);

        bool showUnitFrame = curState == GameManager.GameState.RUNNING
            || curState == GameManager.GameState.PAUSED;
        _unitFrame.SetActive(showUnitFrame);
    }

    void Update()
    {
        if (GameManager.Instance.CurGameState != GameManager.GameState.PREGAME)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //_mainMenu.FadeOut();
            GameManager.Instance.StartGame();
        }
    }

    public void SetDummyCameraActive(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }
}

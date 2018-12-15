using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Camera _dummyCamera;
    [SerializeField] private GameObject _unitFrame;

    [SerializeField] private Image healthBar;
    [SerializeField] private Text levelText;

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

    public void InitUnitFrame()
    {
        levelText.text = "1";
        healthBar.fillAmount = 1;
    }

    public void UpdateUnitFrame(HeroController hero)
    {
        int curHp = hero.GetCurHp();
        int maxHp = hero.GetMaxHp();

        healthBar.fillAmount = (float) curHp / maxHp;
        levelText.text = hero.GetCurLevel().ToString();
    }
}

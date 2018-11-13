using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button ResumeBtn;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private Button QuitBtn;

    private void Start()
    {
        ResumeBtn.onClick.AddListener(HandleResumeClicked);
        RestartBtn.onClick.AddListener(HandleRestartClicked);
        QuitBtn.onClick.AddListener(HandleQuitClicked);
    }

    void HandleResumeClicked()
    {
        GameManager.Instance.TogglePause();
    }

    void HandleRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }

    void HandleQuitClicked()
    {
        GameManager.Instance.QuitGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    /*what level the game is currently in
      load and unload game levels
      keep track of the game state
      generate other persistent system
    */

    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED
    }

    public GameState CurGameState { get; private set; }
    public Events.EventGameState OnGameStateChanged;

    //a list of system prefabs that GameManager need to keep track of 
    public GameObject[] SystemPrefabs;

    //a list of system prefabs after they've been created.
    //to be able to clean up
    List<GameObject> _instancedSystemPrefabs;

    string _curLevelName = string.Empty;

    //track the number of asyncOperations
    List<AsyncOperation> _loadOperations;

    /*
     * The GameManager will exist and start before the hero exist, so we 
     * initialize the variable in the accessor instead of in the Start() method.
     * This method is called Lazy Initialization.
     */
    HeroController heroCtrl;
    HeroController hero {
        get {
            if (heroCtrl == null)
            {
                heroCtrl = FindObjectOfType<HeroController>();
            }
            return heroCtrl; 
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        CurGameState = GameState.PREGAME;
        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        //LoadLevel("Main");
        UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
    }

    void Update()
    {
        if (CurGameState == GameState.PREGAME)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /*wrapping SceneManager calls inside of our own methods for loading and unloading levels.
      And let the rest of the game know about when scene loads are happening and when 
      scene loads are complete.
    */
    public void LoadLevel(string levelName)
    {
        /** we can access a scene by build index,or by the name, or by the path

          SceneManager.LoadScene is a blocking call. This means until 
          I'm done doing my operations, everyone else needs to wait.

          SceneManager.LoadSceneAsync is an asynchronous operation
        **/

        /*LoadSceneMode is an enum and specify whether or not to load a scene
         * additively or as a single scene.*/
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        /*If a request is made to load a scene that it can't load,
            the SceneManager will return a null value instead.*/
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }

        /** C# actions can += to a list of event listeners
         * there can be other functions who want to listen to this event
         * 
         * use "=" would replace any existing listeners with yourself only and
         * wipe all those out.
         **/
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _curLevelName = levelName;
    }

    //The listening function needs to receive an argument of at least an AnsyncOperation.
    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if (_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);

                // Set current scene as the active Scene
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_curLevelName));

                // See now that the name is updated
                Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);
            }
        }

        Debug.Log("Load Complete.");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        // Set current scene as the active Scene
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));

        // See now that the name is updated
        Debug.Log("Active Scene : " + SceneManager.GetActiveScene().name);

        Debug.Log("Unload Complete.");
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {

        //if (!fadeOut)//if MainMenu has fadeIn
        //UnloadLevel(_curLevelName);
    }

    void UpdateState(GameState state)
    {
        GameState prevGameState = CurGameState;
        CurGameState = state;

        switch (CurGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                /**One way to pause the simulation:
                 * Leverage(利用) the Unity Built-in Time object.
                 * TimeScale is a multiplier that changes what the frame rate is of a game at any given time.
                 * timeScale = 0, no updates will happen at this time.
                 **/
                Time.timeScale = 0.0f;
                break;
            default:
                break;
        }

        //dispath message
        //transition between scenes

        //Sometimes the Invoke method is used to distinguish between events and methods
        OnGameStateChanged.Invoke(CurGameState, prevGameState);

    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for (int i = 0; i < SystemPrefabs.Length; ++i)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_instancedSystemPrefabs == null)
            return;

        for (int i = 0; i < _instancedSystemPrefabs.Count; ++i)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }
        _instancedSystemPrefabs.Clear();
    }

    public void StartGame()
    {
        LoadLevel("Main");
    }

    public void TogglePause()
    {
        //The ternary operator 三元运算符: condition ? true : false
        UpdateState(CurGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    public void RestartGame()
    {
        //transition from the running state to a pregame state
        UpdateState(GameState.PREGAME);
    }

    public void QuitGame()
    {
        //implement features for quitting, eg. auto saving
        Debug.LogError("Quitting...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    #region CallBacks
    public void OnHeroLeveledUp(int newLv)
    {
        UIManager.Instance.UpdateUnitFrame(hero);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.LevelUp);
    }

    public void OnHeroDamaged(int damage)
    {
        //Debug.LogWarningFormat("Hero damage for {0}", damage);
        UIManager.Instance.UpdateUnitFrame(hero);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.HeroHit);
    }

    public void OnHeroGainedHealth(int health)
    {
        UIManager.Instance.UpdateUnitFrame(hero);
        Debug.LogWarningFormat("Hero gained {0} health", health);
    }

    public void OnHeroDied()
    {
        UIManager.Instance.UpdateUnitFrame(hero);
        //UIManager.Instance.PlayGameOver();
        //SaveSession(EndGameState.Loss);
        //StartCoroutine(EndGame());
    }

    public void OnHeroInit()
    {
        UIManager.Instance.InitUnitFrame();
    }

    #endregion
}
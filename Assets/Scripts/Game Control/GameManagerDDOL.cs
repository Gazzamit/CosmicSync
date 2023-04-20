using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerDDOL : MonoBehaviour
{
    //Not Singleton - just one instance check
    private static bool _instance = false;

    public bool DEV_THIS_LEVEL = false;

    //DDOL calibration timing value
    public static float _timingSliderValue = 0f;

    //DDOL PitchYaw slider value
    public static float _pitchYawSliderValue = 1f;

    //DDOL GameMode private statics (set to Main Menu at the start)
    private static GameMode _currentGameMode = GameMode.MainMenu;
    private static GameMode _previousGameMode = GameMode.MainMenu;

    //Track mode in editor
    public GameMode _gameModePublic, _previousGameModePublic;

    //Track wheter to show welcome text
    public static bool _doWelcome; //run welcome dialogues
    public static bool _doWelcomeRunOnce = false;

    public bool _doWelcomePublicStatic, _doWelcomeRunOncePubSt;
    public static bool _doWelcomeFlickerFX = true; //runonce menu fx in HUDanimations

    //Track current level
    public static int _currentLevel;

    //enum for game mode
    public enum GameMode
    {
        Game,
        MainMenu,
        SettingsMenu
    }

    public static GameMode _currentMode
    {
        get { return _currentGameMode; }
        set
        {
            _previousGameMode = _currentGameMode;
            _currentGameMode = value;
        }
    }

    public static GameMode _previousMode
    {
        get { return _previousGameMode; }
    }

    private void Awake()
    {
        if (!_instance)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (DEV_THIS_LEVEL == true)
        {
            Debug.Log("THIS LEVEL IS IN DEV MODE");
            //see start()
        }
        else if (_doWelcomeRunOnce == false)
        {
            Debug.Log("RUNNING WELCOME");
            _doWelcomeRunOnce = true;
            _doWelcome = true;
        }
    }

    void Start()
    {
        if (DEV_THIS_LEVEL == true)
        {
            StartCoroutine(SETDEVMODE());
        }
    }
    void Update()
    {
        StartCoroutine(SetGameModes());
    }


    IEnumerator SetGameModes()
    {
        _gameModePublic = _currentGameMode;
        _previousGameModePublic = _previousGameMode;
        _doWelcomePublicStatic = _doWelcome;
        _doWelcomeRunOncePubSt = _doWelcomeRunOnce;
        yield return null;
    }

    IEnumerator SETDEVMODE()
    {
        yield return new WaitForSeconds(1);
        //activate main menu (set it to game first so it will trigger change)
        HUDAnimations._switchingHUD = true;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
    }
}

using System.Collections;
using UnityEngine;

public class GameManagerDDOL : MonoBehaviour
{
    //Not Singleton - just one instance check
    private static bool _instance = false;

    //DDOL calibration timing value
    public static float _timingSliderValue = 0f;

    //DDOL PitchYaw slider value
    public static float _pitchYawSliderValue = 1f;

    public static bool _normalStart = true;

    //DDOL GameMode private statics (set to Main Menu at the start)
    private static GameMode _currentGameMode = GameMode.MainMenu;
    private static GameMode _previousGameMode = GameMode.MainMenu;

    //Track mode in editor
    public GameMode _gameModePublic, _previousGameModePublic;

    //Track wheter to show welcome text
    public static bool _doWelcome = false; //run welcome dialogues
    public static bool _doWelcomeFlickerFX = true; //runonce menu fx in HUDanimations

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

        //_gameModePublic = _currentMode; 

        //GAZ REMOVE
        _doWelcome = false;
    }

    void Update()
    {
        StartCoroutine(SetGameModes());
    }

    IEnumerator SetGameModes()
    {
        _gameModePublic = _currentGameMode;
        _previousGameModePublic = _previousGameMode;
        yield return null;
    }
}

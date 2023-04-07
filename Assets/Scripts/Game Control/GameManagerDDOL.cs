using UnityEngine;

public class GameManagerDDOL : MonoBehaviour
{
    private static bool _instance = false;

    //DDOL calibration timing value
    public static float _timingSliderValue = 0f;

    //DDOL PitchYaw slider value
    public static float _pitchYawSliderValue = 1f;

    public static bool _init = true;

    //DDOL GameMode private statics (set to Main Menu at the start)
    private static GameMode _currentGameMode = GameMode.MainMenu;
    private static GameMode _previousGameMode = GameMode.MainMenu;

    //Track mode in editor
    public GameMode _gameModePublic, _previousGameModePublic;

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
    }

    void Update()
    {
        _gameModePublic = _currentGameMode;
        _previousGameModePublic = _previousGameMode;
    }
}

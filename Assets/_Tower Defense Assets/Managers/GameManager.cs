using MonsterLove.StateMachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance = null;

    public GameObject environmentTransform;
    public GameObject entitiesTransform;

    [Header("Prefabs")]
    public new GameObject camera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private SelectionManager selectionManager;

    public static SelectionManager SelectionManager
    {
        get
        {
            return instance.selectionManager.GetComponent<SelectionManager>();
        }
    }
    public static LevelManager LevelManager
    {
        get
        {
            return instance.levelManager;
        }
    }
    public static TowerManager TowerManager
    {
        get
        {
            return instance.towerManager;
        }
    }
    public static EnemyManager EnemyManager
    {
        get
        {
            return instance.enemyManager;
        }
    }
    public static SoundManager SoundManager
    {
        get
        {
            return instance.soundManager;
        }
    }
    public static PlayerController Player
    {
        get
        {
            return instance.playerController;
        }
    }

    public static StateMachine<States> Fsm
    {
        get
        {
            return instance.fsm;
        }
    }
    public static GameObject EntitiesList
    {
        get
        {
            return instance.entitiesTransform;
        }
    }
    public static GameObject EnvironmentList
    {
        get
        {
            return instance.entitiesTransform;
        }
    }

    [HideInInspector]
    public StateMachine<States> fsm;

    [HideInInspector]
    public enum States
    {
        MainEntry,
        MainMenu,
        LevelSelect,
        Edit,
        Play,
        Pause,
        LevelWin,
        LevelLose,
    }
    // States
    [HideInInspector]
    public MainMenuState mainMenuState;
    [HideInInspector]
    public EditState editState;
    [HideInInspector]
    public PlayState playState;
    [HideInInspector]
    public PauseState pauseState;
    [HideInInspector]
    public LevelWinState levelWinState;
    [HideInInspector]
    public LevelLoseState levelLoseState;
    [HideInInspector]
    public LevelSelectState levelSelectState;


    public void Awake()
    {
        // Making sure there is exactly one instance of GameManager
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // Making sure the GameManager persists across scene loads
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);

        // Getting useful components
        editState = GetComponent<EditState>();
        playState = GetComponent<PlayState>();
        pauseState = GetComponent<PauseState>();
        mainMenuState = GetComponent<MainMenuState>();
        levelWinState = GetComponent<LevelWinState>();
        levelLoseState = GetComponent<LevelLoseState>();
        levelSelectState = GetComponent<LevelSelectState>();

        // Creating finite state machine
        fsm = StateMachine<States>.Initialize(instance);
#if (UNITY_ANDROID || UNITY_IOS)
        LevelManager.CopyLevelsToDisk(1, 1);
#endif
    }

    public void Start()
    {
        // Setting first game state
        fsm.ChangeState(States.MainEntry);
    }

    // --------------- FSM --------------- //
    //********** MAINENTRY GAME STATE **********//
    public void MainEntry_Enter()
    {
        Player.Load("Player.dat");
        /*for (int i = 0; i < GUIManager.instance.transform.childCount; i++)
        {
            GUIManager.instance.transform.GetChild(i).gameObject.SetActive(true);
            GUIManager.instance.transform.GetChild(i).gameObject.SetActive(false);
        }*/
        Fsm.ChangeState(States.MainMenu);
    }
    //********** MAINMENU GAME STATE **********//
    public void MainMenu_Enter()
    {
        mainMenuState.enabled = true;
        mainMenuState.Enter();
    }
    public void MainMenu_Exit()
    {
        mainMenuState.Exit();
        mainMenuState.enabled = false;
    }
    //********** MAINENTRY GAME STATE **********//
    public void LevelSelect_Enter()
    {
        levelSelectState.enabled = true;
        levelSelectState.Enter();
    }
    public void LevelSelect_Exit()
    {
        levelSelectState.Exit();
        levelSelectState.enabled = false;
    }
    //********** EDIT GAME STATE **********//
    public void Edit_Enter()
    {
        editState.enabled = true;
        editState.Enter();
    }
    public void Edit_Exit()
    {
        editState.Exit();
        editState.enabled = false;
    }

    //********** PLAY GAME STATE **********//
    public void Play_Enter()
    {
        playState.enabled = true;
        playState.Enter();
    }
    public void Play_Exit()
    {
        playState.Exit();
        playState.enabled = false;
    }

    //********** PAUSE GAME STATE **********//
    public void Pause_Enter()
    {
        pauseState.enabled = true;
        pauseState.Enter();
    }
    public void Pause_Exit()
    {
        pauseState.Exit();
        pauseState.enabled = false;
    }

    //********** LEVELWIN GAME STATE **********//
    public void LevelWin_Enter()
    {
        levelWinState.enabled = true;
        levelWinState.Enter();
    }
    public void LevelWin_Exit()
    {
        levelWinState.Exit();
        levelWinState.enabled = false;
    }
    //********** LEVELLOSE GAME STATE **********//
    public void LevelLose_Enter()
    {
        levelLoseState.enabled = true;
        levelLoseState.Enter();
    }
    public void LevelLose_Exit()
    {
        levelLoseState.Exit();
        levelLoseState.enabled = false;
    }
}

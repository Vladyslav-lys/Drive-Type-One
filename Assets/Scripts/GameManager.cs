using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    public bool isStarted;
    public bool isFinished;
    public bool isLosing;
    public bool debugTutorial;
    public bool debugLevel;
    public int level;
    public int diamonds;
    public List<GameObject> levels;
    private Level _currentLevel;
    private SpawnManager _sm;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private KeyboardManager _km;
    private PlayerController _playerController;
    private TargetCamera _targetCam;

    public int Diamonds
    {
        get => PlayerPrefs.GetInt("Diamonds", 0);
        set => PlayerPrefs.SetInt("Diamonds", value);
    }

    public int CurrentLevelNum
    {
        get => PlayerPrefs.GetInt("Level", 1);
        set => PlayerPrefs.SetInt("Level", value);
    }

    public GameObject CurrentLevelObj => levels[CurrentLevelNum - 1];

    public Level CurrentLevel => _currentLevel;

    public int FifthLevelCount
    {
        get => PlayerPrefs.GetInt("FifthLevel", 0);
        set => PlayerPrefs.SetInt("FifthLevel", value);
    }

    public int Tutorial
    {
        get => PlayerPrefs.GetInt("Tutorial", 1);
        set => PlayerPrefs.SetInt("Tutorial", value);
    }

    public int ComingSoon
    {
        get => PlayerPrefs.GetInt("ComingSoon", 0);
        set => PlayerPrefs.SetInt("ComingSoon", value);
    }

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        if (debugLevel)
            CurrentLevelNum = level;
        _currentLevel = CurrentLevelObj.GetComponent<Level>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        _playerController = PlayerController.instance;
        _playerController.moveX = _currentLevel.startPlayerX;
        _playerController.transform.position = new Vector3(_currentLevel.startPlayerX,
            _playerController.transform.position.y, _playerController.transform.position.z);
        _sm = SpawnManager.instance;
        _uiManager = UIManager.instance;
        _km = KeyboardManager.instance;
        _soundManager = SoundManager.instance;
        _targetCam = Camera.main.GetComponent<TargetCamera>();
        CurrentLevelObj.SetActive(true);
        _km.words = _currentLevel.words;
        _km.WinGame += WinGame;
        _km.AddDiamonds += AddDiamonds;
        _uiManager.diamondMainText.text = Diamonds.ToString();
        _uiManager.currentLevelText.text = CurrentLevelNum.ToString();
        int nextLevelNum = CurrentLevelNum + 1;
        if (nextLevelNum > levels.Count)
            _uiManager.nextLevelText.text = 1.ToString();
        else
            _uiManager.nextLevelText.text = nextLevelNum.ToString();
        
        _sm.envNum = FifthLevelCount;
        _sm.spawnPosEnv = _sm.environmentObjs[_sm.envNum].transform.position;
        _sm.StartSpawn();
        _soundManager.CreateSound(_soundManager.preparingSound, _playerController.transform);
        OpenComingSoon();
    }

    public void StartGame()
    {
        isStarted = true;
        _uiManager.StartGame();
        _sm.SpawnRepeat();
        _soundManager.DestroySound(_soundManager.preparingSound,0f);
        _soundManager.CreateSoundAndDestroy(_soundManager.startSound, 1f, _playerController.transform);
        _soundManager.CreateSound(_soundManager.drivingSound, _playerController.transform);
        _playerController.SetStartAnim(true);
    }
    
    public void TutorialGameInTime(float delay) => Invoke(nameof(TutorialGame), delay);

    public void TutorialGame()
    {
        if(Tutorial == 0 && !debugTutorial)
            return;

        Time.timeScale = 0;
        _soundManager.StopAll();
        _uiManager.TutorialGame(true);
        _uiManager.ShowTapTutorialTextInTime(3f);
        _km.CloseKeyboard();
    }
    
    public void UntutorialGame()
    {
        Time.timeScale = 1;
        _soundManager.ContinueAll();
        _uiManager.TutorialGame(false);
        _km.OpenKeyboard();
        Tutorial = 0;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        _uiManager.PauseGame(true);
        _soundManager.StopAll();
        if(_km.canClick)
            _km.CloseKeyboard();
    }
    
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        _uiManager.PauseGame(false);
        _soundManager.ContinueAll();
        if(_km.canClick)
            _km.OpenKeyboard();
    }

    public void LoseGame()
    {
        Time.timeScale = 1f;
        isLosing = true;
        _soundManager.DestroySound(_soundManager.drivingSound, 0f);
        _sm.InterruptSpawn();
        _currentLevel.StopAllLevelAnotherCars();
        _uiManager.LoseGame();
    }

    public void WinGame()
    {
        Invoke(nameof(WinGameActs), 0.5f);
    }

    private void WinGameActs()
    {
        if(isLosing)
            return;

        _targetCam.cameraTransformState = CameraTransformState.Finish;
        isFinished = true;
        Diamonds += diamonds;
        _uiManager.WinGame();
        _sm.InterruptSpawn();
        _soundManager.DecreaseVolumeAndOffLoopSound(_soundManager.drivingSound);
        //_soundManager.RestartSound(_soundManager.drivingSound);
        StartCoroutine(CreateWinSound());
        StartCoroutine(DiamondsCount());
    }

    public void ReduceAllLevelOtherCarsSpeed() => _currentLevel.ReduceAllCarsSpeed();
    
    public void IncreaseAllLevelOtherCarsSpeed() => _currentLevel.IncreaseAllCarsSpeed();

    public bool CanPlay() => !isFinished && !isLosing && isStarted;

    public void NextLevel()
    {
        if (CurrentLevelNum % 5 == 0)
            FifthLevelCount++;
        if (FifthLevelCount > _sm.environmentObjs.Length - 1)
            FifthLevelCount = 0;
        CurrentLevelNum++;

        if (CurrentLevelNum > levels.Count)
        {
            CurrentLevelNum = 1;
            ComingSoon = 1;
        }
        
        RestartLevel();
    }

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void AddDiamonds()
    {
        if (diamonds == 0 || (diamonds % 5 == 0 && diamonds % 10 != 0))
        {
            diamonds += 5;
            _uiManager.ShowDiamondPlayText("5");
            return;
        }

        diamonds += 10;
        _uiManager.ShowDiamondPlayText("10");
    }
    
    private IEnumerator DiamondsCount()
    {
        yield return new WaitForSeconds(3f);
        for(int curDiamonds = 0; curDiamonds <= diamonds; curDiamonds += 5)
        {
            _uiManager.SetWinDiamonds(curDiamonds);
            _soundManager.CreateDiamondSound();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator CreateWinSound()
    {
        yield return new WaitForSeconds(1.5f);
        _soundManager.CreateSoundAndDestroy(_soundManager.winSound, 2f, _targetCam.transform);
    }

    public void OpenComingSoon()
    {
        if (ComingSoon != 0)
        {
            _uiManager.OpenComingSoon();
        }
    }
    
    public void CloseComingSoon()
    {
        _uiManager.CloseComingSoon();
        ComingSoon = 0;
    }

    public void OpenShop()
    {
        _targetCam.destroyer.SetActive(false);
        _targetCam.cameraTransformState = CameraTransformState.Shop;
        _uiManager.OpenShop();
    }

    public void CloseShop()
    {
        _targetCam.destroyer.SetActive(true);
        _targetCam.cameraTransformState = CameraTransformState.Standard;
        _uiManager.CloseShop();
    }
}

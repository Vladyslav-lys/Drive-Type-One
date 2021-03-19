using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : BaseManager<UIManager>
{
    public GameObject mainUI;
    public GameObject playUI;
    public GameObject pauseUI;
    public GameObject settingsUI;
    public GameObject winUI;
    public GameObject loseUI;
    public GameObject creditsUI;
    public GameObject tutorialUI;
    public GameObject comingSoonUI;
    public GameObject shopUI;
    public GameObject diamondPlayObj;
    public GameObject nextLevelBtnObj;
    public GameObject restartBtnObj;
    public GameObject levelHolderObj;
    public GameObject tapTutorialTextObj;
    public GameObject keyboardLikeObj;
    public GameObject[] shopCars;
    public RectTransform levelLineTransform;
    public Animator diamondPlayAnimator;
    public Animator wordAnimator;
    public Animator pauseAnimator;
    public Animator settingsAnimator;
    public Image backPauseImage;
    public Key[] keys;
    public TextMeshProUGUI diamondMainText, diamondWinText, diamondPlayText;
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI vibrationPauseText, vibrationSettingText;
    public TextMeshProUGUI soundPauseText, soundSettingText;
    public TextMeshProUGUI currentLevelText, nextLevelText;
    public float maxLevelLineWidth;
    private int _linePartsCount;
    private float _partLength;
    private float _targetLinelength;
    private Animator _keyboardLikeAnimator;
    private Key _curKey;
    
    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        OnOffByKey("Sound",new List<TextMeshProUGUI> {soundPauseText, soundSettingText});
        OnOffByKey("Vibration",new List<TextMeshProUGUI> {vibrationPauseText, vibrationSettingText});
        levelLineTransform.sizeDelta = new Vector2(0f, levelLineTransform.sizeDelta.y);
        _keyboardLikeAnimator = keyboardLikeObj.GetComponent<Animator>();
    }

    private void Start()
    {
        _linePartsCount = GameManager.instance.CurrentLevel.words.Count;
        _partLength = (float)Math.Ceiling(maxLevelLineWidth / _linePartsCount);
        _targetLinelength = _partLength;
    }

    private void OnOffByKey(string key, List<TextMeshProUGUI> textMeshes)
    {
        if (PlayerPrefs.GetInt(key, 1) == 0)
        {
            foreach (var textMesh in textMeshes)
            {
                textMesh.text = key.ToUpper() + " OFF";
            }
        }
        else
        {
            foreach (var textMesh in textMeshes)
            {
                textMesh.text = key.ToUpper() + " ON";
            }
        }
    }

    public void TutorialGame(bool isTutorial) => tutorialUI.SetActive(isTutorial);

    public void StartGame()
    {
        mainUI.SetActive(false);
        playUI.SetActive(true);
    }

    public void PauseGame(bool isPaused)
    {
        pauseAnimator.enabled = !isPaused;
        if(isPaused)
            backPauseImage.color = new Color(backPauseImage.color.r, backPauseImage.color.g, backPauseImage.color.b, 0.29f);
        pauseUI.SetActive(isPaused);
        //playUI.SetActive(!isPaused);
    }

    public void LoseGame()
    {
        playUI.SetActive(false);
        levelHolderObj.SetActive(false);
        Invoke(nameof(ShowLosePanel), 1.2f);
    }

    private void ShowLosePanel()
    {
        loseUI.SetActive(true);
        Invoke(nameof(ShowRestartBtnObj), 0.5f);
    }

    public void WinGame()
    {
        playUI.SetActive(false);
        levelHolderObj.SetActive(false);
        Invoke(nameof(ShowWinPanel), 1.5f);
    }

    private void ShowWinPanel()
    {
        winUI.SetActive(true);
        Invoke(nameof(ShowDiamondBtnObj), 0.5f);
        Invoke(nameof(ShowNextLevelBtnObj), 1f);
    }

    private void ShowDiamondBtnObj() => diamondPlayObj.SetActive(true);
    
    private void ShowNextLevelBtnObj() => nextLevelBtnObj.SetActive(true);
    
    private void ShowRestartBtnObj() => restartBtnObj.SetActive(true);

    public void OpenSettings()
    {
        if(settingsAnimator.GetBool("Close"))
            settingsAnimator.SetBool("Close", false);
        settingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsAnimator.SetBool("Close", true);
        Invoke(nameof(FalseSettings), 0.3f);
    }
    
    public void OpenCredits()
    {
        settingsUI.SetActive(false);
        creditsUI.SetActive(true);
    }

    public void CloseCredits()
    {
        settingsUI.SetActive(true);
        creditsUI.SetActive(false);
    }
    
    public void OpenShop()
    {
        levelHolderObj.SetActive(false);
        mainUI.SetActive(false);
        shopUI.SetActive(true);
    }

    public void CloseShop()
    {
        levelHolderObj.SetActive(true);
        mainUI.SetActive(true);
        shopUI.SetActive(false);
    }
    
    public void OpenComingSoon() => comingSoonUI.SetActive(true);

    public void CloseComingSoon() => comingSoonUI.SetActive(false);
    
    public void SetWinDiamonds(int diamonds) => diamondWinText.text = "+" + diamonds;
    
    public void ShowDiamondPlayText(string countText)
    {
        diamondPlayText.text = "+" + countText;
        diamondPlayAnimator.SetBool("Show",true);
        Invoke(nameof(FalseDiamondShowAnim), 1f);
    }

    private void FalseDiamondShowAnim() => diamondPlayAnimator.SetBool("Show",false);

    public void ShowWord() => wordAnimator.SetBool("Show",true);

    public void CloseWord() => wordAnimator.SetBool("Show",false);
    
    private void FalseSettings() => settingsUI.SetActive(false);
    
    public void LevelLineUp() => StartCoroutine(LevelLineUpEnum());

    private IEnumerator LevelLineUpEnum()
    {
        while (levelLineTransform.sizeDelta.x < _targetLinelength)
        {
            levelLineTransform.sizeDelta += new Vector2(5f,0f);
            yield return new WaitForSeconds(0.02f);
        }

        _targetLinelength += _partLength;
    }

    public void ShowTapTutorialTextInTime(float delay) => StartCoroutine(InvokeRealTime(ShowTapTutorialText, delay));
        
    private void ShowTapTutorialText() => tapTutorialTextObj.SetActive(true);
    
    private IEnumerator InvokeRealTime(Action invokedMethod, float delay)
    {
        yield return new WaitForSecondsRealtime(1f);
        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        invokedMethod?.Invoke();
    }

    public void SetShopCar(int lastCarNum, int shopCarNum)
    {
        shopCars[lastCarNum].SetActive(false);
        SetShopCar(shopCarNum);
    }

    public void SetShopCar(int shopCarNum) => shopCars[shopCarNum].SetActive(true);

    public void SetKeyboardLikeUp(bool isUp) => _keyboardLikeAnimator.SetBool("Up",isUp);

    public void SetBtnColorOnKeyPress(string keyString)
    {
        if(keyString.Length < 1)
            return;
        
        foreach (var curKey in keys)
        {
            if (curKey.keyName.ToUpper().Equals(keyString.ToUpper()))
            {
                _curKey = curKey;
                break;
            }
        }

        if (!_curKey)
            return;

        _curKey.keyImage.color = new Color(0.8f, 0.8f, 0.8f);
        Invoke(nameof(SetBtnColorToUnclicked),0.1f);
    }

    private void SetBtnColorToUnclicked()
    {
        _curKey.keyImage.color = new Color(1f, 1f, 1f);
    }
}
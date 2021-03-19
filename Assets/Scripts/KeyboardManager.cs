using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManager : BaseManager<KeyboardManager>
{
    public delegate void Method();

    public Method WinGame;
    public Method AddDiamonds;
    public List<string> words;
    public bool canClick;
    private PlayerController _playerController;
    private GameManager _gm;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private TouchScreenKeyboard _keyboard;
    private Camera _camera;

    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        canClick = true;
    }

    private void Start()
    {
        _playerController = PlayerController.instance;
        _gm = GameManager.instance;
        _uiManager = UIManager.instance;
        _soundManager = SoundManager.instance;
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_gm.CanPlay())
            return;
        
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Backspace))
        {
            DeleteLastLetter();
            _uiManager.SetBtnColorOnKeyPress("Del");
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            Ok();
            _uiManager.SetBtnColorOnKeyPress("Ok");
        }
        else if (Input.anyKey)
        {
            KeyLetter(Input.inputString);
            _uiManager.SetBtnColorOnKeyPress(Input.inputString);
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if(_keyboard == null)
            return;
        
        if (_keyboard.done || Input.GetKeyDown(KeyCode.Return))
            Ok();
        else if (_keyboard.text.Length > 0)
            SetWord(_keyboard.text);
#endif
    }

    public void KeyLetter(string key) => _uiManager.wordText.text += key;

    public void SetWord(string word) => _uiManager.wordText.text = word;

    public void Ok()
    {
        if(!canClick)
            return;
        
        canClick = false;
        if (_uiManager.wordText.text.ToUpper() == words.FirstOrDefault().ToUpper())
        {
            AddDiamonds?.Invoke();
            words.RemoveAt(0);
            _playerController.SetTurn();
            _uiManager.wordText.text = "";
            _uiManager.LevelLineUp();
            _soundManager.CreateSoundAndDestroy(_soundManager.turnSound, 1f, _playerController.transform);
            _soundManager.CreateSoundAndDestroy(_soundManager.completeWordSound, 1f, _playerController.transform);
            if (words.Count == 0)
            {
                WinGame?.Invoke();
            }
        }
        else
            _soundManager.CreateSoundAndDestroy(_soundManager.wrongWordSound, 1f, _playerController.transform);
        _soundManager.SetPitch(_soundManager.drivingSound, 1f);
        _uiManager.CloseWord();
        _playerController.IncreaseSpeed();
        _gm.IncreaseAllLevelOtherCarsSpeed();
        CloseKeyboard();
    }

    public void DeleteLastLetter()
    {
        if (_uiManager.wordText.text.Length <= 0 || !canClick)
            return;

        _uiManager.wordText.text = _uiManager.wordText.text.Substring(0, _uiManager.wordText.text.Length - 1);
        canClick = false;
        Invoke(nameof(TrueClick), 0.2f);
    }

    public void OpenKeyboard()
    {
        TrueClick();
        _uiManager.SetKeyboardLikeUp(true);
#if UNITY_ANDROID || UNITY_IOS
        TouchScreenKeyboard.hideInput = true;
        _keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false);
#endif
    }

    public void CloseKeyboard()
    {
        _uiManager.SetKeyboardLikeUp(false);
#if UNITY_ANDROID || UNITY_IOS
        if(_keyboard == null)
            return;
        
        _keyboard.active = false;
        _keyboard = null;
#endif
    }

    private void TrueClick() => canClick = true;
}

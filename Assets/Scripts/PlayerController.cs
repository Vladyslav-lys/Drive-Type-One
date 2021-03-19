using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class PlayerController : BaseCarController
{
    public static PlayerController instance;
    public float[] xPos;
    private KeyboardManager _km;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private SkinManager _skinManager;
    private Camera _camera;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
    }

    protected override void Start()
    {
        base.Start();
        _km = KeyboardManager.instance;
        _uiManager = UIManager.instance;
        _soundManager = SoundManager.instance;
        _camera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        //ANOTHER CAR ZONE
        if (other.gameObject.layer == 7)
        {
            ReduceSpeed();
            Gm.ReduceAllLevelOtherCarsSpeed();
            _km.OpenKeyboard();
            _uiManager.ShowWord();
            _soundManager.SetPitch(_soundManager.drivingSound, -0.1f);
            _soundManager.CreateSoundAndDestroy(_soundManager.slowDownSound, 1f, _camera.transform);
            Gm.TutorialGameInTime(1f);
        }
        
        //ANOTHER CAR
        if (other.gameObject.layer == 6)
        {
            Gm.LoseGame();
            NullifyVelocity();
            SetStartAnim(false);
            SetLoseAnim();
            _soundManager.Vibrate();
            _soundManager.CreateSoundAndDestroy(_soundManager.carSkidSound, 0.7f, _camera.transform);
            _soundManager.CreateSoundAndDestroy(_soundManager.hitSound, 1f, _camera.transform);
            _km.CloseKeyboard();
            other.GetComponent<OtherCarsController>().SetLoseAnim();
        }
    }

    public void SetTurn()
    {
        if (moveX == xPos[0])
        {
            animator.SetBool("TurnLeft",true);
            moveX = xPos[1];
            Invoke(nameof(OffTurnLeft),1f);
            return;
        } if (moveX == xPos[1])
        {
            animator.SetBool("TurnRight",true);
            moveX = xPos[0];
            Invoke(nameof(OffTurnRight),1f);
        }
    }
    
    private void OffTurnLeft() => animator.SetBool("TurnLeft",false);
    
    private void OffTurnRight() => animator.SetBool("TurnRight",false);
}

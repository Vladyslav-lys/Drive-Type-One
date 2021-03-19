using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : BaseManager<SkinManager>
{
    public GameObject[] skins;
    private UIManager _uiManager;
    private PlayerController _playerController;
    
    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        CurrentSkin.SetActive(true);
    }

    private void Start()
    {
        _playerController = PlayerController.instance;
        _playerController.SetAnimator(CurrentSkin.GetComponent<Animator>());
        _uiManager = UIManager.instance;
        _uiManager.SetShopCar(SkinNum);
    }

    public int SkinNum
    {
        get => PlayerPrefs.GetInt("Skin", 0);
        set => PlayerPrefs.SetInt("Skin", value);
    }

    public GameObject CurrentSkin => skins[SkinNum];

    public void NextCarShop() => SetSkinByCondition(SkinNum < skins.Length-1, SkinNum, () => ++SkinNum);

    public void PrevCarShop() => SetSkinByCondition(SkinNum > 0, SkinNum, () => --SkinNum);

    private void SetSkinByCondition(bool condition, int lastCarNum, Func<int> func)
    {
        if (!condition)
            return;
        
        skins[lastCarNum].SetActive(false);
        int shopCarNum = func();
        _uiManager.SetShopCar(lastCarNum, shopCarNum);
        skins[shopCarNum].SetActive(true);
        _playerController.SetAnimator(CurrentSkin.GetComponent<Animator>());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundManager : BaseManager<SoundManager>
{
    public GameObject hitSound;
    public GameObject carSkidSound;
    public GameObject drivingSound;
    public GameObject startSound;
    public GameObject preparingSound;
    public GameObject turnSound;
    public GameObject slowDownSound;
    public GameObject tapSound;
    public GameObject completeWordSound;
    public GameObject wrongWordSound;
    public GameObject diamondCountSound;
    public GameObject winSound;
    private UIManager _uiManager;
    private Dictionary<GameObject, GameObject> _createdSounds;
    
    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        _createdSounds = new Dictionary<GameObject, GameObject>();
    }

    private void Start()
    {
        _uiManager = UIManager.instance;
    }

    public int Sound
    {
        get => PlayerPrefs.GetInt("Sound", 1);
        set => PlayerPrefs.SetInt("Sound", value);
    }

    public int Vibration
    {
        get => PlayerPrefs.GetInt("Vibration", 1);
        set => PlayerPrefs.SetInt("Vibration", value);
    }

    public void OnOffSound()
    {
        OnOffByKey("Sound", 
            new List<TextMeshProUGUI> {_uiManager.soundPauseText, _uiManager.soundSettingText});
    }

    public void OnOffVibration()
    {
        OnOffByKey("Vibration",
            new List<TextMeshProUGUI> {_uiManager.vibrationPauseText, _uiManager.vibrationSettingText});
    }

    private void OnOffByKey(string key, List<TextMeshProUGUI> textMeshes)
    {
        if (PlayerPrefs.GetInt(key, 1) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            foreach (var textMesh in textMeshes)
            {
                textMesh.text = key.ToUpper() + " ON";
            }
        }
        else
        {
            PlayerPrefs.SetInt(key, 0);
            foreach (var textMesh in textMeshes)
            {
                textMesh.text = key.ToUpper() + " OFF";
            }
        }
    }

    public void Vibrate()
    {
        if(Vibration != 0)
            Handheld.Vibrate();
    }

    public void CreateSoundAndDestroy(GameObject soundPref, float destroyDelay, Transform parentTransform)
    {
        CreateSound(soundPref, parentTransform);
        DestroySound(soundPref, destroyDelay);
    }

    public void CreateSound(GameObject soundPref, Transform parentTransform)
    {
        if(!soundPref)
            return;
        
        GameObject soundObj = GameObject.Instantiate(soundPref);
        soundObj.transform.SetParent(parentTransform);
        if(Sound != 0)
            soundObj.GetComponent<AudioSource>().Play();
        _createdSounds.Add(soundPref, soundObj);
    }

    public void DestroySound(GameObject soundKeyObj, float destroyDelay)
    {
        if (!soundKeyObj || !_createdSounds[soundKeyObj])
            return;

        Destroy(_createdSounds[soundKeyObj], destroyDelay);
        _createdSounds.Remove(soundKeyObj);
    }

    public void StopSound(GameObject soundKeyObj)
    {
        AudioSource audio = _createdSounds[soundKeyObj].GetComponent<AudioSource>();
        
        if(!audio)
            return;
        
        audio.Pause();
    }

    public void ContinueSound(GameObject soundKeyObj)
    {
        AudioSource audio = _createdSounds[soundKeyObj].GetComponent<AudioSource>();
        
        if(Sound == 0 || !audio)
            return;
        
        audio.Play();
    }

    public void DecreaseVolumeAndOffLoopSound(GameObject soundKeyObj)
    {
        AudioSource audio = _createdSounds[soundKeyObj].GetComponent<AudioSource>();
        
        if(!audio)
            return;

        audio.loop = false;
        audio.volume = 0.1f;
    }

    public void RestartSound(GameObject soundKeyObj)
    {
        AudioSource audio = _createdSounds[soundKeyObj].GetComponent<AudioSource>();
        
        if(Sound == 0 || !audio)
            return;
        
        audio.Stop();
        audio.Play();
    }

    public void SetPitch(GameObject soundKeyObj, float pitch)
    {
        AudioSource audio = _createdSounds[soundKeyObj].GetComponent<AudioSource>();
        
        if(!audio)
            return;

        audio.pitch = pitch;
    }

    public void StopAll()
    {
        foreach (var createdSoundKey in _createdSounds.Keys)
        {
            StopSound(createdSoundKey);
        }
    }

    public void ContinueAll()
    {
        foreach (var createdSoundKey in _createdSounds.Keys)
        {
            ContinueSound(createdSoundKey);
        }
    }

    public void CreateTapSound() => CreateOrContinue(tapSound);

    public void CreateDiamondSound() => CreateOrContinue(diamondCountSound);

    private void CreateOrContinue(GameObject soundKeyObj)
    {
        if (!_createdSounds.ContainsKey(soundKeyObj))
        {
            CreateSound(soundKeyObj, null);
            return;
        }
        ContinueSound(soundKeyObj);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SpawnManager : BaseManager<SpawnManager>
{
    public GameObject roadObj;
    public GameObject[] environmentObjs;
    public float startSpawnTime;
    public float intervalSpawnTime;
    public float offsetZRoad;
    public float offsetZEnv;
    public int envNum;
    public Vector3 spawnPosEnv;
    private Vector3 _spawnPosRoad;
    
    protected override void Awake()
    {
        if (instance == null)
            instance = this;
        base.Awake();
    }

    protected override void Initialize()
    {
        _spawnPosRoad = roadObj.transform.position;
    }

    private void SpawnRoad()
    {
        Instantiate(environmentObjs[envNum], spawnPosEnv, Quaternion.identity);
        Instantiate(roadObj,_spawnPosRoad, Quaternion.identity);
        _spawnPosRoad += new Vector3(0f,0f,offsetZRoad);
        spawnPosEnv += new Vector3(0f,0f,offsetZEnv);
    }

    public void SpawnRepeat() => InvokeRepeating(nameof(SpawnRoad), startSpawnTime, intervalSpawnTime);

    public void StartSpawn()
    {
        for (int i = 0; i < 11; i++)
        {
            SpawnRoad();
        }
    }

    public void InterruptSpawn()
    {
        if(IsInvoking(nameof(SpawnRoad)))
            CancelInvoke(nameof(SpawnRoad));
    }
}

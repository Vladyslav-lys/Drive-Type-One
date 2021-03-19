using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<string> words;
    public List<OtherCarsController> otherCars;
    public float startPlayerX;

    private void Start()
    {
        for (int i = 0; i < otherCars.Count; i++)
        {
            otherCars[i].SetWord(words[i]);
        }
    }

    public void StopAllLevelAnotherCars()
    {
        foreach (var otherCar in otherCars)
        {
            otherCar.NullifyVelocity();
        }
    }

    public void ReduceAllCarsSpeed()
    {
        foreach (var otherCar in otherCars)
        {
            otherCar.ReduceSpeed();
        }
    } 
    
    public void IncreaseAllCarsSpeed()
    {
        foreach (var otherCar in otherCars)
        {
            otherCar.IncreaseSpeed();
        }
    }

    public void NullifyVelocityAllCars()
    {
        foreach (var otherCar in otherCars)
        {
            otherCar.NullifyVelocity();
        }
    }
}

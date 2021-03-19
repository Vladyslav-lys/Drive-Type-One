using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : MonoBehaviour
{
    public static T instance;

    protected virtual void Awake() => Initialize();

    protected virtual void Initialize() {}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraTransformState
{
    Standard, Finish, Shop
}

public class TargetCamera : MonoBehaviour
{
    public CameraTransformState cameraTransformState;
    public Transform shopTransform;
    public Transform standardTransform;
    public Transform winTransform;
    public GameObject destroyer;
    private GameManager _gm;
    
    private void Start()
    {
        _gm = GameManager.instance;
    }

    private void Update()
    {
        switch (cameraTransformState)
        {
            case CameraTransformState.Standard:
                TransformTowards(standardTransform);
                break;
            case CameraTransformState.Shop:
                TransformTowards(shopTransform);
                break;
            case CameraTransformState.Finish:
                return;
            default:
                return;
        }
    }

    private void TransformTowards(Transform targetTransform)
    {
        transform.position = targetTransform.position;
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetTransform.rotation, 300f * Time.deltaTime);
    }
}

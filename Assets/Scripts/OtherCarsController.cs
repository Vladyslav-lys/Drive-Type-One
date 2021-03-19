using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherCarsController : BaseCarController
{
    public string word;
    public TextMeshProUGUI letterText;
    public GameObject otherCarPrefab;

    protected override void Awake()
    {
        base.Awake();
        Transform createdTransform = Instantiate(otherCarPrefab, otherCarPrefab.transform.position,
            Quaternion.identity,
            transform).transform;
        createdTransform.localPosition = otherCarPrefab.transform.localPosition;
        OtherCar otherCar = createdTransform.GetComponent<OtherCar>();
        animator = otherCar.animator;
        letterText = otherCar.letterText;
    }

    protected override void Start()
    {
        base.Start();
        moveX = transform.position.x;
    }

    public void SetWord(string word)
    {
        this.word = word.ToUpper();
        letterText.text = word.ToUpper();
    }
}

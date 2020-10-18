using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class TargetStats : MonoBehaviour
{
    
    
    [SerializeField] private TMP_Text _hitText;
    [SerializeField] private TMP_Text _hitPerSecond;

    [SerializeField] private float _localHitDelay = 1.0f;
    [SerializeField] private float _prevAverage = 0f;
    
    private int _totalHits;
    private int _localHitsPerSecond;
    private float _localTimeStart;

    private void Awake()
    {
        
        var shootable = GetComponent<Shootable>();
        shootable.Shot += ShootableOnShot;
        FixText();
    }

    private void ShootableOnShot(object sender, ShotEventArgs e)
    {
        if (Time.time - _localTimeStart >= + _localHitDelay)
        {
            _prevAverage = CalcAverage();
            _localHitsPerSecond = 0;
            _localTimeStart = Time.time;
        }

        _totalHits++;
        _localHitsPerSecond++;
        FixText();
    }

    float CalcAverage()
    {
        var hps = (_localHitsPerSecond / _localHitDelay);
        var places = Mathf.Pow(10, 2);
        hps = Mathf.Round(hps * places) / places;
        return hps;
    }

    private void FixText()
    {
        _hitText.text = _totalHits.ToString();
        
        _hitPerSecond.text = _prevAverage.ToString(CultureInfo.InvariantCulture);
    }
}

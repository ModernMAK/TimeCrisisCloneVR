using System;
using UnityEngine;

[Serializable]
public struct FiringInfo
{
#pragma warning disable 649
    [SerializeField] private float _fireCooldown;
    [SerializeField] private int _pellets;
    [SerializeField] private bool _allowAutofire;
    [SerializeField]
    private float _timeToSpread;
    [SerializeField]
    private float _timeToRecover;
#pragma warning restore 649
    public int Pellets => _pellets;
    public float FireCooldown => _fireCooldown;
    public bool AllowAutoFire => _allowAutofire;
    public float SpreadGain => _timeToSpread != 0f ? 1f / _timeToSpread : 0f;
    public float SpreadRecovery => _timeToRecover != 0f ? 1f / _timeToRecover : 0f;
}
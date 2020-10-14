using System;
using UnityEngine;

[Serializable]
public struct FiringInfo
{
    [SerializeField] private float _fireCooldown;
    [SerializeField] private int _pellets;
    public int Pellets => _pellets;
    public float FireCooldown => _fireCooldown;
}
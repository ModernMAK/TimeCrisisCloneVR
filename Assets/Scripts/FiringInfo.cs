using System;
using UnityEngine;

[Serializable]
public struct FiringInfo
{
    #pragma warning disable 649
    [SerializeField] private float _fireCooldown;
    [SerializeField] private int _pellets;
    #pragma warning restore 649
    public int Pellets => _pellets;
    public float FireCooldown => _fireCooldown;
}
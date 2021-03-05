using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunMagazine))]
[RequireComponent(typeof(GunRayCaster))]
public class Gun : MonoBehaviour, IGun
{
	//public Gun()
	//{
	//    _ammoState = new AmmoState(this);
	//    _reloadingState = new ReloadingState(this);
	//}

	//protected Gun(IGun attachedGun)
	//{
	//    _ammoState = new AmmoState(attachedGun);
	//    _reloadingState = new ReloadingState(attachedGun);
	//}

	[SerializeField] private GunData _gunData;
	private GunMagazine _magazine;
	private GunRayCaster _rayCaster;
	private ReloadingState _reloadingState;
	private AimCone _aimCone;
	private FiringInfo _firingInfo;
	private float _lastAction;

	private void Awake()
	{
		_magazine = GetComponent<GunMagazine>();
		_rayCaster = GetComponent<GunRayCaster>();
		_reloadingState = new ReloadingState(this);
		Initialize(_gunData);
	}

	public float LastAction
	{
		get => _lastAction;
		private set => _lastAction = value;
	}

	[SerializeField] private bool _useRandomSpread = false;

	public GunMagazine Magazine => _magazine;
	public GunRayCaster RayCaster => _rayCaster;

	public bool CanFire => (_firingInfo.FireCooldown + LastAction - Time.time <= 0f);



	void Update()
	{
		if(_reloadingState.IsReloading)
			_reloadingState.PerformReload(ref _lastAction, Magazine);
	}
	public void Stop()
	{
		_reloadingState.StopReloading();
	}

	public void Reload()
	{
		_reloadingState.StartReloading();
		_reloadingState.PerformReload(ref _lastAction, Magazine);
	}


	public void Fire()
	{
		if (CanFire)
		{
			if (!_magazine.HasAmmo && !GlobalSettings.CheatCodes.InfiniteAmmo)
			{
				OnFiredEmpty(new EventArgs());
				LastAction = Time.time;
				_reloadingState.StopReloading();
			}
			else
			{
				_debugs = _rayCaster.GetRays(_firingInfo.Pellets, _aimCone, 2, _useRandomSpread);
				foreach (var ray in _debugs)
					if (_rayCaster.Cast(ray, out var hitInfo))
						if (hitInfo.rigidbody != null)
						{
							var shootable = hitInfo.rigidbody.GetComponent<Shootable>();
							if (shootable != null)
							{
								shootable.TakeShot(hitInfo.point, ray.direction, hitInfo.normal);
							}
						}

				OnFired(new FiredEventArgs() { Raycasts = _debugs });
				if (!GlobalSettings.CheatCodes.InfiniteAmmo)
				{
					_magazine.CurrentAmmo--;
					_reloadingState.StopReloading();
				}
				LastAction = Time.time;

			}
		}
	}


	public event EventHandler<FiredEventArgs> Fired;
	public event EventHandler FiredEmpty;


	public void Initialize()
	{
		LastAction = Mathf.NegativeInfinity;
		_magazine.CurrentAmmo = _magazine.MaxAmmo;
	}

	public void Initialize(GunData data)
	{
		_aimCone = data.AimCone;
		_firingInfo = data.FiringInfo;
		_magazine.MaxAmmo = data.AmmoData.MaxAmmo;
		_reloadingState.CopyFrom(data.ReloadingData);
		_reloadingState.StopReloading();
		Initialize();
	}

	private Ray[] _debugs;
	public Ray[] DebugRays => _debugs;

	public ReloadingState ReloadingState => _reloadingState;


	public void OnDrawGizmos()
	{
		const float DrawRange = 10f;
		const float ImpactSize = 0.1f;
		if (_debugs == null)
			return;
		for (var i = 0; i < _debugs.Length; i++)
		{
			var ray = _debugs[i];
			if (i != 0)
			{
				var hue = 1f / (_debugs.Length - 1) * (i - 1);
				Gizmos.color = Color.HSVToRGB(hue, 1f, 1f);
			}
			else
				Gizmos.color = Color.white;

			Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * DrawRange);
			Gizmos.DrawSphere(ray.origin + ray.direction * DrawRange, ImpactSize);			
		}
	}


	protected virtual void OnFired(FiredEventArgs args)
	{
		Fired?.Invoke(this, args);
	}


	protected virtual void OnFiredEmpty(EventArgs args)
	{
		FiredEmpty?.Invoke(this, args);
	}
}

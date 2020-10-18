using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(IGun))]
public class GunAudioManager : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private AudioClip[] _firingClipPool;
    [SerializeField] private AudioClip[] _firingEmptyClipPool;
    [SerializeField] private AudioClip[] _reloadingClipPool;
    [SerializeField] private AudioClip[] _reloadingStartClipPool;
    [SerializeField] private AudioClip[] _reloadingEndClipPool;

    [SerializeField] private AudioSource _source;
#pragma warning restore 649
    private void Awake()
    {
        if (_source == null)
        {
            _source = GetComponent<AudioSource>();
            if (_source == null)
                _source = gameObject.AddComponent<AudioSource>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var gun = GetComponent<IGun>();
        gun.Fired += PlayFireClip;
        gun.FiredEmpty += PlayFireEmptyClip;

        gun.ReloadingState.Reloading += PlayReloadClip;
        gun.ReloadingState.ReloadingStarted += PlayReloadStartClip;
        gun.ReloadingState.ReloadingEnded += PlayReloadEndClip;
    }


    void PlayRandomClip(IList<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0)
        {
            Debug.LogWarning("No Clips Provided!");
            return;
        }

        var clipId = Random.Range(0, clips.Count);
        _source.PlayOneShot(clips[clipId]);
    }

    void PlayFireClip(object sender, EventArgs args) => PlayRandomClip(_firingClipPool);
    void PlayReloadClip(object sender, EventArgs args) => PlayRandomClip(_reloadingClipPool);
    void PlayReloadStartClip(object sender, EventArgs args) => PlayRandomClip(_reloadingStartClipPool);
    void PlayReloadEndClip(object sender, EventArgs args) => PlayRandomClip(_reloadingEndClipPool);
    void PlayFireEmptyClip(object sender, EventArgs args) => PlayRandomClip(_firingEmptyClipPool);
}
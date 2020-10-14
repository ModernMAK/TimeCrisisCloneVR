using System;
using Random = UnityEngine.Random;

public class TempRandom : IDisposable
{
    private readonly Random.State _state;
    public TempRandom(int newSeed)
    {
        _state = Random.state;
        Random.InitState(newSeed);
    }
    public TempRandom(Random.State newState)
    {
        _state = Random.state;
        Random.state = newState;
    }

    public void Dispose()
    {
        Random.state = _state;
    }
}
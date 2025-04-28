using UnityEngine;

public class WaitAction : IAgentAction
{
    private float _waitTime;
    private float _time;
    public WaitAction(float time)
    {
        _waitTime = time;
        _time = 0f;
    }

    public bool Complete => _time >= _waitTime;

    public void Update(float deltaTime)
    {
        _time += Time.deltaTime;
    }
}

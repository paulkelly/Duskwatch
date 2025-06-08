using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class DuskwatchAgent : MonoBehaviour
{
    private const float DestinationThreshold = 1.2f;
    
    protected static readonly int SpeedAnimationHash = Animator.StringToHash("Speed");
    protected static readonly int AttackAnimationHash = Animator.StringToHash("Attack");

    [SerializeField] protected Animator _animator;
    private IAstarAI _navAgent;
    
    private Queue<IAgentAction> _actionQueue = new Queue<IAgentAction>();

    private IAgentTask _currentTask;
    private IAgentAction _currentAction;
    private float _lastVelocityUpdate;

    public Vector3 Position { get; private set; }
    public Vector3 Destination { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Transform RotationTarget { get; set; }
    public float Speed { get; private set; }
    public float MaxSpeed { get; private set; }
    public void SetDestination(Vector3 destination)
    {
        Destination = destination;
        _navAgent.destination = Destination;
    }
    public void Halt() => _navAgent.destination = _navAgent.position;
    public bool HasReachedDestination(float targetDistance) => Vector3.Distance(Position, Destination) < targetDistance + DestinationThreshold;

    public void OnAnimationHit() // Called From Animation Behaviour
    {
        if(_currentAction != null) _currentAction.OnAgentHit();
    }

    public void QueueAction(IAgentAction action)
    {
        _actionQueue.Enqueue(action);
    }
    public abstract void FindNewTask();

    private void Awake()
    {
        _navAgent = GetComponent<IAstarAI>();
        MaxSpeed = _navAgent.maxSpeed;
    }

    private void Update()
    {
        Profiler.BeginSample("Update Agent");
        Position = transform.position;
        _lastVelocityUpdate += Time.deltaTime;
        if (RotationTarget)
        {
            transform.LookAt(RotationTarget);
        }
        if (_lastVelocityUpdate >= 0.1f)
        {
            _lastVelocityUpdate = 0f;
            Velocity = _navAgent.velocity;
            Speed = Velocity.magnitude;
            _animator.SetFloat(SpeedAnimationHash, Mathf.Clamp01(Speed/MaxSpeed));
        }
        
        if (_currentAction == null)
        {
            if (_actionQueue.Count == 0)
            {
                StopCurrentTask();
                FindNewTask();
            }
            else
            {
                _currentAction = _actionQueue.Dequeue();
                try
                {
                    _currentAction.Start();
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.LogError("Caught Exception while starting agent action");
                    Debug.LogException(e);
#endif
                    _currentAction = null;
                }
            }
        }
        else
        {
            if (_currentAction.Complete)
            {
                StopCurrentAction();
            }
            else
            {
                try
                {
                    _currentAction.Update(Time.deltaTime);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.LogError("Caught Exception while updating agent action");
                    Debug.LogException(e);
#endif
                    _currentAction = null;
                }
            }
        }
        Profiler.EndSample();
    }

    private void StopCurrentTask()
    {
        if (_currentTask != null) _currentTask.StopTask(this);
        _currentTask = null;
    }
    
    private void StopCurrentAction()
    {
        try
        {
            if(_currentAction != null) _currentAction.Stop();
        }
        catch (Exception e)
        {
#if DEBUG
            Debug.LogError("Caught Exception while updating agent action");
            Debug.LogException(e);
#endif
        }
        _currentAction = null;
    }

    protected void SetTask(IAgentTask task)
    {
        StopCurrentTask();
        StopCurrentAction();
        _actionQueue.Clear();
        _currentTask = task;
        task.StartTask(this);
    }
}

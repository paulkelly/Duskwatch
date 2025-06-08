using UnityEngine;

public class EnemyAgent : DuskwatchAgent, IDestroyReaction
{
    [SerializeField] private CharacterAttack _attack;

    private Health _target;
    
    public override void FindNewTask()
    {
        SetTask(new MoveToTownCenterTask());
    }
    
    public void OnDestroyed()
    {
        Destroy(gameObject);
    }

    public void OnResurrect()
    {
    }
}

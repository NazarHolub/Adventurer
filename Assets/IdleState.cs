using UnityEngine;

public class IdleState : State
{
    public IdleState(EnemyAI enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.PlayAnimation("Idle");
    }

    public override void LogicUpdate()
    { 
        if (enemy.IsDetected())
            enemy.SetState(EnemyAI.States.Move);
    }
}
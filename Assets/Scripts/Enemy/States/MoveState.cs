using UnityEngine;
public class MoveState : State
{
    public MoveState(EnemyAI enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.PlayAnimation("Run");
    }

    public override void PhysicsUpdate()
    {
        if (enemy.IsDetected() || enemy._Follow())
        {
            enemy.LookAtPlayer();

            if (enemy.CanAttack())
                enemy.SetState(EnemyAI.States.Attack);
            else
                enemy.Move();
        }
        else
        {
            enemy.SetState(EnemyAI.States.Idle);
        }    
    }

    public override void Exit()
    {
        enemy.StopMoving();
    }
}
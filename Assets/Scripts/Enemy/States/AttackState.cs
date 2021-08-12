using UnityEngine;
public class AttackState : State
{
    public AttackState(EnemyAI enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.PlayAnimation("Attack");
    }

    public override void LogicUpdate()
    {
        if (!enemy.CanAttack() && !enemy.IsAnimationPlaying("Attack"))
        {
            enemy.LookAtPlayer();
            enemy.SetState(EnemyAI.States.Move);
        }
    }

}


using System;

namespace TowerDefense.Domain.Enemy.Entities
{
    public enum EnemyStateType
    {
        Moving,
        Damaged,
        Dead
    }

    public abstract class EnemyState
    {
        protected readonly EnemyEntity Enemy;

        protected EnemyState(EnemyEntity enemy)
        {
            Enemy = enemy;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
    }

    public class MovingState : EnemyState
    {
        public MovingState(EnemyEntity enemy) : base(enemy) { }

        public override void Enter()
        {
            // 进入移动状态的逻辑
        }

        public override void Exit()
        {
            // 退出移动状态的逻辑
        }

        public override void Update()
        {
            // 移动状态的更新逻辑
        }
    }

    public class DamagedState : EnemyState
    {
        public DamagedState(EnemyEntity enemy) : base(enemy) { }

        public override void Enter()
        {
            // 进入受伤状态的逻辑
        }

        public override void Exit()
        {
            // 退出受伤状态的逻辑
        }

        public override void Update()
        {
            // 受伤状态的更新逻辑
        }
    }

    public class DeadState : EnemyState
    {
        public DeadState(EnemyEntity enemy) : base(enemy) { }

        public override void Enter()
        {
            // 进入死亡状态的逻辑
        }

        public override void Exit()
        {
            // 死亡状态不需要退出逻辑
        }

        public override void Update()
        {
            // 死亡状态不需要更新逻辑
        }
    }
} 
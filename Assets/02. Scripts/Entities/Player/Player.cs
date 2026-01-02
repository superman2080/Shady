using UnityEngine;

namespace PlayerNameSpace
{
    public enum PlayerStateEnum
    {
        HardCC = -1,
        Idle = 0,
        Move,
        Dash,
        Attack,
        Dead
    }

    public class BasicBehaviour : StateBase<Player>
    {
        public override void Enter(Player caster)
        {
        }

        public override void Execute(Player caster)
        {
        }

        public override void Exit(Player caster)
        {
        }
    }

    public class MoveBehaviour : StateBase<Player>
    {
        private Rigidbody2D rb;

        public override void Enter(Player caster)
        {
            rb = caster.GetComponent<Rigidbody2D>();
        }

        public override void FixedExecute(Player caster)
        {
            var inputVector = caster.Handler.MoveInput;
            float moveSpeed = caster.Stat.Get(DefaultStatType.MOVE_SPEED);
            rb.MovePosition(rb.position + (inputVector * moveSpeed * Time.fixedDeltaTime));
        }

        public override void Exit(Player caster)
        {
        }
    }

    public class DashBehaviour : StateBase<Player>
    {
        private Rigidbody2D rb;

        public override void Enter(Player caster)
        {
            rb = caster.GetComponent<Rigidbody2D>();
            caster.PlayerStat.dashStamina.disableRegen = true;
        }

        public override void FixedExecute(Player caster)
        {
            var inputVector = caster.Handler.MoveInput;
            float dashSpeed = caster.PlayerStat.Get(PlayerStatType.DASH_SPEED);
            rb.MovePosition(rb.position + (inputVector * dashSpeed * Time.fixedDeltaTime));
        }

        public override void Execute(Player caster)
        {
            float consumeAmount = caster.PlayerStat.Get(PlayerStatType.DASH_COST) * Time.deltaTime;
            caster.PlayerStat.dashStamina.Subtract(consumeAmount);
        }

        public override void Exit(Player caster)
        {
            caster.PlayerStat.dashStamina.disableRegen = false;
        }
    }

    public class AttackBehaviour : StateBase<Player>
    {
        public override void Enter(Player caster)
        {
        }

        public override void Execute(Player caster)
        {
        }

        public override void Exit(Player caster)
        {
        }
    }



    public class Player : Entity<Player>, ICameraLookable, IAttackable
    {
        private PlayerInputHandler handler;
        public PlayerInputHandler Handler => handler;

        #region Player Stat
        public PlayerStat PlayerStat { get; set; }
        #endregion

        #region Attack Attribute
        private WeaponCtrl weaponController;

        public WeaponCtrl WeaponController => weaponController;

        public LayerMask AttackLayer => 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Enemy");

        public event OnAttack OnAttackEvent;

        private AttackStat weaponStat;
        public AttackStat WeaponStat => weaponStat;
        #endregion

        protected override void Start()
        {
            base.Start();
            handler = gameObject.GetComponent<PlayerInputHandler>();
            RegisterStates();
            RegisterConditions();
            weaponController = new WeaponCtrl(this);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            PlayerStat.Update();
        }

        public void Attack(Entity caster, float amount)
        {
            if (weaponController.TryUsingWeapon() == false)
                return;

            OnAttackEvent?.Invoke(this, amount);
        }

        public void DisableCamera()
        {
        }

        public void EnableCamera()
        {
        }

        protected override void RegisterStates()
        {
            StateMachine.RegisterState(PlayerStateEnum.Idle, new BasicBehaviour());
            StateMachine.RegisterState(PlayerStateEnum.Move, new MoveBehaviour());
            StateMachine.RegisterState(PlayerStateEnum.Attack, new AttackBehaviour());
            StateMachine.RegisterState(PlayerStateEnum.Dash, new DashBehaviour());
            StateMachine.ChangeState(PlayerStateEnum.Idle);
        }

        protected override void RegisterConditions()
        {
            #region Idle ->
            StateMachine.RegisterCondition(PlayerStateEnum.Idle, PlayerStateEnum.Move, () => handler.MoveInput != Vector2.zero);
            StateMachine.RegisterCondition(PlayerStateEnum.Idle, PlayerStateEnum.Attack, () => handler.AttackPressed && weaponController.CanAttack == true);
            StateMachine.RegisterCondition(PlayerStateEnum.Idle, PlayerStateEnum.Dash, () => 
            handler.DashPressed && handler.MoveInput != Vector2.zero && PlayerStat.dashStamina > PlayerStat.Get(PlayerStatType.DASH_STAMINA) * 0.1f);
            #endregion

            #region Move ->
            StateMachine.RegisterCondition(PlayerStateEnum.Move, PlayerStateEnum.Idle, () => handler.MoveInput == Vector2.zero);
            StateMachine.RegisterCondition(PlayerStateEnum.Move, PlayerStateEnum.Dash, () => 
            handler.DashPressed && handler.MoveInput != Vector2.zero && PlayerStat.dashStamina > PlayerStat.Get(PlayerStatType.DASH_STAMINA) * 0.1f);
            StateMachine.RegisterCondition(PlayerStateEnum.Move, PlayerStateEnum.Attack, () => handler.AttackPressed && weaponController.CanAttack == true);
            #endregion

            #region Attack ->
            StateMachine.RegisterCondition(PlayerStateEnum.Attack, PlayerStateEnum.Move, () => handler.MoveInput != Vector2.zero && weaponController.CanAttack == true);
            StateMachine.RegisterCondition(PlayerStateEnum.Attack, PlayerStateEnum.Idle, () => handler.MoveInput == Vector2.zero == false && weaponController.CanAttack == true);
            #endregion

            #region Dash ->
            StateMachine.RegisterCondition(PlayerStateEnum.Dash, PlayerStateEnum.Move, () =>
                handler.DashPressed == false && handler.MoveInput != Vector2.zero ||
                PlayerStat.dashStamina.IsEmpty && handler.MoveInput != Vector2.zero);

            StateMachine.RegisterCondition(PlayerStateEnum.Dash, PlayerStateEnum.Idle, () =>
                handler.DashPressed == false && handler.MoveInput == Vector2.zero ||
                PlayerStat.dashStamina.IsEmpty && handler.MoveInput == Vector2.zero);
            #endregion
        }
    }
}
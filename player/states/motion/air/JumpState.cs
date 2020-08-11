using Godot;

namespace States
{
    public class JumpState : AirState
    {
        #region Properties

        /// <summary> Multiplicative factor for the height of the jump. </summary>
        [Export(PropertyHint.Range, "1,32,0.1")] 
        public float JumpHeightFactor = 12.0f;

        // /// <summ        ary>
        // /// A multiplicative factor for the horizontal distance covered by the jump.
        // /// </summary>
        // public float Speed = 6.0f;

        /// <summary> Whether the jump was initialized by the player pressing a key or was forced by something else. </summary>
        public bool WasInitializedByPlayer;

        /// <summary>
        /// A way to initialize jumps with zero height, one height, or any value in between.
        /// Stick to either 0 or 1 unless specifically increasing the jump height.
        /// </summary>
        private float jumpHeight;

        #endregion

        #region Methods

        public override void Initialize(Vector3 velocity, params float[] parameters)
        {
            if (parameters.Length > 0)
                this.jumpHeight = parameters[0];
                WasInitializedByPlayer = (parameters[0] > 0);
            base.Initialize(velocity, parameters);
        }

        public override void StateReady() 
        { 
            base.StateReady(); 

            WasInitializedByPlayer = false;
            DoesReplaceCurrentState = false;
        }

        public override void Enter() 
        { 
            base.updateInputAndMovementDirections();

            Vector3 _jumpDirection;
            float _jumpHeight;
            if (WasInitializedByPlayer)
            {
                _jumpDirection = MovementDirection * psm.MoveStateReference.Speed; //* Speed;
                _jumpHeight = jumpHeight;
            }
            else
            {
                _jumpDirection = Velocity;
                _jumpHeight = 0;
            }

            Velocity = new Vector3(
                _jumpDirection.x,
                _jumpHeight * JumpHeightFactor,
                _jumpDirection.z
            );

            base.Enter();
        }
        
        public override void Reenter() 
        {
            jumpHeight = 1.0f;
            Enter(); 
            base.Reenter(); 
        }

        public override void Exit() 
        { 
            WasInitializedByPlayer = false;
            base.Exit();
        }

        // public override int Update(float delta) { }
        // public override int HandleInput(InputEvent inputEvent) { }

        #endregion
    }
}
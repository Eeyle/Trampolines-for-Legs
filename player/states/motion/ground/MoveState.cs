using Godot;

namespace States
{
    public class MoveState : GroundState
    {
        #region Properties

        // /// <summary> The speed of the player when walking. </summary>
        // [Export(PropertyHint.Range, "1,10,0.1,or_greater")] 
        // public float WalkSpeed = 5.0f;

        /// <summary> The speed of the player when moving around. </summary>  
        [Export(PropertyHint.Range, "1,10,0.1")]
        public float Speed = 6.0f;

        /// <summary> How quickly the player increases their speed when starting from nothing. </summary>
        [Export(PropertyHint.Range, "1,32,0.1")] 
        public float PositiveAcceleration = 7.0f;

        /// <summary> How quickly the player decreases their speed after letting go. </summary>
        [Export(PropertyHint.Range, "1,32,0.1")]
        public float NegativeAcceleration = 10.0f;

        /// <summary>
        /// The current acceleration of the player, as determined by whether they are moving roughly in the same direction as their velocity.
        /// </summary>
        public float Acceleration
        {
            get
            {
                return (MovementDirection.Dot(Velocity) > 0) ? PositiveAcceleration : NegativeAcceleration;
            }
        }

        /// <summary> The velocity limit under which the player will stop moving completely. </summary>
        [Export(PropertyHint.Range, "0.1,1,0.01")] 
        public float WalkSpeedMinimum = 0.25f;

        /// <summary>
        /// A limit set on the player's input which stops the player if they stop inputting a direction of movement.
        /// This is called a speed because it is determined by the length of their input vector.
        /// </summary>
        [Export(PropertyHint.Range, "0.1,1,0.01")] 
        public float InputSpeedMinimum = 0.5f; // set in MoveStateReady();

        /// <summary> A snap variable for the MoveAndSlideWithSnap function. </summary>
        private Vector3 snapVector;

        #endregion

        #region Methods

        public override void StateReady() 
        { 
            base.StateReady(); 

            snapVector = new Vector3();
        }

        // public override void Enter() { }
        // public override void Reenter() { }
        // public override void Exit() { }

        public override void Update(float delta) 
        {  
            base.updateInputAndMovementDirections();

            // Only update movement if not jumping.
            if (psm.CurrentStateID != (int) PlayerStateMachine.PlayerStates.JUMP_STATE)
            {
                // Going idle.
                if (Velocity.Length() < WalkSpeedMinimum
                 && InputDirection.Length() < InputSpeedMinimum)
                {
                    EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.IDLE_STATE, Velocity, emptyParameterList);
                }

                // Snapping.
                snapVector = getSnapVector();

                MovementDirection = MovementDirection * Speed;
                Velocity = InterpolateVelocity(
                    Velocity,
                    MovementDirection,
                    Acceleration,
                    delta
                );

                // Keep the player on the floor.
                Velocity = new Vector3(
                    Velocity.x,
                    -10 * delta,
                    Velocity.z
                );

                // Move the player.
                player.MoveAndSlideWithSnap(
                    Velocity,
                    snapVector,
                    Vector3.Up
                );

                // Falling
                if (!player.IsOnFloor())
                {
                    EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.JUMP_STATE, Velocity, new float[] {0});
                }

                base.Update(delta);
            }
        }

        // public override void HandleInput(InputEvent inputEvent) { base.HandleInput(inputEvent); }

        private Vector3 getSnapVector()
        {
            return Input.IsActionJustPressed("move_jump") ? Vector3.Zero : Vector3.Up;
        }

        #endregion
    }
}
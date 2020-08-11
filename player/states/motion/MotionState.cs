using Godot;

namespace States
{
    public class MotionState : PlayerState
    {
        #region Properties

        /// <summary> The current moving velocity of the player. Used as a parameter in the MoveAndSlide function called by the current state. </summary>
        public Vector3 Velocity { get; set; }

        /// <summary> The direction that the player in inputting via controls. w -> (0, 1), d -> (1, 0) </summary>
        public Vector2 InputDirection { get; set; }

        /// <summary> The direction the player would move if they began walking forward at the moment.x </summary>
        public Vector3 MovementDirection { get; set; }

        #endregion

        #region Methods

        public override void Initialize(Vector3 velocity, params float[] parameters)
        {
            Velocity = velocity;
        }

        public override void StateReady()
        {
            Velocity = new Vector3();
            InputDirection = new Vector2();
            MovementDirection = new Vector3();

            base.StateReady();
        }

        // public override void Enter() { }
        // public override void Reenter() { }
        // public override void Exit() { }
        // public override void Update(float delta) { }
        public override void HandleInput(InputEvent inputEvent) 
        {
            // if (inputEvent.IsActionPressed("ui_end"))
            // {
            //     if (psm.CurrentStateID == (int) PlayerStateMachine.PlayerStates.NOCLIP_STATE)
            //         EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.IDLE_STATE, Velocity, emptyParameterList);
            //     else
            //         EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.NOCLIP_STATE, Velocity, emptyParameterList);
            // }

            base.HandleInput(inputEvent);
            
        }

        protected void updateInputAndMovementDirections()
        {
            InputDirection = getInputDirection();
            MovementDirection = getMovementDirection();
        }

        private Vector2 getInputDirection()
        {
            return new Vector2(
                Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
                Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
            );
        }

        private Vector3 getMovementDirection()
        {
            Vector3 movementDirection = new Vector3();
            Basis playerBasis = player.GlobalTransform.basis;
            if      (InputDirection.x > 0.25)  { movementDirection += playerBasis.x; }
            else if (InputDirection.x < -0.25) { movementDirection -= playerBasis.x; }
            if      (InputDirection.y > 0.25)  { movementDirection += playerBasis.z; }
            else if (InputDirection.y < -0.25) { movementDirection -= playerBasis.z; }
            return movementDirection.Normalized();
        }

        /// <summary>
        /// Takes the player's current velocity and intended movement direction and returns a new velocity that has been interpolated
        /// from the velocity towards the movement direction.
        /// </summary>
        /// <param name="velocity">The current velocity of the player.</param>
        /// <param name="movementDirection">The direction that the player intends to move in.</param>
        /// <param name="delta">The forward step in time.</param>
        /// <returns></returns>
        protected Vector3 InterpolateVelocity(Vector3 velocity, Vector3 movementDirection, float acceleration, float delta)
        {
            // Pseudocode in C#
            // Vector3 vel = velocity;
            // vel = vel.LinearInterpolate(movementDirection, acceleration * delta);
            // return vel;
            return velocity.LinearInterpolate(
                new Vector3(
                    movementDirection.x,
                    velocity.y,
                    movementDirection.z
                ), 
                acceleration * delta
            );
        }

        #endregion
    }
}
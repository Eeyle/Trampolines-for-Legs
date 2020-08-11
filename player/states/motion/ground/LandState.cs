using Godot;

namespace States
{
    public class LandState : MotionState
    {
        #region Properties

        /// <summary> Factor for how strongly to rebound the player after landing. </summary>
        [Export(PropertyHint.Range, "0.1,1,0.05")] 
        public float ReboundStrength = 0.5f;

        /// <summary> Record of whether or not the player is allowed to rebound at the time of landing. </summary>
        private bool canRebound;

        /// <summary>
        /// The minimum height after which the player will be allowed to rebound from the landing state.
        /// Likely exactly equal to AirState.LandingHeightMinimum.
        /// </summary>
        [Export(PropertyHint.Range, "1,5,0.1")]
        public float ReboundHeightMinimum = 3.0f;

        /// <summary> The amount of time to keep the player in the land state waiting for a new jump input before returning them to idle. </summary>
        [Export(PropertyHint.Range, "0.1,2,0.01")] 
        public float TimeDelayFactor = 0.04f;

        /// <summary> The true time delay depends on the height of the jump just made. </summary>
        private float timeDelay;

        /// <summary> Timer progressing through the state. </summary>
        private float currentTimeDelay;

        /// <summary> A reference to the head of the player. </summary>
        private Spatial head;

        /// <summary> Factor which increases the amplitude of vertical head bobble when landing in this state. </summary>
        [Export(PropertyHint.Range, "0.001,0.1,0.001")]
        public float HeadBobbleVerticalAmplitudeFactor = 0.01f;

        /// <summary> Actual head bobble vertical amplitude, as determined by the above factor and by the height difference of the jump just made. </summary>
        private float headBobbleVerticalAmplitude;

        /// <summary> Factor which increases the amplitude of horizontal head bobble when landing in this state. </summary>
        [Export(PropertyHint.Range, "0.0001,0.01,0.0001")]
        public float HeadBobbleHorizontalAmplitudeFactor = 0.001f;

        /// <summary> The player's slow movement speed when in this state. </summary>
        [Export(PropertyHint.Range, "1,10,0.1")]
        public float Speed = 2.0f;

        /// <summary> The player's acceleration when moving in this state. </summary>
        [Export(PropertyHint.Range, "1,5,0.01")]
        public float Acceleration = 0.3f;

        [Export(PropertyHint.Range, "1,32,0.1")]
        public float MaximumJumpHeightBeforeConstantLandTime = 12.0f;

        /// <summary> Actual head bobble horizontal amplitude, as determined by the above factor and the height difference of the jump just made. </summary>
        private float headBobbleHorizontalAmplitude;

        /// <summary> The maximum height of the jump that spawned this landing. </summary>
        private float maximumHeight;

        #endregion

        #region Methods

        public override void Initialize(Vector3 velocity, params float[] parameters)
        {
            if (parameters.Length > 0)
                this.maximumHeight = parameters[0];
            base.Initialize(velocity, parameters);
        }

        public override void StateReady() 
        { 
            base.StateReady(); 

            head = GetNode<Spatial>("../../Head");
        }

        public override void Enter() 
        { 
            float heightDifference = maximumHeight - player.Translation.y;
            
            timeDelay = getLandTime(heightDifference);
            currentTimeDelay = 0;

            headBobbleVerticalAmplitude = HeadBobbleVerticalAmplitudeFactor * heightDifference;
            headBobbleHorizontalAmplitude = HeadBobbleHorizontalAmplitudeFactor * heightDifference;

            canRebound = (heightDifference > ReboundHeightMinimum);

            Velocity = Velocity.Normalized();

            base.Enter(); 
        }
        
        public override void Reenter() 
        { 
            if ((maximumHeight - player.Translation.y) > ReboundHeightMinimum)
            {
                Enter();
            }
            else
            {
                EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.IDLE_STATE);
            }

            base.Reenter();
        }

        public override void Exit() 
        { 
            head.Translation = Vector3.Up;

            base.Exit(); 
        }
        
        public override void Update(float delta) 
        { 
            base.updateInputAndMovementDirections();

            // Delay.
            if (currentTimeDelay > timeDelay)
            {
                EmitSignal("change_state_signal",PlayerStateMachine.PlayerStates.IDLE_STATE, Vector3.Zero, emptyParameterList);
            }
            else
            {
                currentTimeDelay += delta;
            }

            // Head bobble.
            float currentAngle = Mathf.Pi / timeDelay * currentTimeDelay;
            Vector3 offset = new Vector3(
                Velocity.x * headBobbleHorizontalAmplitude * Mathf.Sin(currentAngle),
                -headBobbleVerticalAmplitude * Mathf.Sin(currentAngle),
                Velocity.z * headBobbleHorizontalAmplitude * Mathf.Sin(currentAngle)
            );
            head.Translation = (Vector3.Up + offset).Rotated(Vector3.Up, -player.Rotation.y);

            // Slow movement.
            MovementDirection = MovementDirection * Speed;
            Velocity = base.InterpolateVelocity(Velocity, MovementDirection, Acceleration, delta);
            player.MoveAndSlide(Velocity, Vector3.Up, true);

            base.Update(delta);
        }
        
        public override void HandleInput(InputEvent inputEvent) 
        { 
            if (canRebound && inputEvent.IsActionPressed("move_jump"))
            {
                canRebound = false;
                // psm.JumpStateReference.Initialize(
                //     Velocity,
                //     ReboundStrength * Mathf.Sqrt(maximumHeight - player.Translation.y)
                // );
                EmitSignal(
                    "change_state_signal", 
                    PlayerStateMachine.PlayerStates.JUMP_STATE, 
                    // Velocity, 
                    Vector3.Zero,
                    new float[] { ReboundStrength * Mathf.Sqrt(maximumHeight - player.Translation.y) }
                );
            }

            base.HandleInput(inputEvent);
        }

        private float getLandTime(float heightDifference)
        {
            return (heightDifference > MaximumJumpHeightBeforeConstantLandTime) ? 
                TimeDelayFactor * MaximumJumpHeightBeforeConstantLandTime : 
                TimeDelayFactor * heightDifference;
        }

        #endregion
    }
}
using Godot;

namespace States
{
    public class AirState : MotionState
    {
        #region Properties

        /// <summary> The strength of gravity on the player. </summary>
        [Export(PropertyHint.Range, "12,32,0.1")] 
        public float Gravity = 24.0f;

        /// <summary> The player's allowed controllable movement speed as they fly through the air, disregarding the y axis. </summary>
        [Export(PropertyHint.Range, "1,10,0.1")] 
        public float ControlSpeed = 5.0f;

        /// <summary> The player's clamped maximum movement speed as they fly through the air. </summary>
        [Export(PropertyHint.Range, "1,10,0.1")] 
        public float MaximumSpeed = 7.0f;

        /// <summary> The player's ability to accelerate around during flight. </summary>
        [Export(PropertyHint.Range, "1,10,0.1")] 
        public float Acceleration = 3.0f;

        /// <summary> The minimum height after which the player will enter the Land state upon hitting the ground. </summary>
        [Export(PropertyHint.Range, "1,5,0.1")] 
        public float LandingHeightMinimum = 3.0f;

        /// <summary> The minimum 2D distance traveled after which the player will be allowed to stick to a wall. </summary>
        [Export(PropertyHint.Range, "1,5,0.1")] 
        public float WallDistanceMinimum = 1.0f;

        /// <summary> The amount of time just after falling when the player is still allowed to jump. </summary>
        [Export(PropertyHint.Range, "0.05,0.5,0.01")]
        public float JumpLeewayTime = 0.2f;

        /// <summary> The minimum inner product needed before a collision is considered to be floor-like instead of wall-like. </summary>
        [Export(PropertyHint.Range, "0.01,0.5,0.01")]
        public float MinimumInnerProductFloor = 0.05f;

        /// <summary> The initial position at which the jump began. </summary>
        private Vector3 InitialPosition;

        /// <summary> The velocity of the player at the time they entered this state. </summary>
        private Vector3 InitialVelocity;

        /// <summary> The maximum height reached by the player during their jump. </summary>
        private float maximumHeight;

        /// <summary> Whether or not the player has wall jumped on this flight through the air. </summary>
        private bool hasWallJumped;

        /// <summary> The amount of time that the player has been in the air. </summary>
        private float airTime;

        #endregion

        #region Methods

        public override void StateReady() 
        { 
            base.StateReady(); 

            InitialPosition = new Vector3();
            hasWallJumped = false;
        }

        public override void Enter() 
        { 
            InitialPosition = player.Translation;
            InitialVelocity = Velocity;
            maximumHeight = InitialPosition.y;
            airTime = 0.0f;

            base.Enter(); 
        }

        // public override void Reenter() { }

        public override void Exit() 
        { 
            hasWallJumped = false;
            base.Exit(); 
        }

        public override void Update(float delta) 
        { 
            base.updateInputAndMovementDirections(); 

            // Update the maximum height.
            if (maximumHeight < player.Translation.y)
                maximumHeight = player.Translation.y;
            
            // Increment airtime.
            airTime += delta;

            // Acceleration in the air.
            MovementDirection = MovementDirection * ControlSpeed;
            Velocity = InterpolateVelocity(
                Velocity,
                MovementDirection,
                Acceleration,
                delta
            );

            // Clamp xz velocity.
            Vector2 velocity2D = getVelocity2D(Velocity);
            
            // Gravity.
            Velocity = new Vector3(
                velocity2D.x,
                Velocity.y - Gravity * delta,
                velocity2D.y
            );

            // Movement.
            KinematicCollision collision;
            collision = player.MoveAndCollide(Velocity * delta);

            // Collision detection.
            if (collision != null)
            {
                // The collision was with something floor-like.
                if (collision.Normal.Dot(Vector3.Up) > MinimumInnerProductFloor)
                {
                    EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.PREVIOUS_STATE, Velocity, emptyParameterList);

                    // Once landed, the player may then switch to the land state, depending on the height of the jump.
                    if (maximumHeight - player.Translation.y > LandingHeightMinimum)
                    {
                        // psm.LandStateReference.Initialize(Velocity, maximumHeight);
                        EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.LAND_STATE, Velocity, new float[] {maximumHeight});
                    }
                }

                // The collision was with something wall-like.
                else if (!hasWallJumped)
                {
                    if (InitialPosition.DistanceTo(player.Translation) > WallDistanceMinimum)
                    {
                        hasWallJumped = true;
                        EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.WALL_STATE, Velocity, emptyParameterList);
                    }
                }
            }

            base.Update(delta);
        }

        public override void HandleInput(InputEvent inputEvent) 
        {  
            if (inputEvent.IsActionPressed("move_jump"))
            {
                if (airTime < JumpLeewayTime && !hasWallJumped)
                {
                    if (!psm.JumpStateReference.WasInitializedByPlayer)
                    {
                        psm.JumpStateReference.Initialize(Velocity, 1);
                        psm.JumpStateReference.Enter();
                        psm.nCoyoteJumps++;
                        airTime = JumpLeewayTime;
                    }
                }
            }

            base.HandleInput(inputEvent);
        }

        /// <summary>
        /// Takes the player's velocity through 3d space and converts it into a 2d vector, clamping it to within MaximumSpeed.
        /// </summary>
        /// <param name="velocity">The Vector3 Velocity of the player.</param>
        /// <returns></returns>
        private Vector2 getVelocity2D(Vector3 velocity)
        {
            Vector2 velocity2D = new Vector2(velocity.x, velocity.z);
            if (velocity2D.Length() > MaximumSpeed)
                velocity2D = MaximumSpeed * velocity2D.Normalized();
            return velocity2D;
        }

        #endregion
    }
}
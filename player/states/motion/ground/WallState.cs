using Godot;

namespace States
{
    public class WallState : MotionState
    {
        #region Properties

        /// <summary> The amount of time to wait while stuck to the wall in this state. </summary>
        [Export(PropertyHint.Range, "0.1,2,0.1")] 
        public float TimeDelay = 0.5f;

        /// <summary> The current amount of time left waiting in this state before falling. </summary>
        private float timeDelay;

        #endregion

        #region Methods

        public override void StateReady() 
        {
            base.StateReady();

            DoesReplaceCurrentState = false;
        }

        public override void Enter() 
        { 
            timeDelay = TimeDelay;
            base.Enter(); 
        }
        
        // public override void Reenter() { }
        // public override void Exit() { }

        public override void Update(float delta) 
        { 
            base.updateInputAndMovementDirections(); 

            if (timeDelay < 0)
            {
                EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.PREVIOUS_STATE, Vector3.Zero, new float[] {0});
                psm.JumpStateReference.Initialize(Vector3.Zero, 0);
            }
            else
            {
                timeDelay -= delta;
            }

            base.Update(delta);
        }

        public override void HandleInput(InputEvent inputEvent) 
        { 
            if (inputEvent.IsActionPressed("move_jump"))
            {
                // psm.JumpStateReference.Initialize(Vector3.Zero, 0);
                EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.PREVIOUS_STATE, Vector3.Zero, new float[] {1});
            }

            base.HandleInput(inputEvent);
        }

        #endregion
    }
}
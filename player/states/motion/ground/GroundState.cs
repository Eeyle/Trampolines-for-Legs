using Godot;

namespace States
{
    public class GroundState : MotionState
    {
        #region Properties



        #endregion

        #region Methods

        public override void StateReady() 
        { 
            base.StateReady(); 
        }

        // public override void Enter() { base.Enter(); }
        // public override void Reenter() { base.Reenter(); }
        // public override void Exit() { base.Exit(); }
        // public override void Update(float delta) { base.Update(delta); }

        public override void HandleInput(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("move_jump"))
            {
                EmitSignal("change_state_signal", States.PlayerStateMachine.PlayerStates.JUMP_STATE, Velocity, new float[] {1.0f});
            }

            base.HandleInput(inputEvent);
        }

        #endregion
    }
}
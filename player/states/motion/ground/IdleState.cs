using Godot;

namespace States
{
    public class IdleState : GroundState
    {
        // public override void StateReady() { }

        public override void Enter() 
        { 
            Velocity = Vector3.Zero;

            base.Enter();
        }
        
        // public override int Reenter() { }
        // public override int Exit() { }

        public override void Update(float delta) 
        { 
            base.updateInputAndMovementDirections();

            // Keep the player on the floor - this updates IsOnFloor() which determines when the player should fall in GroundState.
            Velocity = new Vector3(0, -10 * delta, 0);
            player.MoveAndSlide(Velocity, Vector3.Up, true);

            if (!InputDirection.Equals(Vector2.Zero))
            {
                EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.MOVE_STATE, Velocity, emptyParameterList);
            }

            if (!player.IsOnFloor())
            {
                EmitSignal("change_state_signal", PlayerStateMachine.PlayerStates.JUMP_STATE, Velocity, new float[] {0});
            }

            base.Update(delta);
        }

        // public override void HandleInput(InputEvent inputEvent) { }
    }
}
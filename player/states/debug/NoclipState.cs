using Godot;

namespace States
{
    public class NoclipState : MotionState
    {
        public float HorizontalSpeed = 24.0f;
        public float VerticalSpeed = 24.0f;

        // public override void StateReady() { }

        public override void Enter() 
        { 
            psm.StateStack = new System.Collections.Generic.Stack<int>(psm.MaximumStateStackSize);
            psm.StateStack.Push((int) PlayerStateMachine.PlayerStates.NOCLIP_STATE);

            base.Enter(); 
        }
        
        // public override int Reenter() { }
        // public override int Exit() { }

        public override void Update(float delta) 
        { 
            base.updateInputAndMovementDirections();

            Velocity = MovementDirection * HorizontalSpeed;
            Velocity = new Vector3(
                Velocity.x,
                ((Input.IsActionPressed("move_jump") ? 1 : 0) - (Input.IsActionPressed("move_sprint") ? 1 : 0)) * VerticalSpeed,
                Velocity.z
            );

            player.Translation += Velocity * delta;

            base.Update(delta);
        }

        // public override void HandleInput(InputEvent inputEvent) { base.HandleInput(inputEvent); }
    }
}
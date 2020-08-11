using Godot;

namespace StateMachine
{
    public class State : Node
    {

        [Signal]
        public delegate void change_state_signal(int nextStateID);

        /// <summary> The ID of this state, matching its order in the node tree. </summary> 
        public int StateID;

        /// <summary>
        /// Whether or not this state replaces the current state when it is placed on the stack.
        /// For example, moving replaces idle when the player presses a movement key, whereas
        /// jumping does not replace the current state and instead gets placed on top of the stack.
        /// </summary>
        public bool DoesReplaceCurrentState = true;

        /// <summary> Called after the state machine has finished its _Ready() function. <summary> 
        public virtual void StateReady() { }

        /// <summary>  Called upon entering the state. </summary> 
        public virtual void Enter() { }

        /// <summary> Called upon leaving the state. </summary> 
        public virtual void Exit() { }

        /// <summary> Called when a state on the stack is removed and this state is reentered. </summary> 
        public virtual void Reenter() { }

        /// <summary> Called when the user makes an input while in this state. </summary> 
        public virtual void HandleInput(InputEvent inputEvent) { }

        /// <summary> Called every physics frame that the player is in this state. </summary> 
        public virtual void Update(float delta) { }
    }
}
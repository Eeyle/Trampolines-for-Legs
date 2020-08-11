using Godot;
using System.Collections;

namespace StateMachine 
{
    public class StateMachine : Node
    {
        #region Properties

        /// <summary> Signal emitted by states when they intend to change to a different state. </summary>
        /// <param name="nextStateID">The next state which the current state intends to change into.</param>
        [Signal]
        public delegate void state_changed_signal(int nextStateID);

        /// <summary> The current state ID. </summary>
        public int CurrentStateID;

        /// <summary> The current state. </summary>
        public State currentState;

        /// <summary> The stack of states, stored as ids since only the current state need to be used at any time, while the others remain inactive. <summary>
        public System.Collections.Generic.Stack<int> StateStack;

        /// <summary> The maximum number of states that the stack can hold. </summary>
        public int MaximumStateStackSize = 64;

        /// <summary> The initial state. </summary>
        private int initialStateID = 0;

        // Whether or not the machine is actively handling HandleInput, updating, and changing state.
        private bool active;
        public bool Active
        { 
            get => active;
            set 
            {
                active = value;
                SetPhysicsProcess(value);
                SetProcessInput(value);
                if (!value)
                {
                    StateStack = new System.Collections.Generic.Stack<int>(MaximumStateStackSize);
                }
            } 
        }

        #endregion

        #region Methods
        
        public override void _Ready()
        {
            // Set active.
            Active = true;
            
            // Set up state stack and references.
            CurrentStateID = initialStateID;
            currentState = GetChild(initialStateID) as State;
            StateStack = new System.Collections.Generic.Stack<int>(MaximumStateStackSize);
            StateStack.Push(initialStateID);

            // Connect signals between children and parent.
            for (int i = 0; i < GetChildCount(); i++)
            {
                Node child = GetChild(i);
                child.Connect("change_state_signal", this, nameof(ChangeState));
                Connect("ready", child, "StateReady");
            }

            currentState.Enter();
        }

        // Delegate HandleInput.
        public override void _Input(InputEvent inputEvent)
        {
            currentState.HandleInput(inputEvent);
        }

        // Delegate Update.
        public override void _PhysicsProcess(float delta)
        {
            currentState.Update(delta);
        }

        public void ChangeState(int stateID)
        {
            if (!Active) 
                return;
              
            bool previous = (stateID < 0);
            if (previous)
            {
                int currentID = StateStack.Pop();
                stateID = StateStack.Peek();
                StateStack.Push(currentID);
            }
            
            State nextState = GetChild(stateID) as State;
            
            // Remove the top state of the stack.
            if (previous || nextState.DoesReplaceCurrentState)
            {
                currentState.Exit();
                StateStack.Pop();
            }

            // Replace the top state of the stack if necessary.
            if (!previous)
                StateStack.Push(stateID);

            // Change to the next state.
            CurrentStateID = StateStack.Peek(); // either the stateID we just pushed or the previous id below it.
            currentState = nextState;

            // Reenter if returning to the previous state.
            if (previous)
                currentState.Reenter();
            else
                currentState.Enter();
        }

        #endregion
    }
}
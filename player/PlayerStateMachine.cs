using System;
using Godot;
using StateMachine;


namespace States
{
    public class PlayerStateMachine : StateMachine.StateMachine
    {
        new public delegate void state_changed_signal(int nextStateID, Vector3 velocity, float[] parameters);

        public enum PlayerStates
        {
            // Pass this state when intending to return to the previous state, instead of specifying what that previous state is.
            PREVIOUS_STATE = -1,

            // The Player's states
            IDLE_STATE = 0,
            MOVE_STATE = 1,
            JUMP_STATE = 2,
            LAND_STATE = 3,
            WALL_STATE = 4,

            // Debug states
            NOCLIP_STATE = 5
        }

        // The player state machine has references to every state. Since every state has a reference to its parent state machine, every state can access every other state.
        public IdleState IdleStateReference;
        public MoveState MoveStateReference;
        public JumpState JumpStateReference;
        public LandState LandStateReference;
        public WallState WallStateReference;
        public NoclipState NoclipStateReference;


        public int nCoyoteJumps;

        public override void _Ready()
        {
            base._Ready();

            IdleStateReference = GetNode<IdleState>("./IdleState");
            MoveStateReference = GetNode<MoveState>("./MoveState");
            JumpStateReference = GetNode<JumpState>("./JumpState");
            LandStateReference = GetNode<LandState>("./LandState");
            WallStateReference = GetNode<WallState>("./WallState");
            NoclipStateReference = GetNode<NoclipState>("./NoclipState");
        }

        public void ChangeState(int stateID, Vector3 Velocity, params float[] parameters)
        {
            initializeStateByID(stateID, Velocity, parameters);
            base.ChangeState(stateID);
        }

        private void initializeStateByID(int stateID, Vector3 Velocity, params float[] parameters)
        {
            switch((PlayerStates) stateID)
            {
                case PlayerStates.PREVIOUS_STATE:
                    int currentStateID = StateStack.Pop();
                    int nextStateID = StateStack.Peek();
                    StateStack.Push(currentStateID);
                    initializeStateByID(nextStateID, Velocity, parameters);
                    break;
                case PlayerStates.IDLE_STATE:
                    IdleStateReference.Initialize(Velocity, parameters);
                    break;
                case PlayerStates.MOVE_STATE:
                    MoveStateReference.Initialize(Velocity, parameters);
                    break;
                case PlayerStates.JUMP_STATE:
                    JumpStateReference.Initialize(Velocity, parameters);
                    break;
                case PlayerStates.LAND_STATE:
                    LandStateReference.Initialize(Velocity, parameters);
                    break;
                case PlayerStates.WALL_STATE:
                    WallStateReference.Initialize(Velocity, parameters);
                    break;
                case PlayerStates.NOCLIP_STATE:
                    NoclipStateReference.Initialize(Velocity, parameters);
                    break;
                default:
                    break;
            }
        }
    }
}
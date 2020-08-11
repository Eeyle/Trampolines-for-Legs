using Godot;
using StateMachine;

namespace States
{
    public class PlayerState : StateMachine.State
    {
        /// <summary> A reference to the player state machine, this node's parent. </summary>
        protected PlayerStateMachine psm;

        /// <summary> A reference to the player, the parent of this node's parent. </summary>
        protected KinematicBody player;

        protected float[] emptyParameterList = new float[0];

        public override void StateReady()
        {
            base.StateReady();

            psm = GetNode<PlayerStateMachine>("..");
            player = GetNode<KinematicBody>("../..");
        }

        public virtual void Initialize(Vector3 velocity, params float[] parameters) { }
    }
}
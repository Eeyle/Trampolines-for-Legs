using Godot;
using States;

public class TutorialController : Spatial
{

    public override void _Input(InputEvent inputEvent)
    {
        // if (inputEvent.IsActionPressed("ui_cancel"))
        // {
        //     // GD.Print(
        //     //     "Player made ", 
        //     //     GetNode<States.PlayerStateMachine>("./Player/PlayerStateMachine").nCoyoteJumps, 
        //     //     " coyote jumps this run."
        //     // );
        //     GetTree().Quit();
        // }

        // else if (inputEvent.IsActionPressed("ui_page_up"))
        //     Input.SetMouseMode(Input.MouseMode.Visible);
        // else if (inputEvent.IsActionPressed("ui_page_down"))
        //     Input.SetMouseMode(Input.MouseMode.Captured);
    }
}
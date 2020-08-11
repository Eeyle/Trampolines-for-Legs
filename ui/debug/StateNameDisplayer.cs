using Godot;
using States;

public class StateNameDisplayer : Panel
{
    private PlayerStateMachine psm;
    private KinematicBody player;

    private Label currentStateLabel;
    private Label stateStackLabel;
    private Label extraLabel1;
    private Label extraLabel2;

    public override void _Ready()
    {
        psm = GetNode<PlayerStateMachine>("../../Player/PlayerStateMachine");
        player = GetNode<KinematicBody>("../../Player");

        currentStateLabel = GetNode<Label>("CurrentState");
        stateStackLabel = GetNode<Label>("StateStack");
        extraLabel1 = GetNode<Label>("ExtraLabel1");
        extraLabel2 = GetNode<Label>("ExtraLabel2");
    }

    public override void _Process(float delta)
    {
        currentStateLabel.Text = psm.CurrentStateID.ToString();
        stateStackLabel.Text = getStateStackLabelText();
        extraLabel1.Text = "position\n" + getThreeLineStringFromVector(player.Translation); // player.Velocity.ToString();
        extraLabel2.Text = "velocity\n" + getThreeLineStringFromVector((psm.GetChild(psm.CurrentStateID) as MotionState).Velocity); // player.Translation.ToString();
    }

    private string getStateStackLabelText()
    {
        string s = "";
        foreach (int stateID in psm.StateStack)
        {
            s += stateID.ToString();
        }
        return s;
    }

    private string getThreeLineStringFromVector(Vector3 vector)
    {
        return "x " + vector.x.ToString("N3") + "\ny " + vector.y.ToString("N3") + "\nz " + vector.z.ToString("N3");
    }
}
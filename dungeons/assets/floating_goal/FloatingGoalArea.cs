using Godot;

public class FloatingGoalArea : Area
{
    private MeshInstance parentMeshInstance;

    public override void _Ready()
    {
        parentMeshInstance = GetNode<MeshInstance>("..");
        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    public void OnBodyEntered(KinematicBody kinematicBody)
    {
        parentMeshInstance.Visible = false;
    }
}

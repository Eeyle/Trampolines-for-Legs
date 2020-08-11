using Godot;

public class PlayerCamera : Camera
{
    [Export]
    public float MouseSensitivity = 0.1f;

    [Export]
    public float playerFOV = 80.0f;

    private Vector2 mouseAxis;
    private Vector2 mouseMovement;

    private KinematicBody player;
    private Spatial head;

    private const float PI_OVER_TWO = Mathf.Pi / 2;

    public override void _Ready()
    {
        player = GetNode<KinematicBody>("../..");
        head = GetNode<Spatial>("..");

        mouseAxis = new Vector2();
        mouseMovement = new Vector2();

        Fov = playerFOV;

        // HTML-5 specific way of capturing mouse. See the _Input function.
        // Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion inputEventMouseMotion)
        {
            mouseAxis = inputEventMouseMotion.Relative;
        }

        if (inputEvent is InputEventMouseButton inputEventMouseButton)
        {
            if (inputEventMouseButton.ButtonIndex == (int) Godot.ButtonList.Left)
            {
                if (Input.GetMouseMode() != Input.MouseMode.Captured)
                {
                    Input.SetMouseMode(Input.MouseMode.Captured);
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        if (Input.GetMouseMode() != Input.MouseMode.Captured) return;

        if (mouseAxis.Length() > 0)
        {
            mouseMovement = - MouseSensitivity * mouseAxis * delta;
            mouseAxis = new Vector2();

            player.RotateY(mouseMovement.x);
            head.RotateX(mouseMovement.y);

            head.Rotation = new Vector3(
                Mathf.Clamp(head.Rotation.x, -PI_OVER_TWO, PI_OVER_TWO),
                head.Rotation.y,
                head.Rotation.z
            );
        }
    }
}
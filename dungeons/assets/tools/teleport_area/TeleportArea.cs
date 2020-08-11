using Godot;
using System;

public class TeleportArea : Area
{
    private Spatial TeleportAreaTo;

    public override void _Ready()
    {
        TeleportAreaTo = GetNode<Spatial>("./TeleportAreaTo");
        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    private void OnBodyEntered(KinematicBody body)
    {
        body.Translation += TeleportAreaTo.Translation;
    }
}

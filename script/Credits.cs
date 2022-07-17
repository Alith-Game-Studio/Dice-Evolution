using Godot;
using System;

public class Credits : Node2D
{
    public void OnMetaClick(string url) {
        OS.ShellOpen(url);
    }
    void OnBackButtonPressed() {
        GetTree().ChangeScene("res://Title.tscn");
    }
}

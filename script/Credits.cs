using Godot;
using System;

public class Credits : Node2D
{
    public void OnMetaClick(string url) {
        OS.ShellOpen(url);
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
    void OnBackButtonPressed() {
        GetTree().ChangeScene("res://Title.tscn");
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
}

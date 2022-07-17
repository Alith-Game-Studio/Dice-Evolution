using Godot;
using System;

public class Title : Node2D {
    void OnButtonStartPressed() {
        GetTree().ChangeScene("res://Main.tscn");
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
    void OnButtonCreditsPressed() {
        GetTree().ChangeScene("res://Credits.tscn");
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
    void OnButtonExitPressed() {
        GetTree().Quit();
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
}

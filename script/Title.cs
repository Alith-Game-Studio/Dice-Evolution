using Godot;
using System;

public class Title : Node2D {
    void OnButtonStartPressed() {
        GameState.Reset();
        GetTree().ChangeScene("res://Main.tscn");
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
    void OnButtonCreditsPressed() {
        GetTree().ChangeScene("res://Credits.tscn");
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
    void OnButtonExitPressed() {
        if (OS.HasFeature("HTML5")) {
            GetNode<Button>("MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/ButtonExit").Text = "Close your browser to exit.";
        } else {
            GetTree().Quit();
        }
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
    }
}

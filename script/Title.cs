using Godot;
using System;

public class Title : Node2D {
    void OnButtonStartPressed() {
        GetTree().ChangeScene("res://Main.tscn");
    }
    void OnButtonCreditsPressed() {
        GetTree().ChangeScene("res://Credits.tscn");
    }
    void OnButtonExitPressed() {
        GetTree().Quit();
    }
}

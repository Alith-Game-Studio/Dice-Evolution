using Godot;
using System;
using System.Text;

public class LogPane : Node2D
{
    private StringBuilder sb;
    private RichTextLabel LogLabel;
    private string LastLog;
    private int Combo;
    public override void _Ready()
    {
        sb = new StringBuilder();
        LogLabel = GetNode<RichTextLabel>("LogLabel");
        LastLog = "";
        Combo = 0;
    }

//  public override void _Process(float delta)
//  {
//      
//  }

    public void Write(string msg, bool doFlush = false) {
        sb.Append(msg);
        if (doFlush) {
            Flush();
        }
    }

    public void Flush() {
        string newLog = sb.ToString();
        if (newLog == LastLog) {
            Combo ++;
        } else {
            Combo = 1;
        }
        LastLog = newLog;
        if (Combo >= 2) {
            newLog += $" x {Combo}";
        }
        LogLabel.Text = newLog;
    }
}

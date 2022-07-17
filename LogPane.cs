using Godot;
using System;
using System.Text;

public class LogPane : RichTextLabel
{
    private StringBuilder sb;
    private string LastLog;
    private int Combo;
    public override void _Ready()
    {
        sb = new StringBuilder();
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
        sb.Clear();
        if (newLog == LastLog) {
            Combo ++;
        } else {
            Combo = 1;
        }
        LastLog = newLog;
        if (Combo >= 2) {
            newLog += $" x {Combo}";
        }
        BbcodeText = newLog;
    }
}

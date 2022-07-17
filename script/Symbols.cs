using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
class Symbols {

    public static Dictionary<string, int> ValidSymbolList { get; private set; }
    static Symbols() {
        ValidSymbolList = new Dictionary<string, int>();
        Directory dir = new Directory();
        dir.Open("res://texture/");
        dir.ListDirBegin();

        while (true) {
            string file = dir.GetNext();
            if (file == "")
                break;
            else if (!file.StartsWith("."))
                ValidSymbolList[file.Split('.')[0]] = 1;
        }
    }
    public static string ImgBB(string name, float size = 24) {
        return $"[img={size}%]res://texture/{name}.png[/img]";
    }
    public static string CenterBB(string str) {
        return $"[center]{str}[/center]";
    }
    public static string DigitBB(int number, bool isGray = false) {
        string suffix = "";
        if (isGray) {
            suffix = "_gray";
        }
        return string.Join("", number.ToString().Select(c => ImgBB(c.ToString() + suffix, 12)));
    }
}

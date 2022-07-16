using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Symbols {
    public static string ImgBB(string name, float size = 24) {
        return $"[img={size}%]res://texture/{name}.png[/img]";
    }
    public static string CenterBB(string str) {
        return $"[center]{str}[/center]";
    }
    public static string DigitBB(int number) {
        return string.Join("", number.ToString().Select(c => ImgBB(c.ToString(), 12)));
    }
}

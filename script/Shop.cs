using Godot;
using System.Collections.Generic;

public class Shop {
    public static DiceFacet[] Items;
    static string[] ProcessString(string str) {
        if (str == "")
            return new string[] { };
        return str.Split('|');
    }
    static Shop() {
        File f = new File();
        f.Open("Shop.csv", File.ModeFlags.Read);
        string[] lines = f.GetAsText().Split('\n');
        List<DiceFacet> itemList = new List<DiceFacet>();
        foreach (string line in lines) {
            string content = line.Replace('\t', '?').StripEdges();
            if (content == "")
                continue;
            string[] tokens = content.Split('?');
            if (tokens[0] == "PLACE")
                continue;
            GD.Print(content);
            if (tokens[4] != "")
                itemList.Add(new DiceFacetCall(ProcessString(tokens[2]), tokens[4], ProcessString(tokens[1]), tokens[0]));
            else
                itemList.Add(new DiceFacetConvert(ProcessString(tokens[2]), ProcessString(tokens[3]), ProcessString(tokens[1]), tokens[0]));
        }
        Items = itemList.ToArray();
    }
}

using Godot;
using System.Collections.Generic;

public class Shop {
    public static DiceFacet[] Items;
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
            if (tokens[0] == "PLACE" || tokens[0].Contains("|"))
                continue;
            GD.Print(content);
            itemList.Add(GameState.CsvLineToFacet(line));
        }
        Items = itemList.ToArray();
    }
}

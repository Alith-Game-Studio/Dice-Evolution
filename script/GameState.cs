using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameState : Node
{
    public static Dictionary<string, int> Inventory { get; private set; }
    public static Dice[] Dices { get; private set; }
    public static int DiceIdToRoll { get; set; }
    public static int RoundNumber { get; set; }
    static string[] ProcessString(string str) {
        if (str == "")
            return new string[] { };
        return str.Split('|');
    }
    public static DiceFacet CsvLineToFacet(string line) {
        string content = line.Replace('\t', '?').StripEdges();
        string[] tokens = content.Split('?');
        GD.Print(content);
        if (tokens[4] != "")
            return new DiceFacetCall(ProcessString(tokens[2]), tokens[4], ProcessString(tokens[1]), tokens[0]);
        else
            return new DiceFacetConvert(ProcessString(tokens[2]), ProcessString(tokens[3]), ProcessString(tokens[1]), tokens[0]);

    }
    public static void Reset() {
        RoundNumber = 0;
        DiceIdToRoll = 0;
        File f = new File();
        f.Open("Shop.csv", File.ModeFlags.Read);
        string[] lines = f.GetAsText().Split('\n');
        List<DiceFacet> itemList = new List<DiceFacet>();
        foreach (string line in lines) {
            string content = line.Replace('\t', '?').StripEdges();
            if (content == "")
                continue;
            string[] tokens = content.Split('?');
            if (tokens[0].Contains("|")) {
                Dices = tokens[0].Split('|').Select(s => new Dice(s, new DiceFacet[6])).ToArray();
                Inventory = DiceFacet.ProcessCompressedStringList(tokens[3].Split('|'));
            }
        }
        foreach (string line in lines) {
            string content = line.Replace('\t', '?').StripEdges();
            if (content == "")
                continue;
            string[] tokens = content.Split('?');
            if (tokens[0] == "PLACE")
                continue;
            if (tokens[5] != "") {
                int diceId = -1;
                for (int i = 0; i < Dices.Length; ++i) {
                    if (Dices[i].Name == tokens[0]) {
                        diceId = i;
                        break;
                    }
                }
                IEnumerable<int> faces = tokens[5].Split('|').Select(s => int.Parse(s));
                DiceFacet facet = CsvLineToFacet(line);
                foreach (int face in faces) {
                    Dices[diceId].Facets[face] = facet;
                }
            }
        }
        Dices[0].IsRevealed = true;
    }
}

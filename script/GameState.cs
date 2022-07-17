using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{
    public static Dictionary<string, int> Inventory { get; private set; }
    public static Dice[] Dices { get; private set; }
    public static int DiceIdToRoll { get; set; }
    public static int RoundNumber { get; set; }
    static GameState() {
        RoundNumber = 0;
        Inventory = new Dictionary<string, int>() {
            {"hp", 12},
            {"mana", 5},
            {"fire", 3},
        };
        Dice diceFight = new Dice("fight", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
        });
        Dice dicePlay = new Dice("play", new DiceFacet[] {
            new DiceFacetConvert(new string[] { }, new string[] {"mana$1"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$3"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$1"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$3"}, null, "play"),
        });
        Dice diceWork = new Dice("work", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
        });
        Dices = new Dice[] {
            new Dice("root", new DiceFacet[] {
                new DiceFacetCall(new string[] { }, "work", null, "root"),
                new DiceFacetCall(new string[] { }, "play", null, "root"),
                new DiceFacetCall(new string[] { }, "fight", null, "root"),
                new DiceFacetCall(new string[] { }, "work", null, "root"),
                new DiceFacetCall(new string[] { }, "play", null, "root"),
                new DiceFacetCall(new string[] { }, "fight", null, "root"),
            }),
            diceWork,
            dicePlay,
            diceFight,
        };
    }
}

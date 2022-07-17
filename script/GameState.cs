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
        Inventory = new Dictionary<string, int>() {
            {"mana", 10},
            {"hp", 5 },
            {"fire", 2 },
        };
        Dice diceFight = new Dice("fight", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null, "fight"),
        });
        Dice dicePlay = new Dice("play", new DiceFacet[] {
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetCall(new string[] { }, "fight", null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"hp"}, null, "play"),
        });
        Dice diceWork = new Dice("work", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"air"}, null, "work"),
            new DiceFacetCall(new string[] {"mana"}, "fight", null, "work"),
        });
        Dices = new Dice[] {
            new Dice("root", new DiceFacet[] {
                new DiceFacetCall(new string[] { }, "work", null, "root"),
                new DiceFacetCall(new string[] { }, "play", null, "root"),
                new DiceFacetCall(new string[] { }, "work", null, "root"),
                new DiceFacetCall(new string[] { }, "play", null, "root"),
                new DiceFacetCall(new string[] { }, "work", null, "root"),
                new DiceFacetCall(new string[] { }, "play", null, "root"),
            }),
            diceWork,
            dicePlay,
            diceFight,
        };
    }
}

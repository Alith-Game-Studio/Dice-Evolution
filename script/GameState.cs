using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{
    public static Dictionary<string, int> Inventory { get; private set; }
    public static Dice[] Dices { get; private set; }
    public static int DiceIdToRoll { get; set; }
    public static int RoundNumber { get; set; }
    public static void Reset() {

        Inventory = new Dictionary<string, int>() {
            {"mana", 15},
            {"hp", 5 },
            {"fire", 2 },
        };
        Dice diceFight = new Dice("fight", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air"}, null, "fight"),
        });
        Dice dicePlay = new Dice("play", new DiceFacet[] {
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$2"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana$4"}, null, "play"),
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
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"air"}, null, "work"),
        });
        Dices = new Dice[] {
            new Dice("root", new DiceFacet[] {
                new DiceFacetCall(new string[] {"mana"}, "work", null, "root"),
                new DiceFacetCall(new string[] {"mana"}, "play", null, "root"),
                new DiceFacetCall(new string[] {"mana"}, "fight", null, "root"),
                new DiceFacetCall(new string[] {"mana"}, "work", null, "root"),
                new DiceFacetCall(new string[] {"mana"}, "play", null, "root"),
                new DiceFacetCall(new string[] {"mana"}, "fight", null, "root"),
            }),
            diceWork,
            dicePlay,
            diceFight,
        };
    }
}

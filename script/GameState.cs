using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{
    public static Dictionary<string, int> Inventory { get; private set; }
    public static Dice[] Dices { get; private set; }
    static GameState() {
        Dice diceFight = new Dice("fight", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null, "fight"),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null, "fight"),
        });
        Dice dicePlay = new Dice("play", new DiceFacet[] {
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null, "play"),
            new DiceFacetCall(new string[] { }, diceFight, "play"),
            new DiceFacetConvert(new string[] { }, new string[] {"hp"}, null, "play"),
        });
        Dice diceWork = new Dice("work", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null, "work"),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"air"}, null, "work"),
            new DiceFacetCall(new string[] {"mana"}, diceFight, "work"),
        });
        Dices = new Dice[] {
            new Dice("root", new DiceFacet[] {
                new DiceFacetCall(new string[] { }, diceWork, "root"),
                new DiceFacetCall(new string[] { }, diceWork, "root"),
                new DiceFacetCall(new string[] { }, diceWork, "root"),
                new DiceFacetCall(new string[] { }, dicePlay, "root"),
                new DiceFacetCall(new string[] { }, dicePlay, "root"),
                new DiceFacetCall(new string[] { }, dicePlay, "root"),
            }),
            diceWork,
            dicePlay,
            diceFight,
        };
    }
}

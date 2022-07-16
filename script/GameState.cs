using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{
    public static Dictionary<string, int> Inventory { get; private set; }
    public static Dice[] Dices { get; private set; }
    static GameState() {
        Dice diceFight = new Dice("fight", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"water"}, null),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null),
            new DiceFacetConvert(new string[] {"fire"}, new string[]{"air", "air"}, null),
        });
        Dice dicePlay = new Dice("play", new DiceFacet[] {
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null),
            new DiceFacetConvert(new string[] { }, new string[] {"mana", "mana"}, null),
            new DiceFacetCall(new string[] { }, diceFight),
            new DiceFacetConvert(new string[] { }, new string[] {"hp"}, null),
        });
        Dice diceWork = new Dice("work", new DiceFacet[] {
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"fire"}, null),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"earth"}, null),
            new DiceFacetConvert(new string[] {"mana"}, new string[]{"air"}, null),
            new DiceFacetCall(new string[] {"mana"}, diceFight),
        });
        Dices = new Dice[] {
            new Dice("root", new DiceFacet[] {
                new DiceFacetCall(new string[] { }, diceWork),
                new DiceFacetCall(new string[] { }, diceWork),
                new DiceFacetCall(new string[] { }, diceWork),
                new DiceFacetCall(new string[] { }, dicePlay),
                new DiceFacetCall(new string[] { }, dicePlay),
                new DiceFacetCall(new string[] { }, dicePlay),
            }),
            diceWork,
            dicePlay,
            diceFight,
        };
    }
}

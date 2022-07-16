using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Shop {
    static DiceFacet[] Items;
    static Shop() {
        Items = new DiceFacet[] {
            new DiceFacetConvert(
                new string[] {"fire", "fire"}, 
                new string[] {"copper"}, 
                new string[]{"water", "water", "fire"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"mana", "earth", "earth"}, 
                new string[] {"copper"}, 
                new string[]{"water", "water", "earth"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] { }, 
                new string[] {"mana$4"}, 
                new string[]{"water", "water", "air"}, 
                "play"
            ),
            new DiceFacetCall(
                new string[] { }, 
                "fight", 
                new string[]{"copper", "copper", "fire"}, 
                "root"
            ),
            new DiceFacetConvert(
                new string[] {"copper"}, 
                new string[] {"elixir"}, 
                new string[]{"copper", "copper", "water"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"elixir"}, 
                new string[] {"hp", "hp"}, 
                new string[]{"elixir", "elixir", "air"}, 
                "play"
            ),
            new DiceFacetConvert(
                new string[] {"mana", "mana", "copper", "copper"}, 
                new string[] {"mushroom"}, 
                new string[]{"copper", "copper", "earth"}, 
                "work"
            ),
            new DiceFacetCall(
                new string[] { }, 
                "work", 
                new string[]{"mushroom", "mushroom", "fire"}, 
                "root"
            ),
            new DiceFacetConvert(
                new string[] {"elixir"}, 
                new string[] {"skeleton"}, 
                new string[]{"elixir", "elixir", "fire"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"skeleton", "elixir"}, 
                new string[] {"key"}, 
                new string[]{"skeleton", "skeleton", "copper"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"mushroom", "mushroom", "copper", "copper"}, 
                new string[] {"key"}, 
                new string[]{"mushroom", "mushroom", "air"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"mana$4"}, 
                new string[] {"mushroom"}, 
                new string[]{"key", "key", "earth"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"air"}, 
                new string[] {"elixir"}, 
                new string[]{"key", "key", "fire"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] { }, 
                new string[] {"mana$8"}, 
                new string[]{"key", "key", "air"}, 
                "play"
            ),
            new DiceFacetConvert(
                new string[] {"mana$4", "elixir"}, 
                new string[] {"bomb"}, 
                new string[]{"mushroom", "mushroom", "skeleton", "skeleton"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"bomb"}, 
                new string[] {"key"}, 
                new string[]{"bomb", "bomb", "air"}, 
                "fight"
            ),
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Shop {
    public static DiceFacet[] Items;
    static Shop() {
        Items = new DiceFacet[] {
            new DiceFacetConvert(
                new string[] {"fire"}, 
                new string[] {"air"}, 
                new string[] {"fire"}, 
                "fight"
            ),
            new DiceFacetCall(
                new string[] { }, 
                "fight", 
                new string[] {"water"}, 
                "play"
            ),
            new DiceFacetConvert(
                new string[] {"mana", "water"}, 
                new string[] {"fire$2"}, 
                new string[] {"air"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"fire", "water", "air"}, 
                new string[] {"elixir"}, 
                new string[] {"air$2"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"elixir"}, 
                new string[] {"elixir$3"}, 
                new string[] {"air"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"elixir"}, 
                new string[] {"hp"}, 
                new string[] {"elixir$3"}, 
                "play"
            ),
            new DiceFacetConvert(
                new string[] {"mana"}, 
                new string[] {"earth"}, 
                new string[] {"fire"}, 
                "work"
            ),
            new DiceFacetCall(
                new string[] { }, 
                "work", 
                new string[] {"earth"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"mana", "earth$2"}, 
                new string[] {"copper"}, 
                new string[] {"earth"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"mana$2"}, 
                new string[] {"earth$3"}, 
                new string[] {"copper"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"mana", "earth$3"}, 
                new string[] {"copper$4"}, 
                new string[] {"earth$2", "copper"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"mana$2"}, 
                new string[] {"mushroom"}, 
                new string[] {"copper$9"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] {"mushroom", "elixir"}, 
                new string[] {"skeleton"}, 
                new string[] {"mana"}, 
                "work"
            ),
            new DiceFacetConvert(
                new string[] { }, 
                new string[] {"mana$6"}, 
                new string[] {"skeleton"}, 
                "play"
            ),
            new DiceFacetConvert(
                new string[] {"fire"}, 
                new string[] {"water"}, 
                new string[] {"mana"}, 
                "fight"
            ),
            new DiceFacetConvert(
                new string[] {"mana$4"}, 
                new string[] {"mushroom"}, 
                new string[] {"mushroom"}, 
                "work"
            ),
        };
    }
}

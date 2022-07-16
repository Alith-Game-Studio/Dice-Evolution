using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Shop {
    public static DiceFacet[] Items;
    static Shop() {
        Items = new DiceFacet[] {
            new DiceFacetConvert(new string[] {"fire", "fire"}, new string[] {"copper"}, new string[]{"water", "water", "fire"}, "fight"),
        };
    }
}

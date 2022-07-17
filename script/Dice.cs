using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Dice {
    public string Name { get; private set; }
    public DiceFacet[] Facets { get; private set; }

    public Dice(string name, DiceFacet[] facets) {
        Name = name;
        Facets = facets;
    }
    public string ToDescription() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Dice ");
        sb.Append(Symbols.ImgBB(Name, 28));
        return sb.ToString();
    }
}

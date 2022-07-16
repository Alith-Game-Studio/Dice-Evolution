using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Dice {
    public string Name { get; private set; }
    public List<DiceFacet> Facets { get; private set; }

    public Dice(string name, int sides) {
        Name = name;
        Facets = new List<DiceFacet>();

    }
}

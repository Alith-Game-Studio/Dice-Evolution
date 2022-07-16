using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DiceFacet {
    public string[] Ingradients;
    public int Type { get; set; }

    public abstract string ToDescription();
}
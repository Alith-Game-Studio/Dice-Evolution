using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class DiceFacetCall : DiceFacet {
    public Dice Dice;
    public DiceFacetCall(string[] ingradients, Dice dice, string type) {
        Ingradients = ingradients;
        Dice = dice;
        Type = type;
    }
    public override string ToDescription() {
        StringBuilder sb = new StringBuilder();
        if (Ingradients.Length > 0) {
            sb.Append(string.Join("", Ingradients.Select(s => Symbols.ImgBB(s))));
            sb.Append(Symbols.ImgBB("right_arrow"));
        }
        sb.Append(Symbols.ImgBB(Dice.Name));
        return sb.ToString();
    }
}

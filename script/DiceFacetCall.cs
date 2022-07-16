using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class DiceFacetCall : DiceFacet {
    public string Dice;
    public DiceFacetCall(string[] ingradients, string dice, string[] prices, string type) {
        Ingradients = ingradients;
        Dice = dice;
        Prices = prices;
        Type = type;
    }
    public override string ToDescription() {
        StringBuilder sb = new StringBuilder();
        if (Ingradients.Length > 0) {
            sb.Append(string.Join("", Ingradients.Select(s => Symbols.ImgBB(s))));
            sb.Append(Symbols.ImgBB("right_arrow"));
        }
        sb.Append(Symbols.ImgBB(Dice));
        return sb.ToString();
    }
}

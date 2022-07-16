using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class DiceFacetCall : DiceFacet {
    public string Dice;
    public DiceFacetCall(string[] ingredients, string dice, string[] prices, string type) {
        Ingredients = ProcessCompressedStringList(ingredients);
        Dice = dice;
        Prices = ProcessCompressedStringList(prices);
        Type = type;
    }
    public override string ToDescription() {
        return base.ToDescription() + "roll" + Symbols.ImgBB(Dice);
    }
}

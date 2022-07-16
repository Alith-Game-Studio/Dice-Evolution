using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DiceFacetConvert : DiceFacet {
    public string[] Products;

    public DiceFacetConvert(string[] ingradients, string[] products, string[] prices, string type) {
        Ingradients = ingradients;
        Products = products;
        Prices = prices;
        Type = type;
    }
    public override string ToDescription() {
        StringBuilder sb = new StringBuilder();
        if (Ingradients.Length > 0) {
            sb.Append(string.Join("", Ingradients.Select(s => Symbols.ImgBB(s))));
            sb.Append(Symbols.ImgBB("right_arrow"));
        }
        sb.Append(string.Join("", Products.Select(s => Symbols.ImgBB(s))));
        return sb.ToString();
    }
}
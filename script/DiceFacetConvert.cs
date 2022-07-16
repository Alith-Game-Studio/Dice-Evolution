using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DiceFacetConvert : DiceFacet {
    public Dictionary<string, int> Products;

    public DiceFacetConvert(string[] ingredients, string[] products, string[] prices, string type) {
        Ingredients = ProcessCompressedStringList(ingredients);
        Products = ProcessCompressedStringList(products);
        Prices = ProcessCompressedStringList(prices);
        Type = type;
    }
    public override string ToDescription() {
        return base.ToDescription() + DictToDescription(Products, false);
    }
}
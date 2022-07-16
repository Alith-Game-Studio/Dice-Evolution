using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DiceFacetConvert : DiceFacet {
    public Dictionary<string, int> Products;

    public DiceFacetConvert(string[] ingradients, string[] products, string[] prices, string type) {
        Ingradients = ProcessCompressedStringList(ingradients);
        Products = ProcessCompressedStringList(products);
        Prices = ProcessCompressedStringList(prices);
        Type = type;
    }
    public override string ToDescription() {
        return base.ToDescription() + DictToDescription(Products);
    }
}
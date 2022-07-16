using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DiceFacet {
    public string[] Ingradients;
    public string[] Prices;
    public string Type { get; set; }

    public abstract string ToDescription();
    protected string[] ProcessCompressedStringList(string[] list) {
        if (list == null)
            return list;
        List<string> result = new List<string>();
        foreach(string str in list) {
            if (str.Contains('$')) {
                string[] split = str.Split('$');
                int repeat = int.Parse(split[1]);
                for (int _ = 0; _ < repeat; ++_)
                    result.Add(split[0]);
            } else {
                result.Add(str);
            }
        }
        return result.ToArray();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DiceFacet {
    public Dictionary<string, int> Ingradients;
    public Dictionary<string, int> Prices;
    public string Type { get; set; }

    protected Dictionary<string, int> ProcessCompressedStringList(string[] list) {
        if (list == null)
            return null;
        Dictionary<string, int> result = new Dictionary<string, int>();
        foreach(string str in list) {
            string key = str;
            int count = 1;
            if (str.Contains('$')) {
                string[] split = str.Split('$');
                count = int.Parse(split[1]);
                key = split[0];
            }
            if (result.ContainsKey(key))
                result[key] += count;
            else
                result[key] = count;
        }
        return result;
    }
    public static string DictToDescription(Dictionary<string, int> itemDict) {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, int> kv in itemDict) {
            string imgBB = Symbols.ImgBB(kv.Key);
            if (kv.Value >= 3) {
                sb.Append(string.Join("", kv.Value.ToString().Select(c => Symbols.ImgBB(c.ToString(), 12))) + imgBB);
            } else {
                for (int _ = 0; _ < kv.Value; ++_)
                    sb.Append(imgBB);
            }
        }
        return sb.ToString();
    }
    public virtual string ToDescription() {
        StringBuilder sb = new StringBuilder();
        if (Ingradients.Count > 0) {
            sb.Append(DictToDescription(Ingradients));
            sb.Append(Symbols.ImgBB("right_arrow"));
        }
        return sb.ToString();
    }
}
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
    public static string DictToDescription(Dictionary<string, int> itemDict, bool isMissingGray) {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, int> kv in itemDict) {
            string symbolWhite = Symbols.ImgBB(kv.Key);
            string symbolGray  = Symbols.ImgBB(kv.Key + "_gray");
            int numIHave = 0;
            if (GameState.Inventory.ContainsKey(kv.Key)) {
                numIHave = GameState.Inventory[kv.Key];
            }
            if (kv.Value >= 3) {
                bool isGray = isMissingGray && numIHave < kv.Value;
                sb.Append(Symbols.DigitBB(kv.Value, isGray));
                if (isGray) {
                    sb.Append(symbolGray);
                } else {
                    sb.Append(symbolWhite);
                }
            } else {
                for (int i = 0; i < kv.Value; ++i)
                    if (isMissingGray && numIHave <= i) {
                        sb.Append(symbolGray);
                    } else {
                        sb.Append(symbolWhite);
                    }
            }
        }
        return sb.ToString();
    }
    public virtual string ToDescription() {
        StringBuilder sb = new StringBuilder();
        if (Ingradients.Count > 0) {
            sb.Append(DictToDescription(Ingradients, true));
            sb.Append(Symbols.ImgBB("right_arrow"));
        }
        return sb.ToString();
    }
}
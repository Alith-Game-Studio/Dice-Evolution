using Godot;
using System;
using System.Linq;

public class DiceLayoutWithPrice : HBoxContainer {

    public void Initialize(DiceFacet facet) {
        GetNode<RichTextLabel>("Button/MarginContainer/RichTextLabel").BbcodeText = facet.ToDescription();
        GetNode<RichTextLabel>("MarginContainer/PriceLabel").BbcodeText = string.Join("", facet.Prices.Select(s => Symbols.ImgBB(s)));

    }
}

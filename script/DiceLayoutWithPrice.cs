using Godot;
using System;
using System.Linq;

public class DiceLayoutWithPrice : HBoxContainer {

    public Button Initialize(DiceFacet facet) {
        GetNode<RichTextLabel>("Button/MarginContainer/RichTextLabel").BbcodeText = Symbols.CenterBB(facet.ToDescription());
        GetNode<RichTextLabel>("MarginContainer/PriceLabel").BbcodeText = Symbols.CenterBB(DiceFacet.DictToDescription(facet.Prices));
        return GetNode<Button>("Button");
    }
}

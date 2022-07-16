using Godot;
using System;
using System.Collections.Generic;

public class Gameplay : Node2D {
    List<HBoxContainer> DiceLayouts;
    List<RichTextLabel> DiceNameLabels;
    List<List<Button>> DiceButtons;
    List<List<RichTextLabel>> DiceButtonLabels;
    public override void _Ready() {
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        foreach (HBoxContainer container in GetNode<HBoxContainer>("HBoxContainer").GetChildren()) {
            DiceLayouts.Add(container);
            List<Button> buttons = new List<Button>();
            List<RichTextLabel> buttonLabels = new List<RichTextLabel>();
            DiceNameLabels.Add(container.GetNode<RichTextLabel>("VBoxContainer/DiceName"));
            DiceButtons.Add(buttons);
            DiceButtonLabels.Add(buttonLabels);
            foreach (Button button in container.GetNode<VBoxContainer>("VBoxContainer/VBoxContainer").GetChildren()) {
                buttons.Add(button);
                buttonLabels.Add(button.GetNode<RichTextLabel>("MarginContainer/RichTextLabel"));
            }
        }
        UpdateFromGameState();
    }
    public void UpdateFromGameState() {

        for (int i = 0; i < GameState.Dices.Length; ++i) {
            Dice dice =  GameState.Dices[i];
            DiceNameLabels[i].BbcodeText = Symbols.CenterBB(dice.ToDescription());
            for (int j = 0; j < dice.Facets.Length; ++j) {
                DiceFacet facet = dice.Facets[j];
                DiceButtonLabels[i][j].BbcodeText = Symbols.CenterBB(facet.ToDescription());
            }
        }
    }
}
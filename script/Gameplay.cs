using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Gameplay : Node2D {
    public List<HBoxContainer> DiceLayouts { get; set; }
    public List<RichTextLabel> DiceNameLabels { get; set; }
    public List<List<Button>> DiceButtons { get; set; }
    public List<List<RichTextLabel>> DiceButtonLabels { get; set; }
    public List<VBoxContainer> DiceUpgrades { get; set; }
    public RichTextLabel InventoryText { get; set; }
    public PackedScene DiceLayoutWithPricePrefab { get; set; }
    public override void _Ready() {
        DiceLayoutWithPricePrefab = GD.Load<PackedScene>("res://DiceLayoutWithPrice.tscn");
        InventoryText = GetNode<RichTextLabel>("InventoryText");
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        DiceUpgrades = new List<VBoxContainer>();
        foreach (HBoxContainer container in GetNode<GridContainer>("HBoxContainer").GetChildren()) {
            DiceLayouts.Add(container);
            List<Button> buttons = new List<Button>();
            List<RichTextLabel> buttonLabels = new List<RichTextLabel>();
            DiceNameLabels.Add(container.GetNode<RichTextLabel>("VBoxContainer/DiceName"));
            DiceButtons.Add(buttons);
            DiceUpgrades.Add(container.GetNode<VBoxContainer>("VBoxContainer2/PanelContainer/ScrollContainer/ItemList"));
            DiceButtonLabels.Add(buttonLabels);
            foreach (Button button in container.GetNode<VBoxContainer>("VBoxContainer/VBoxContainer").GetChildren()) {
                buttons.Add(button);
                buttonLabels.Add(button.GetNode<RichTextLabel>("MarginContainer/RichTextLabel"));
            }
        }
        UpdateFromGameState();
    }
    public void UpdateFromGameState() {
        InventoryText.BbcodeText = "Inventory\n\n" + string.Join("\n",
            GameState.Inventory.Select(kv => $"{Symbols.ImgBB(kv.Key)}: {kv.Value}"));
        for (int i = 0; i < GameState.Dices.Length; ++i) {
            Dice dice =  GameState.Dices[i];
            DiceNameLabels[i].BbcodeText = Symbols.CenterBB(dice.ToDescription());
            for (int j = 0; j < dice.Facets.Length; ++j) {
                DiceFacet facet = dice.Facets[j];
                DiceButtonLabels[i][j].BbcodeText = Symbols.CenterBB(facet.ToDescription());
            }
            foreach (Node node in DiceUpgrades[i].GetChildren()) {
                DiceUpgrades[i].RemoveChild(node);
            }
            foreach (DiceFacet facet in Shop.Items) {
                if (dice.Name == facet.Type) {
                    DiceLayoutWithPrice node = DiceLayoutWithPricePrefab.Instance() as DiceLayoutWithPrice;
                    DiceUpgrades[i].AddChild(node);
                    node.Initialize(facet);
                }
            }
        }

    }
}
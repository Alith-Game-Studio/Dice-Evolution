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

    int currentSelectedDiceId = -1, currentSelectedFaetId = -1;
    public override void _Ready() {
        DiceLayoutWithPricePrefab = GD.Load<PackedScene>("res://DiceLayoutWithPrice.tscn");
        InventoryText = GetNode<RichTextLabel>("InventoryText");
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        DiceUpgrades = new List<VBoxContainer>();
        int diceId = 0;
        foreach (HBoxContainer container in GetNode<GridContainer>("HBoxContainer").GetChildren()) {
            DiceLayouts.Add(container);
            List<Button> buttons = new List<Button>();
            List<RichTextLabel> buttonLabels = new List<RichTextLabel>();
            DiceNameLabels.Add(container.GetNode<RichTextLabel>("VBoxContainer/DiceName"));
            DiceButtons.Add(buttons);
            DiceUpgrades.Add(container.GetNode<VBoxContainer>("VBoxContainer2/PanelContainer/ScrollContainer/ItemList"));
            DiceButtonLabels.Add(buttonLabels);
            int facetId = 0;
            foreach (Button button in container.GetNode<VBoxContainer>("VBoxContainer/VBoxContainer").GetChildren()) {
                buttons.Add(button);
                button.Connect("pressed", this, "FacetButtonClick", new Godot.Collections.Array() { diceId, facetId });
                buttonLabels.Add(button.GetNode<RichTextLabel>("MarginContainer/RichTextLabel"));
                facetId += 1;
            }
            diceId += 1;
        }
        UpdateFromGameState();
        UpdateUpgradeVisibility();
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
            int itemId = 0;
            foreach (DiceFacet facet in Shop.Items) {
                if (dice.Name == facet.Type) {
                    DiceLayoutWithPrice node = DiceLayoutWithPricePrefab.Instance() as DiceLayoutWithPrice;
                    DiceUpgrades[i].AddChild(node);
                    Button button = node.Initialize(facet);
                    button.Connect("pressed", this, "UpdateButtonPressed", new Godot.Collections.Array() { itemId });
                    itemId += 1;
                }
            }
        }
    }
    void FacetButtonClick(int diceId, int facetId) {
        if (currentSelectedDiceId == diceId && currentSelectedFaetId == facetId) {
            currentSelectedDiceId = -1;
            currentSelectedFaetId = -1;
        } else {
            currentSelectedDiceId = diceId;
            currentSelectedFaetId = facetId;
        }
        UpdateUpgradeVisibility();
    }
    void UpdateButtonPressed(int itemId) {
        bool affordable = Shop.Items[itemId].Prices.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
        if (affordable) {
            foreach (KeyValuePair<string, int> req in Shop.Items[itemId].Prices) {
                GameState.Inventory[req.Key] -= req.Value;
            }
            GameState.Dices[currentSelectedDiceId].Facets[currentSelectedFaetId] = Shop.Items[itemId];
            currentSelectedDiceId = -1;
            currentSelectedFaetId = -1;
            UpdateFromGameState();
            UpdateUpgradeVisibility();
        }
    }
    void UpdateUpgradeVisibility() {
        for (int i = 0; i < GameState.Dices.Length; ++i) {
            if (i == currentSelectedDiceId) {
                DiceUpgrades[i].Visible = true;
            } else {
                DiceUpgrades[i].Visible = false;
            }
        }
    }

    private bool CanRollNow = true;
    private bool IsBlinking = false;
    private int BlinkStage;
    private int BlinkingDiceI;
    private int BlinkingFaceI;
    private float BlinkAge;
    private int BlinkOffset;
    private float BlinkDuration;
    const float BLINK_INIT_VELOCITY = 12;
    const float BLINK_DURATION_MEAN = 3;
    const float BLINK_SELF_INTERVAL = .3f;
    const float BLINK_SELF_DURATION = 1;
    Color BLINK_MODULATE = new Color(255, 120, 0);
    public override void _Process(float delta)
    {
        base._Process(delta);
        if (IsBlinking) {
            BlinkAge += delta;
            if (BlinkStage == 0) {
                BlinkingFaceI = (((int) Math.Round(
                    - 0.5 * BLINK_INIT_VELOCITY / BlinkDuration * Math.Pow(
                        BlinkAge - BlinkDuration, 2
                    )
                ) + BlinkOffset) % 6 + 6) % 6;
                for (int i = 0; i < 6; i ++) {
                    DiceButtons[BlinkingDiceI][i].Modulate = Colors.White;
                }
                DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                if (BlinkAge >= BlinkDuration) {
                    BlinkStage = 1;
                    BlinkAge = 0;
                }
            } else if (BlinkStage == 1) {
                if (BlinkAge < BLINK_SELF_DURATION) {
                    if (BlinkAge % BLINK_SELF_INTERVAL < BLINK_SELF_INTERVAL * .5) {
                        DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                    } else {
                        DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = Colors.White;
                    }
                } else {                    
                    IsBlinking = false;
                    onBlinkFinish(BlinkingDiceI, BlinkingFaceI);
                }
            }
        }
    }

    public void OnRollPressed() {
        if (CanRollNow) {
            CanRollNow = false;
            IsBlinking = true;
            BlinkStage = 0;
            BlinkingDiceI = 0;
            BlinkAge = 0;
            BlinkOffset = new Random().Next(0, 6);
            BlinkDuration = BLINK_DURATION_MEAN + (float) (
                (new Random().NextDouble() - .5) * .4
            );
        }
    }

    public void onBlinkFinish(int DiceI, int FaceI) {
    }
}

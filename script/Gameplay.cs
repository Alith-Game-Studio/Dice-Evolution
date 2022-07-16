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
    Random rng = new Random();
    public Button RollButton { get; set; }

    int currentSelectedDiceId = -1;
    int currentSelectedFacetId = -1;
    int currentSelectedShopItemId = -1;
    public override void _Ready() {
        DiceLayoutWithPricePrefab = GD.Load<PackedScene>("res://DiceLayoutWithPrice.tscn");
        InventoryText = GetNode<RichTextLabel>("InventoryText");
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        DiceUpgrades = new List<VBoxContainer>();
        RollButton = GetNode<Button>("Button");
        RollButton.Text = "Roll " + Symbols.ImgBB("root");
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
    private string ReprItemStack(KeyValuePair<string, int> kv) {
        if (kv.Value < 6) {
            return string.Concat(Enumerable.Repeat(
                Symbols.ImgBB(kv.Key), kv.Value
            ));
        } else {
            return $"{Symbols.DigitBB(kv.Value)}{Symbols.ImgBB(kv.Key)}";
        }
    }
    public void UpdateFromGameState() {
        InventoryText.BbcodeText = "Inventory\n\n" + string.Join("\n",
            GameState.Inventory.Where(kv => kv.Value > 0).Select(ReprItemStack));
        for (int diceId = 0; diceId < GameState.Dices.Length; ++diceId) {
            Dice dice =  GameState.Dices[diceId];
            DiceNameLabels[diceId].BbcodeText = Symbols.CenterBB(dice.ToDescription());
            for (int j = 0; j < dice.Facets.Length; ++j) {
                DiceFacet facet = dice.Facets[j];
                DiceButtonLabels[diceId][j].BbcodeText = Symbols.CenterBB(facet.ToDescription());
            }
            foreach (Node node in DiceUpgrades[diceId].GetChildren()) {
                DiceUpgrades[diceId].RemoveChild(node);
            }
            int itemId = 0;
            foreach (DiceFacet facet in Shop.Items) {
                if (dice.Name == facet.Type) {
                    bool hideForNow = false;
                    foreach (KeyValuePair<string, int> kv in facet.Prices) {
                        if (!GameState.Inventory.ContainsKey(kv.Key)) {
                            hideForNow = true;
                            break;
                        }
                    } 
                    if (hideForNow) {
                        continue;
                    }
                    DiceLayoutWithPrice node = DiceLayoutWithPricePrefab.Instance() as DiceLayoutWithPrice;
                    DiceUpgrades[diceId].AddChild(node);
                    Button button = node.Initialize(facet);
                    button.Connect("pressed", this, "UpgradeButtonPressed", new Godot.Collections.Array() { diceId, itemId });
                }
                itemId += 1;
            }
        }
    }
    void FacetButtonClick(int diceId, int facetId) {
        if (!CanOperateNow)
            return;
        if (currentSelectedDiceId == diceId) {
            if (currentSelectedFacetId == facetId) {
                currentSelectedFacetId = -1;
            } else {
                currentSelectedFacetId = facetId;
            }
        } else {
            currentSelectedDiceId = diceId;
            currentSelectedFacetId = facetId;
            currentSelectedShopItemId = -1;
        }
        UpdateUpgradeVisibility();
        TryUpgrade();
    }
    void UpgradeButtonPressed(int diceId, int itemId) {
        if (!CanOperateNow)
            return;
        if (currentSelectedDiceId == diceId) {
            if (currentSelectedShopItemId == itemId) {
                currentSelectedShopItemId = -1;
            } else {
                currentSelectedShopItemId = itemId;
            }
        } else {
            currentSelectedDiceId = diceId;
            currentSelectedShopItemId = itemId;
            currentSelectedFacetId = -1;
        }
        TryUpgrade();
    }
    void TryUpgrade() {
        if (!CanOperateNow)
            return;
        if (
            currentSelectedDiceId     == -1 ||
            currentSelectedFacetId    == -1 ||
            currentSelectedShopItemId == -1
        ) 
            return;
        DiceFacet facet = Shop.Items[currentSelectedShopItemId];
        bool affordable = facet.Prices.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
        if (affordable) {
            foreach (KeyValuePair<string, int> req in facet.Prices) {
                GameState.Inventory[req.Key] -= req.Value;
            }
            GameState.Dices[currentSelectedDiceId].Facets[currentSelectedFacetId] = facet;
            currentSelectedDiceId = -1;
            currentSelectedFacetId = -1;
            UpdateFromGameState();
            UpdateUpgradeVisibility();
        }
    }
    void UpdateUpgradeVisibility() {
        for (int i = 0; i < GameState.Dices.Length; ++i) {
            if (i == currentSelectedDiceId) {
                DiceUpgrades[i].Visible = true;
            } else {
                // DiceUpgrades[i].Visible = false;
            }
        }
    }

    private bool CanOperateNow = true;
    private bool IsBlinking = false;
    private int BlinkStage;
    private int BlinkingDiceI = -1;
    private int BlinkingFaceI = -1;
    private float BlinkAge;
    private int BlinkOffset;
    private float BlinkDuration;
    const float BLINK_INIT_VELOCITY = 18;
    const float BLINK_DURATION_MEAN = 1;
    const float BLINK_SELF_INTERVAL = .2f;
    const float BLINK_SELF_DURATION = .5f;
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
                    DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                    OnBlinkFinish(BlinkingDiceI, BlinkingFaceI);
                }
            }
        }
    }

    public void OnRollPressed() {
        if (CanOperateNow) {
            if (BlinkingDiceI >= 0) {
                DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = Colors.White;
                BlinkingDiceI = -1;
                BlinkingFaceI = -1;
            }
            CanOperateNow = false;
            RollButton.Visible = false;
            IsBlinking = true;
            BlinkStage = 0;
            BlinkingDiceI = GameState.DiceIdToRoll;
            BlinkAge = 0;
            BlinkOffset = rng.Next(0, 6);
            BlinkDuration = BLINK_DURATION_MEAN + (float) (
                (rng.NextDouble() - .5) * .4
            );
            currentSelectedDiceId = -1;
            currentSelectedFacetId = -1;
            UpdateUpgradeVisibility();
        }
    }

    public void OnBlinkFinish(int diceI, int faceI) {
        DiceFacet facet = GameState.Dices[diceI].Facets[faceI];
        bool affordable = facet.Ingredients.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
        if (affordable) {
            foreach (KeyValuePair<string, int> req in facet.Ingredients) {
                GameState.Inventory[req.Key] -= req.Value;
            }
            if (facet is DiceFacetCall facetCall) {
                for (int i = 0; i < GameState.Dices.Length; ++i) {
                    if (GameState.Dices[i].Name == facetCall.Dice) {
                        GameState.DiceIdToRoll = i;
                        break;
                    }
                }
            } else if (facet is DiceFacetConvert diceFacetConvert) {
                foreach (KeyValuePair<string, int> prod in diceFacetConvert.Products) {
                    if (!GameState.Inventory.ContainsKey(prod.Key))
                        GameState.Inventory[prod.Key] = prod.Value;
                    else
                        GameState.Inventory[prod.Key] += prod.Value;
                }
                GameState.DiceIdToRoll = 0;
            }
        } else {
            if (GameState.Dices[diceI].Name == "fight") {
                GameState.Inventory["hp"] -= 1;
            }
            GameState.DiceIdToRoll = 0;
        }
        currentSelectedDiceId = -1;
        currentSelectedFacetId = -1;
        UpdateFromGameState();
        UpdateUpgradeVisibility();
        CanOperateNow = true;
        RollButton.Visible = true;
    }
}

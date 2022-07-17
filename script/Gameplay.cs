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
    public RichTextLabel RollButtonText { get; set; }

    int currentSelectedDiceId = -1;
    int currentSelectedFacetId = -1;
    public override void _Ready() {
        DiceLayoutWithPricePrefab = GD.Load<PackedScene>("res://DiceLayoutWithPrice.tscn");
        InventoryText = GetNode<RichTextLabel>("InventoryText");
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        DiceUpgrades = new List<VBoxContainer>();
        RollButton = GetNode<Button>("Button");
        RollButton.Connect("pressed", this, "OnRollPressed");
        RollButtonText = RollButton.GetNode<RichTextLabel>("MarginContainer/RollTextBox");
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
    }
    private string ReprItemStack(KeyValuePair<string, int> kv) {
        if (kv.Value < 8) {
            return string.Concat(Enumerable.Repeat(
                Symbols.ImgBB(kv.Key), kv.Value
            ));
        } else {
            return $"{Symbols.DigitBB(kv.Value)}{Symbols.ImgBB(kv.Key)}";
        }
    }
    public void UpdateFromGameState() {
        RenderInventory();
        RollButtonText.BbcodeText = Symbols.CenterBB("Roll " + Symbols.ImgBB(GameState.Dices[GameState.DiceIdToRoll].Name));
        for (int diceId = 0; diceId < GameState.Dices.Length; ++diceId) {
            Dice dice =  GameState.Dices[diceId];
            DiceNameLabels[diceId].BbcodeText = Symbols.CenterBB(dice.ToDescription());
            for (int j = 0; j < dice.Facets.Length; ++j) {
                DiceFacet facet = dice.Facets[j];
                DiceButtonLabels[diceId][j].BbcodeText = Symbols.CenterBB(facet.ToDescription());
                if (diceId == currentSelectedDiceId && j == currentSelectedFacetId)
                    DiceButtons[diceId][j].GrabClickFocus();
                else
                    DiceButtons[diceId][j].ReleaseFocus();
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
                    if (!hideForNow) {
                        DiceLayoutWithPrice node = DiceLayoutWithPricePrefab.Instance() as DiceLayoutWithPrice;
                        DiceUpgrades[diceId].AddChild(node);
                        Button button = node.Initialize(facet);
                        button.Connect("pressed", this, "UpgradeButtonPressed", new Godot.Collections.Array() { diceId, itemId });
                        button.FocusMode = Control.FocusModeEnum.None;
                        if (currentSelectedDiceId != -1)
                            button.Disabled = !Affordable(facet);
                        else
                            button.Disabled = true;
                    }
                }
                itemId += 1;
            }
        }
    }
    void FacetButtonClick(int diceId, int facetId) {
        if (currentSelectedDiceId == diceId && currentSelectedFacetId == facetId) {
            currentSelectedFacetId = -1;
            currentSelectedDiceId = -1;
        } else {
            currentSelectedDiceId = diceId;
            currentSelectedFacetId = facetId;
        }
        UpdateFromGameState();
    }
    void UpgradeButtonPressed(int diceId, int itemId) {
        TryUpgrade(itemId);
    }
    bool Affordable(DiceFacet facet) {
        return facet.Type == GameState.Dices[currentSelectedDiceId].Name &&
            facet.Prices.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
    }
    void TryUpgrade(int itemId) {
        if (!CanOperateNow)
            return;
        // GD.Print("TryUpgrade");
        if (
            currentSelectedDiceId     == -1 ||
            currentSelectedFacetId    == -1
        ) 
            return;
        DiceFacet facet = Shop.Items[itemId];
        if (Affordable(facet)) {
            GameState.Dices[currentSelectedDiceId].Facets[currentSelectedFacetId] = facet;
            BlinkingDiceI = currentSelectedDiceId;
            BlinkingFaceI = currentSelectedFacetId;
            currentSelectedDiceId = -1;
            currentSelectedFacetId = -1;
            Transact(facet.Prices, new Dictionary<string, int> { });
        } else {
            GD.Print("cannot afford");
        }
        UpdateFromGameState();
    }

    private bool CanOperateNow = true;
    private bool IsFacetBlinking = false;
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
    Color BLINK_MODULATE = new Color(255, 200, 0);
    private void AnimateFacet(float delta) {
        if (IsFacetBlinking) {
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
                    IsFacetBlinking = false;
                    DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                    OnBlinkFinish(BlinkingDiceI, BlinkingFaceI);
                }
            }
        }
    }
    public override void _Process(float delta)
    {
        base._Process(delta);
        AnimateFacet(delta);
        AnimateInventory(delta);
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
            IsFacetBlinking = true;
            BlinkStage = 0;
            BlinkingDiceI = GameState.DiceIdToRoll;
            BlinkAge = 0;
            BlinkOffset = rng.Next(0, 6);
            BlinkDuration = BLINK_DURATION_MEAN + (float) (
                (rng.NextDouble() - .5) * .4
            );
        }
    }

    public void OnBlinkFinish(int diceI, int faceI) {
        DiceFacet facet = GameState.Dices[diceI].Facets[faceI];
        bool affordable = facet.Ingredients.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
        Dictionary<string, int> ToReceive = new Dictionary<string, int> { };
        if (affordable) {
            if (facet is DiceFacetCall facetCall) {
                for (int i = 0; i < GameState.Dices.Length; ++i) {
                    if (GameState.Dices[i].Name == facetCall.Dice) {
                        GameState.DiceIdToRoll = i;
                        break;
                    }
                }
            } else if (facet is DiceFacetConvert diceFacetConvert) {
                ToReceive = diceFacetConvert.Products;
                GameState.DiceIdToRoll = 0;
                GameState.RoundNumber ++;
            }
            Transact(facet.Ingredients, ToReceive);
        } else {
            if (GameState.Dices[diceI].Name == "fight") {
                Transact(new Dictionary<string, int> {
                    {"hp", 1}, 
                }, ToReceive);
            }
            GameState.DiceIdToRoll = 0;
        }
        UpdateFromGameState();
        CanOperateNow = true;
        RollButton.Visible = true;
    }

    private Dictionary<string, int> TransactSpending;
    private Dictionary<string, int> TransactReceiving;
    private bool IsInventoryBlinking = false;
    private float InventoryBlinkAge;
    const float BLINK_INVENTORY_INTERVAL = .2f;
    const float BLINK_INVENTORY_DURATION = .5f;
    public void Transact(Dictionary<string, int> spending, Dictionary<string, int> receiving) {
        foreach (KeyValuePair<string, int> req in spent) {
            GameState.Inventory[req.Key] -= req.Value;
        }
        foreach (KeyValuePair<string, int> req in got) {
            if (GameState.Inventory.ContainsKey(req.Key)) {
                GameState.Inventory[req.Key] += req.Value;
            } else {
                GameState.Inventory[req.Key] = req.Value;
            }
        }
        TransactSpending = spending;
        TransactReceiving = receiving;
        IsInventoryBlinking = true;
        InventoryBlinkAge = 0;
    }
    private void AnimateInventory(float delta) {
        if (IsInventoryBlinking) {
            InventoryBlinkAge += delta;
            if (InventoryBlinkAge < BLINK_INVENTORY_DURATION) {
                if (InventoryBlinkAge % BLINK_INVENTORY_INTERVAL < BLINK_INVENTORY_INTERVAL * .5) {
                    DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                } else {
                    DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = Colors.White;
                }
            } else {                    
                IsInventoryBlinking = false;
                RenderInventory();
            }
        }
    }

    public void RenderInventory(bool DoGray = false) {
        InventoryText.BbcodeText = "Inventory\n\n" + string.Join("\n",
        GameState.Inventory.Where(kv => kv.Value > 0).Select(ReprItemStack));    
    }
}

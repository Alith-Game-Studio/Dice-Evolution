using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public AudioStreamPlayer TickPlayer { get; set; }

    int currentSelectedItemId = -1;
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
        TickPlayer = GetNode<AudioStreamPlayer>("TickPlayer");
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
    public void UpdateFromGameState() {
        RenderInventory(0);
        RollButtonText.BbcodeText = Symbols.CenterBB("Roll " + Symbols.ImgBB(GameState.Dices[GameState.DiceIdToRoll].Name));
        Dictionary<string, int> possibleProducts = new Dictionary<string, int>();
        for (int diceId = 0; diceId < GameState.Dices.Length; ++diceId) {
            Dice dice = GameState.Dices[diceId];
            DiceNameLabels[diceId].BbcodeText = Symbols.CenterBB(dice.ToDescription());
            for (int j = 0; j < dice.Facets.Length; ++j) {
                DiceFacet facet = dice.Facets[j];
                DiceButtonLabels[diceId][j].BbcodeText = Symbols.CenterBB(facet.ToDescription());
                if (currentSelectedItemId >= 0 && Shop.Items[currentSelectedItemId].Type == dice.Name)
                    DiceButtons[diceId][j].Disabled = false;
                else
                    DiceButtons[diceId][j].Disabled = true;
                if (facet is DiceFacetConvert facetConvert) {
                    foreach (KeyValuePair<string, int> prod in facetConvert.Products)
                        possibleProducts[prod.Key] = 1;
                }
            }
        }
        for (int diceId = 0; diceId < GameState.Dices.Length; ++diceId) {
            Dice dice = GameState.Dices[diceId];
            foreach (Node node in DiceUpgrades[diceId].GetChildren()) {
                DiceUpgrades[diceId].RemoveChild(node);
            }
            int itemId = 0;
            foreach (DiceFacet facet in Shop.Items) {
                if (dice.Name == facet.Type) {
                    bool hideForNow = false;
                    foreach (KeyValuePair<string, int> kv in facet.Prices) {
                        if (!GameState.Inventory.ContainsKey(kv.Key) && !possibleProducts.ContainsKey(kv.Key)) {
                            hideForNow = true;
                            break;
                        }
                    }
                    if (!hideForNow) {
                        DiceLayoutWithPrice node = DiceLayoutWithPricePrefab.Instance() as DiceLayoutWithPrice;
                        DiceUpgrades[diceId].AddChild(node);
                        Button button = node.Initialize(facet);
                        button.Connect("pressed", this, "UpgradeButtonPressed", new Godot.Collections.Array() { diceId, itemId });
                        button.FocusMode = Control.FocusModeEnum.Click;
                        if (!Affordable(diceId, facet)) {
                            button.Disabled = true;
                            button.FocusMode = Control.FocusModeEnum.None;
                            if (currentSelectedItemId == itemId)
                                currentSelectedItemId = -1;
                        } else {
                            if (currentSelectedItemId == itemId)
                                button.GrabFocus();
                        }
                    }
                }
                itemId += 1;
            }
        }
    }
    void FacetButtonClick(int diceId, int facetId) {
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
        TryUpgrade(diceId, facetId);
    }
    void UpgradeButtonPressed(int diceId, int itemId) {
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
        if (itemId != currentSelectedItemId) {
            currentSelectedItemId = itemId;
        } else {
            currentSelectedItemId = -1;
        }
        UpdateFromGameState();
    }
    bool Affordable(int diceId, DiceFacet facet) {
        return facet.Type == GameState.Dices[diceId].Name &&
            facet.Prices.All(req => GameState.Inventory.ContainsKey(req.Key) && GameState.Inventory[req.Key] >= req.Value);
    }
    void TryUpgrade(int diceId, int facetId) {
        if (!CanOperateNow)
            return;
        // GD.Print("TryUpgrade");
        if (currentSelectedItemId == -1) 
            return;
        DiceFacet facet = Shop.Items[currentSelectedItemId];
        if (Affordable(diceId, facet)) {
            GameState.Dices[diceId].Facets[facetId] = facet;
            IsFacetBlinking = true;
            BlinkAge = 0;
            BlinkStage = 1;
            BlinkingDiceI = diceId;
            BlinkingFaceI = facetId;
            currentSelectedItemId = -1;
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
    const float BLINK_SELF_INTERVAL = .3f;
    const float BLINK_SELF_DURATION = 1f;
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
                if (DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate == Colors.White)
                    TickPlayer.Play();
                for (int i = 0; i < 6; i++) {
                    DiceButtons[BlinkingDiceI][i].Modulate = Colors.White;
                }
                DiceButtons[BlinkingDiceI][BlinkingFaceI].Modulate = BLINK_MODULATE;
                if (BlinkAge >= BlinkDuration) {
                    BlinkStage = 1;
                    BlinkAge = 0;
                    OnFacetDecided(BlinkingDiceI, BlinkingFaceI);
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
            GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
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
            for (int j = 0; j < GameState.Dices.Length; j ++) {
                for (int i = 0; i < 6; i ++) {
                    DiceButtons[j][i].Modulate = Colors.White;
                }
            }
        }
    }

    public void OnFacetDecided(int diceI, int faceI) {
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

    private Dictionary<string, int> TransactDelta;
    private bool IsInventoryBlinking = false;
    private float InventoryBlinkAge;
    const float BLINK_INVENTORY_INTERVAL = .3f;
    const float BLINK_INVENTORY_DURATION = 1f;
    public void Transact(Dictionary<string, int> spending, Dictionary<string, int> receiving) {
        TransactDelta = new Dictionary<string, int> { };
        foreach (KeyValuePair<string, int> req in spending) {
            if (!TransactDelta.ContainsKey(req.Key)) {
                TransactDelta[req.Key] = 0;
            }
            GameState.Inventory[req.Key] -= req.Value;
            TransactDelta      [req.Key] -= req.Value;
        }
        foreach (KeyValuePair<string, int> req in receiving) {
            if (! GameState.Inventory.ContainsKey(req.Key)) {
                GameState.Inventory[req.Key] = 0;
            }
            if (!TransactDelta.ContainsKey(req.Key)) {
                TransactDelta[req.Key] = 0;
            }
            GameState.Inventory[req.Key] += req.Value;
            TransactDelta      [req.Key] += req.Value;
        }
        IsInventoryBlinking = true;
        InventoryBlinkAge = 0;
    }
    private void AnimateInventory(float delta) {
        if (IsInventoryBlinking) {
            InventoryBlinkAge += delta;
            if (InventoryBlinkAge < BLINK_INVENTORY_DURATION) {
                if (InventoryBlinkAge % BLINK_INVENTORY_INTERVAL < BLINK_INVENTORY_INTERVAL * .5) {
                    RenderInventory(1);
                } else {
                    RenderInventory(2);
                }
            } else {                    
                IsInventoryBlinking = false;
                RenderInventory(0);
            }
        }
    }

    const int INVENTORY_LINE_WIDTH = 8;
    public void RenderInventory(int DeltaMode = 0) {
        // DeltaMode {
        //     0: Show current inv
        //     1: Show previous inv
        //     2: Show previous inv +- delta
        // }
        StringBuilder sb = new StringBuilder();
        sb.Append("Day ");
        sb.Append(Symbols.DigitBB(GameState.RoundNumber));
        sb.Append("\n===============\n");
        sb.Append("Inventory \n\n");
        foreach (KeyValuePair<string, int> invItem in GameState.Inventory) {
            if (DeltaMode == 0) {
                if (invItem.Value <= 0) {
                    continue;
                }
                if (invItem.Value < INVENTORY_LINE_WIDTH) {
                    for (int _ = 0; _ < invItem.Value; _ ++) {
                        sb.Append(Symbols.ImgBB(invItem.Key));
                    }
                } else {
                    sb.Append(Symbols.DigitBB(invItem.Value));
                    sb.Append(Symbols.ImgBB(invItem.Key));
                }
            } else {
                int deltaValue;
                if (TransactDelta.ContainsKey(invItem.Key)) {
                    deltaValue = TransactDelta[invItem.Key];
                } else {
                    deltaValue = 0;
                }
                int previousValue = invItem.Value - deltaValue;
                int maxValue = Math.Max(
                    invItem.Value, previousValue
                );
                if (maxValue <= 0) {
                    continue;
                }
                if (maxValue < INVENTORY_LINE_WIDTH) {
                    int n;
                    if (DeltaMode == 1) {
                        n = previousValue;
                    } else {
                        n = invItem.Value;
                    }
                    for (int _ = 0; _ < n; _ ++) {
                        sb.Append(Symbols.ImgBB(invItem.Key));
                    }
                } else {
                    sb.Append(Symbols.DigitBB(previousValue));
                    sb.Append(Symbols.ImgBB(invItem.Key));
                    if (DeltaMode == 2 && deltaValue != 0) {                    
                        sb.Append(" ");
                        sb.Append(Symbols.ImgBB(deltaValue > 0 ? "+" : "-", 12));
                        sb.Append(" ");
                        sb.Append(Symbols.DigitBB(Math.Abs(deltaValue)));
                        sb.Append(Symbols.ImgBB(invItem.Key));
                    }
                }
            }
            sb.Append("\n");
        }
        InventoryText.BbcodeText = sb.ToString();
    }
    void OnBackButtonPressed() {
        GetNode<AudioStreamPlayer>("/root/ClickPlayer").Play();
        GetTree().ChangeScene("res://Title.tscn");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventKey eventKey)
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Space) {
                OnRollPressed();
            }
    }
}

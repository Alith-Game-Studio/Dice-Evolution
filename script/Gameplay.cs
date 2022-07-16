using Godot;
using System;
using System.Collections.Generic;

public class Gameplay : Node2D {
    List<HBoxContainer> DiceLayouts;
    List<RichTextLabel> DiceNameLabels;
    List<ItemList> DiceUpgradeLists;
    List<List<Button>> DiceButtons;
    List<List<RichTextLabel>> DiceButtonLabels;
    public override void _Ready() {
        DiceLayouts = new List<HBoxContainer>();
        DiceNameLabels = new List<RichTextLabel>();
        DiceUpgradeLists = new List<ItemList>();
        DiceButtons = new List<List<Button>>();
        DiceButtonLabels = new List<List<RichTextLabel>>();
        foreach (HBoxContainer container in GetNode<HBoxContainer>("HBoxContainer").GetChildren()) {
            DiceLayouts.Add(container);
            List<Button> buttons = new List<Button>();
            List<RichTextLabel> buttonLabels = new List<RichTextLabel>();
            DiceNameLabels.Add(container.GetNode<RichTextLabel>("VBoxContainer/DiceName"));
            DiceUpgradeLists.Add(container.GetNode<ItemList>("VBoxContainer2/ItemList"));
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

using System.Linq;
using JunkStorm;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class JunkStormUnityController : MonoBehaviour
{
    [SerializeField] private Font uiFont;

    private JunkStormGame game;
    private const string DefaultInfoText = "Hover over a button for details.";

    private Transform boardRoot;
    private Transform controlRoot;
    private Transform logRoot;
    private Text infoBannerText;

    private void Awake()
    {
        EnsureEventSystem();
        game = new JunkStormGame();
        BuildCanvas();
        Render();
    }

    private void BuildCanvas()
    {
        var canvasObject = new GameObject("Junk Storm Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600, 900);

        var root = CreateBackgroundPanel(canvasObject.transform, "Root", new Color(0.08f, 0.06f, 0.05f));
        Stretch(root.GetComponent<RectTransform>());

        var title = CreateText(root.transform, "Title", "Junk Storm", 48, FontStyle.Bold, Color.white);
        Anchor(title.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(32, -74), new Vector2(-32, -16));

        var infoBanner = CreateBackgroundPanel(root.transform, "Info Banner", new Color(0.16f, 0.11f, 0.07f));
        Anchor(infoBanner.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(1, 1), new Vector2(32, -128), new Vector2(-32, -78));
        infoBannerText = CreateText(infoBanner.transform, "Info Text", DefaultInfoText, 20, FontStyle.Bold, new Color(1f, 0.81f, 0.46f));
        Stretch(infoBannerText.rectTransform, 16, -8, -16, 8);

        boardRoot = CreateScrollablePanel(root.transform, "Board", new Color(0.14f, 0.10f, 0.08f));
        Anchor((RectTransform)boardRoot.parent.parent, new Vector2(0, 0.28f), new Vector2(0.66f, 0.86f), new Vector2(32, 18), new Vector2(-14, -136));

        controlRoot = CreateScrollablePanel(root.transform, "Controls", new Color(0.12f, 0.09f, 0.08f));
        Anchor((RectTransform)controlRoot.parent.parent, new Vector2(0.66f, 0.28f), new Vector2(1, 0.86f), new Vector2(14, 18), new Vector2(-32, -136));

        logRoot = CreateScrollablePanel(root.transform, "Log", new Color(0.10f, 0.08f, 0.07f));
        Anchor((RectTransform)logRoot.parent.parent, new Vector2(0, 0), new Vector2(1, 0.28f), new Vector2(32, 18), new Vector2(-32, -16));
    }

    private void Render()
    {
        Clear(boardRoot);
        Clear(controlRoot);
        Clear(logRoot);
        RenderBoard();
        RenderControls();
        RenderLog();
    }

    private void RenderBoard()
    {
        CreateText(boardRoot, "Phase", $"Generation {game.Generation} — {game.CurrentPhase} — Active: {game.ActivePlayer.Name}", 28, FontStyle.Bold, new Color(1f, 0.81f, 0.46f));

        var outpostText = string.Join("\n", game.Outposts.Select((outpost, index) =>
        {
            var markers = string.Empty;
            if (game.JunkStormOutpost == index)
            {
                markers += "  🌪 Junk Storm";
            }

            if (game.BiodomerOutpost == index)
            {
                markers += "  🛡 Biodomers";
            }

            return $"{index + 1}. {outpost.Name} — {outpost.Deck.Count} cards{markers}";
        }));
        CreateText(boardRoot, "Outposts", outpostText, 22, FontStyle.Normal, Color.white);

        foreach (var player in game.Players)
        {
            var buildings = player.Buildings.Count == 0 ? "No buildings" : string.Join(", ", player.Buildings);
            var hand = player.Hand.Count == 0 ? "Empty" : string.Join(", ", player.Hand);
            CreateText(
                boardRoot,
                player.Name,
                $"{player.Name} ({player.Character})\nClout {player.Clout} | Workers {player.Workers} | Deck {player.Deck.Count} | Discard {player.Discard.Count}\nResources — Alloy {player.Resources.Alloy}, Organics {player.Resources.Organics}, Plastoid {player.Resources.Plastoid}, Labor {player.Resources.Labor}\nHand: {hand}\nBuildings: {buildings}",
                19,
                FontStyle.Normal,
                new Color(0.88f, 0.80f, 0.70f));
        }
    }

    private void RenderControls()
    {
        CreateButton(controlRoot, "New Game", () =>
        {
            game.NewGame();
            Render();
        }, true, "New Game — Restart the prototype from Generation 1 with freshly shuffled decks and reset player state.");

        if (game.CurrentPhase == Phase.Expedition)
        {
            foreach (var indexedOutpost in game.Outposts.Select((outpost, index) => new { outpost, index }))
            {
                CreateButton(controlRoot, $"Scavenge {indexedOutpost.outpost.Name}", () =>
                {
                    game.Scavenge(indexedOutpost.index);
                    Render();
                }, true, GetExpeditionTooltip(indexedOutpost.outpost.Name, indexedOutpost.index));
            }
        }
        else if (game.CurrentPhase == Phase.Action)
        {
            for (var index = 0; index < game.ActivePlayer.Hand.Count; index++)
            {
                var handIndex = index;
                var card = game.ActivePlayer.Hand[index];
                CreateButton(controlRoot, $"Recycle {card}", () =>
                {
                    game.PlayCard(handIndex, false);
                    Render();
                }, true, $"Recycle {card} — Use the card's recycle effect, then place it in your discard pile. {GetCardTooltip(card)}");
                CreateButton(controlRoot, $"Destroy {card}", () =>
                {
                    game.PlayCard(handIndex, true);
                    Render();
                }, true, $"Destroy {card} — Use the card's stronger destroy effect, then remove it from the game. {GetCardTooltip(card)}");
            }

            foreach (var building in game.Buildings)
            {
                var canBuild = game.CanBuild(building);
                CreateButton(controlRoot, $"Build {building.Name}", () =>
                {
                    game.Build(building.Name);
                    Render();
                }, canBuild, GetBuildingTooltip(building, canBuild));
            }

            CreateButton(controlRoot, "End Action", () =>
            {
                game.EndActionTurn();
                Render();
            }, true, "End Action — Finish this player's Action Phase turn and continue to the next player or the Junk Storm Phase.");
        }
        else if (game.CurrentPhase == Phase.Storm)
        {
            CreateButton(controlRoot, "Roll Junk Storm Die", () =>
            {
                game.ResolveStormRoll();
                Render();
            }, true, "Roll Junk Storm Die — Resolve the generation's hazard roll. 1–5 moves the Junk Storm; 6–10 moves the Biodomers.");
        }
        else if (game.CurrentPhase == Phase.Reset)
        {
            CreateButton(controlRoot, "Reset Generation", () =>
            {
                game.ResetGeneration();
                Render();
            }, true, "Reset Generation — Discard remaining hands, clear temporary resources, draw back up, check victory, and start the next generation.");
        }
        else
        {
            CreateText(controlRoot, "Winner", $"{game.Winner} wins!", 30, FontStyle.Bold, new Color(0.62f, 0.90f, 0.58f));
        }
    }

    private void RenderLog()
    {
        CreateText(logRoot, "LogTitle", "Event Log", 24, FontStyle.Bold, new Color(1f, 0.81f, 0.46f));
        CreateText(logRoot, "LogEntries", string.Join("\n", game.Log), 18, FontStyle.Normal, new Color(0.86f, 0.78f, 0.68f));
    }


    public void SetInfo(string message)
    {
        if (infoBannerText != null)
        {
            infoBannerText.text = string.IsNullOrWhiteSpace(message) ? DefaultInfoText : message;
        }
    }

    public void ClearInfo()
    {
        SetInfo(DefaultInfoText);
    }

    private string GetCardTooltip(string cardName)
    {
        if (!game.Cards.TryGetValue(cardName, out var card))
        {
            return cardName;
        }

        var notes = cardName switch
        {
            "Storm Shield" => "Timing: destroy from hand to cancel all Junk Storm effects against you this generation.",
            "Soldier" => "Timing: destroy from hand to cancel one Biodomer attack against you.",
            "Weapon Outfitting" => "Timing: destroy from hand to cancel one Biodomer attack against you.",
            "Preemptive Intelligence" => "Timing: recycle to cancel one Attack card targeting you.",
            _ => string.Empty
        };

        return $"{card.Name} — {card.Type}. Recycle: {DescribeEffect(card.Recycle)} Destroy: {DescribeEffect(card.Destroy)} {notes}".Trim();
    }

    private string GetBuildingTooltip(BuildingDefinition building, bool canBuild)
    {
        var requirement = string.IsNullOrWhiteSpace(building.Requires) ? "None" : building.Requires;
        var bonus = building.CloutReward > 0 ? $"Gain {building.CloutReward} Clout when built." : "Completes a Tier 3 victory project.";
        var expeditionBonus = GetBuildingBonusText(building.Name);
        var status = canBuild ? "Currently: can build." : $"Currently: cannot build — {GetBuildBlocker(building)}";
        return $"{building.Name} — Tier {building.Tier}. Cost: {DescribeCost(building.Cost)}. Requirement: {requirement}. {bonus} {expeditionBonus} {status}";
    }

    private string GetExpeditionTooltip(string outpostName, int outpostIndex)
    {
        if (outpostIndex == game.JunkStormOutpost)
        {
            return $"{outpostName} — Unavailable because the Junk Storm currently occupies this outpost.";
        }

        if (outpostIndex == game.BiodomerOutpost)
        {
            return $"{outpostName} — Unavailable because the Biodomers currently occupy this outpost.";
        }

        return $"Scavenge {outpostName} — Send {game.ActivePlayer.Name}'s available workers up to their Clout limit and draw that many cards from this outpost deck.";
    }

    private string GetBuildBlocker(BuildingDefinition building)
    {
        if (game.ActivePlayer.Buildings.Contains(building.Name))
        {
            return "already built";
        }

        if (building.MinClout > 0 && game.ActivePlayer.Clout < building.MinClout)
        {
            return $"requires {building.MinClout} Clout";
        }

        if (!string.IsNullOrWhiteSpace(building.Requires) && !game.ActivePlayer.Buildings.Contains(building.Requires))
        {
            return $"requires {building.Requires}";
        }

        if (building.Tier == 2 && game.ActivePlayer.Buildings.Count < 2)
        {
            return "requires two Tier 1 buildings";
        }

        return "cannot afford the resource/Labor cost";
    }

    private static string GetBuildingBonusText(string buildingName)
    {
        return buildingName switch
        {
            "Farm" => "Expedition Bonus: scavenged Organics may go on top of your deck.",
            "Military Base" => "Expedition Bonus: lose 1 fewer worker if Biodomers attack and you cannot defend.",
            "Laboratory" => "Expedition Bonus: destroy 1 fewer deck card if the Junk Storm affects you and you cannot defend.",
            "Transportation Station" => "Expedition Bonus: scavenged Alloy or Plastoid may go on top of your deck.",
            "Farming Center" => "Colony Bonus: each player gains 1 worker token. Expedition Bonus: scavenge 1 additional card per worker.",
            "Terraforming Station" => "Victory Meaning: terraform the planet and make Earth livable again.",
            _ => string.Empty
        };
    }

    private static string DescribeEffect(Effect effect)
    {
        var parts = new System.Collections.Generic.List<string>();
        if (effect.Clout != 0) parts.Add($"Gain {effect.Clout} Clout");
        if (effect.Workers != 0) parts.Add($"Gain {effect.Workers} worker(s)");
        if (effect.Alloy != 0) parts.Add($"Generate {effect.Alloy} Alloy");
        if (effect.Organics != 0) parts.Add($"Generate {effect.Organics} Organics");
        if (effect.Plastoid != 0) parts.Add($"Generate {effect.Plastoid} Plastoid");
        if (effect.Labor != 0) parts.Add($"Generate {effect.Labor} Labor this generation");
        if (effect.Defense != 0) parts.Add($"Gain {effect.Defense} Defense Strength this generation");
        if (effect.Draw != 0) parts.Add($"Draw {effect.Draw} card(s)");
        if (effect.TargetCloutLoss != 0) parts.Add($"Target loses {effect.TargetCloutLoss} Clout");
        if (effect.TargetWorkerLoss != 0) parts.Add($"Target loses {effect.TargetWorkerLoss} worker(s)");
        if (effect.StormShield) parts.Add("Cancel all Junk Storm effects against you this generation");
        if (effect.BiodomeShield) parts.Add("Cancel one Biodomer attack against you");
        return parts.Count == 0 ? "No effect." : string.Join(", ", parts) + ".";
    }

    private static string DescribeCost(ResourcePool cost)
    {
        var parts = new System.Collections.Generic.List<string>();
        if (cost.Organics > 0) parts.Add($"{cost.Organics} Organics");
        if (cost.Alloy > 0) parts.Add($"{cost.Alloy} Alloy");
        if (cost.Plastoid > 0) parts.Add($"{cost.Plastoid} Plastoid");
        if (cost.Labor > 0) parts.Add($"{cost.Labor} Labor");
        return parts.Count == 0 ? "Free" : string.Join(" + ", parts);
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystem);
    }

    private Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool interactable = true, string tooltipText = null)
    {
        var buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        buttonObject.GetComponent<Image>().color = interactable ? new Color(1f, 0.72f, 0.30f) : new Color(0.35f, 0.31f, 0.28f);

        var button = buttonObject.GetComponent<Button>();
        button.interactable = interactable;
        button.onClick.AddListener(onClick);

        var layout = buttonObject.GetComponent<LayoutElement>();
        layout.minHeight = 46;

        var text = CreateText(buttonObject.transform, "Label", label, 17, FontStyle.Bold, new Color(0.12f, 0.08f, 0.06f));
        Stretch(text.rectTransform, 8, -4, -8, 4);
        text.alignment = TextAnchor.MiddleCenter;
        AddTooltip(buttonObject, tooltipText ?? label);
        return button;
    }

    private void AddTooltip(GameObject target, string tooltipText)
    {
        var trigger = target.AddComponent<TooltipTrigger>();
        trigger.Initialize(this, tooltipText);
    }

    private Text CreateText(Transform parent, string name, string value, int size, FontStyle style, Color color)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        textObject.transform.SetParent(parent, false);
        var text = textObject.GetComponent<Text>();
        text.text = value;
        text.font = uiFont != null ? uiFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        var layout = textObject.GetComponent<LayoutElement>();
        layout.minHeight = Mathf.Max(36, value.Split('\n').Length * (size + 8));
        return text;
    }

    private Transform CreateScrollablePanel(Transform parent, string name, Color color)
    {
        var panel = CreateBackgroundPanel(parent, name, color);
        var scrollRect = panel.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 28f;

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
        viewport.transform.SetParent(panel.transform, false);
        viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
        Stretch(viewport.GetComponent<RectTransform>(), 18, -18, -18, 18);

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);

        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        var layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = contentRect;
        return content.transform;
    }

    private GameObject CreateBackgroundPanel(Transform parent, string name, Color color)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    private static void Clear(Transform root)
    {
        for (var i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }

    private static void Stretch(RectTransform rectTransform, float left = 0, float top = 0, float right = 0, float bottom = 0)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(right, top);
    }

    private static void Anchor(RectTransform rectTransform, Vector2 min, Vector2 max, Vector2 offsetMin, Vector2 offsetMax)
    {
        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }
}

public sealed class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private JunkStormUnityController owner;
    private string message;

    public void Initialize(JunkStormUnityController tooltipOwner, string tooltipMessage)
    {
        owner = tooltipOwner;
        message = tooltipMessage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        owner?.SetInfo(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        owner?.ClearInfo();
    }
}

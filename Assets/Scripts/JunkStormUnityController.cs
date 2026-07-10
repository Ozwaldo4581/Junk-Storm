using System.Linq;
using JunkStorm;
using UnityEngine;
using UnityEngine.UI;

public sealed class JunkStormUnityController : MonoBehaviour
{
    [SerializeField] private Font uiFont;

    private JunkStormGame game;
    private Transform boardRoot;
    private Transform controlRoot;
    private Transform logRoot;

    private void Awake()
    {
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

        var root = CreatePanel(canvasObject.transform, "Root", new Color(0.08f, 0.06f, 0.05f), TextAnchor.UpperLeft);
        Stretch(root.GetComponent<RectTransform>());

        var title = CreateText(root.transform, "Title", "Junk Storm", 54, FontStyle.Bold, Color.white);
        Anchor(title.rectTransform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(32, -96), new Vector2(-32, -16));

        boardRoot = CreatePanel(root.transform, "Board", new Color(0.14f, 0.10f, 0.08f), TextAnchor.UpperLeft).transform;
        Anchor((RectTransform)boardRoot, new Vector2(0, 0.25f), new Vector2(0.68f, 0.9f), new Vector2(32, 24), new Vector2(-16, -112));

        controlRoot = CreatePanel(root.transform, "Controls", new Color(0.12f, 0.09f, 0.08f), TextAnchor.UpperLeft).transform;
        Anchor((RectTransform)controlRoot, new Vector2(0.68f, 0.25f), new Vector2(1, 0.9f), new Vector2(16, 24), new Vector2(-32, -112));

        logRoot = CreatePanel(root.transform, "Log", new Color(0.10f, 0.08f, 0.07f), TextAnchor.UpperLeft).transform;
        Anchor((RectTransform)logRoot, new Vector2(0, 0), new Vector2(1, 0.25f), new Vector2(32, 24), new Vector2(-32, -16));
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
        });

        if (game.CurrentPhase == Phase.Expedition)
        {
            foreach (var indexedOutpost in game.Outposts.Select((outpost, index) => new { outpost, index }))
            {
                CreateButton(controlRoot, $"Scavenge {indexedOutpost.outpost.Name}", () =>
                {
                    game.Scavenge(indexedOutpost.index);
                    Render();
                });
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
                });
                CreateButton(controlRoot, $"Destroy {card}", () =>
                {
                    game.PlayCard(handIndex, true);
                    Render();
                });
            }

            foreach (var building in game.Buildings)
            {
                CreateButton(controlRoot, $"Build {building.Name}", () =>
                {
                    game.Build(building.Name);
                    Render();
                }, game.CanBuild(building));
            }

            CreateButton(controlRoot, "End Action", () =>
            {
                game.EndActionTurn();
                Render();
            });
        }
        else if (game.CurrentPhase == Phase.Storm)
        {
            CreateButton(controlRoot, "Roll Junk Storm Die", () =>
            {
                game.ResolveStormRoll();
                Render();
            });
        }
        else if (game.CurrentPhase == Phase.Reset)
        {
            CreateButton(controlRoot, "Reset Generation", () =>
            {
                game.ResetGeneration();
                Render();
            });
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

    private Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, bool interactable = true)
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
        Stretch(text.rectTransform, 8, 4, -8, -4);
        text.alignment = TextAnchor.MiddleCenter;
        return button;
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

    private GameObject CreatePanel(Transform parent, string name, Color color, TextAnchor childAlignment)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;

        var layout = panel.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 10;
        layout.childAlignment = childAlignment;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
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

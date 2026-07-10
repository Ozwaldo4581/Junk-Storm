using System;
using System.Collections.Generic;
using System.Linq;

namespace JunkStorm
{
    public enum Phase
    {
        Expedition,
        Action,
        Storm,
        Reset,
        GameOver
    }

    public enum CardType
    {
        Starter,
        Resource,
        Attack,
        Defense,
        Passive
    }

    [Serializable]
    public sealed class CardDefinition
    {
        public string Name;
        public CardType Type;
        public Effect Recycle = new();
        public Effect Destroy = new();

        public CardDefinition(string name, CardType type, Effect recycle, Effect destroy)
        {
            Name = name;
            Type = type;
            Recycle = recycle;
            Destroy = destroy;
        }
    }

    [Serializable]
    public sealed class Effect
    {
        public int Clout;
        public int Workers;
        public int Alloy;
        public int Organics;
        public int Plastoid;
        public int Labor;
        public int Defense;
        public int Draw;
        public int TargetCloutLoss;
        public int TargetWorkerLoss;
        public bool StormShield;
        public bool BiodomeShield;
    }

    [Serializable]
    public sealed class BuildingDefinition
    {
        public string Name;
        public int Tier;
        public string Requires;
        public int MinClout;
        public ResourcePool Cost = new();
        public int CloutReward;

        public BuildingDefinition(string name, int tier, ResourcePool cost, int cloutReward = 0, string requires = null, int minClout = 0)
        {
            Name = name;
            Tier = tier;
            Cost = cost;
            CloutReward = cloutReward;
            Requires = requires;
            MinClout = minClout;
        }
    }

    [Serializable]
    public sealed class ResourcePool
    {
        public int Alloy;
        public int Organics;
        public int Plastoid;
        public int FlexibleAlloyPlastoid;
        public int Labor;
        public int Defense;

        public bool CanPay(ResourcePool cost)
        {
            var remainingAlloy = Alloy - cost.Alloy;
            var remainingPlastoid = Plastoid - cost.Plastoid;
            return Organics >= cost.Organics && Labor >= cost.Labor && remainingAlloy >= 0 && remainingPlastoid >= 0 && remainingAlloy + remainingPlastoid >= cost.FlexibleAlloyPlastoid;
        }

        public string Pay(ResourcePool cost)
        {
            Alloy -= cost.Alloy;
            Organics -= cost.Organics;
            Plastoid -= cost.Plastoid;
            Labor -= cost.Labor;

            var flexiblePaidWithAlloy = Math.Min(Alloy, cost.FlexibleAlloyPlastoid);
            Alloy -= flexiblePaidWithAlloy;
            var flexiblePaidWithPlastoid = cost.FlexibleAlloyPlastoid - flexiblePaidWithAlloy;
            Plastoid -= flexiblePaidWithPlastoid;

            if (cost.FlexibleAlloyPlastoid <= 0)
            {
                return string.Empty;
            }

            return $" Paid flexible material cost with {flexiblePaidWithAlloy} Alloy and {flexiblePaidWithPlastoid} Plastoid.";
        }

        public void ClearTemporary()
        {
            Labor = 0;
            Defense = 0;
        }
    }

    [Serializable]
    public sealed class OutpostState
    {
        public string Name;
        public List<string> Deck = new();

        public OutpostState(string name, IEnumerable<string> deck)
        {
            Name = name;
            Deck = deck.ToList();
        }
    }

    [Serializable]
    public sealed class PlayerState
    {
        public string Name;
        public string Character;
        public int Clout = 1;
        public int Workers = 3;
        public int ExpeditionOutpost = -1;
        public int ExpeditionWorkers;
        public bool StormShield;
        public bool BiodomeShield;
        public List<string> Deck = new();
        public List<string> Hand = new();
        public List<string> Discard = new();
        public List<string> Destroyed = new();
        public List<string> Buildings = new();
        public ResourcePool Resources = new();
    }

    public sealed class JunkStormGame
    {
        private static readonly string[] StarterDeck = { "Dweller", "Worker", "Soldier", "Storm Shield", "Alloy", "Organics", "Plastoid" };
        private static readonly string[] Characters = { "Albus", "Luvana" };
        private readonly Random random;

        public int Generation { get; private set; } = 1;
        public Phase CurrentPhase { get; private set; } = Phase.Expedition;
        public int ActivePlayerIndex { get; private set; }
        public int JunkStormOutpost { get; private set; }
        public int BiodomerOutpost { get; private set; } = 3;
        public string Winner { get; private set; }
        public List<PlayerState> Players { get; } = new();
        public List<OutpostState> Outposts { get; } = new();
        public List<string> Log { get; } = new();

        public IReadOnlyDictionary<string, CardDefinition> Cards => cardLibrary;
        public IReadOnlyList<BuildingDefinition> Buildings => buildings;

        private readonly Dictionary<string, CardDefinition> cardLibrary = CreateCards();
        private readonly List<BuildingDefinition> buildings = CreateBuildings();

        public JunkStormGame(int seed = 0)
        {
            random = seed == 0 ? new Random() : new Random(seed);
            NewGame();
        }

        public PlayerState ActivePlayer => Players[ActivePlayerIndex];

        public void NewGame()
        {
            Generation = 1;
            CurrentPhase = Phase.Expedition;
            ActivePlayerIndex = 0;
            JunkStormOutpost = 0;
            BiodomerOutpost = 3;
            Winner = null;
            Players.Clear();
            Outposts.Clear();
            Log.Clear();

            CreateOutposts().ForEach(Outposts.Add);
            for (var index = 0; index < 2; index++)
            {
                var player = CreatePlayer(index);
                Draw(player, 5);
                Players.Add(player);
            }

            AddLog("Generation 1 begins. Send each player to an outpost.");
        }

        public bool Scavenge(int outpostIndex)
        {
            if (CurrentPhase != Phase.Expedition || outpostIndex < 0 || outpostIndex >= Outposts.Count)
            {
                return false;
            }

            if (outpostIndex == JunkStormOutpost || outpostIndex == BiodomerOutpost)
            {
                AddLog($"{ActivePlayer.Name} cannot visit {Outposts[outpostIndex].Name}; it is occupied.");
                return false;
            }

            var player = ActivePlayer;
            var workers = Math.Min(player.Workers, player.Clout);
            var gained = new List<string>();
            player.ExpeditionOutpost = outpostIndex;
            player.ExpeditionWorkers = workers;

            for (var i = 0; i < workers; i++)
            {
                var card = DrawOutpostCard(outpostIndex);
                if (card == null)
                {
                    continue;
                }

                player.Discard.Add(card);
                gained.Add(card);
            }

            AddLog($"{player.Name} scavenged {Outposts[outpostIndex].Name} with {workers} worker(s): {(gained.Count == 0 ? "nothing" : string.Join(", ", gained))}.");
            AdvanceTurnOrPhase();
            return true;
        }

        public bool PlayCard(int handIndex, bool destroy)
        {
            if (CurrentPhase != Phase.Action || handIndex < 0 || handIndex >= ActivePlayer.Hand.Count)
            {
                return false;
            }

            var player = ActivePlayer;
            var cardName = player.Hand[handIndex];
            player.Hand.RemoveAt(handIndex);

            var card = cardLibrary[cardName];
            ApplyEffect(player, destroy ? card.Destroy : card.Recycle);
            if (destroy)
            {
                player.Destroyed.Add(cardName);
            }
            else
            {
                player.Discard.Add(cardName);
            }

            AddLog($"{player.Name} {(destroy ? "destroyed" : "recycled")} {cardName}.");
            return true;
        }

        public bool CanBuild(BuildingDefinition building, PlayerState player = null)
        {
            player ??= ActivePlayer;
            if (player.Buildings.Contains(building.Name) || !player.Resources.CanPay(building.Cost))
            {
                return false;
            }

            if (building.MinClout > 0 && player.Clout < building.MinClout)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(building.Requires) && !player.Buildings.Contains(building.Requires))
            {
                return false;
            }

            if (building.Tier == 2 && player.Buildings.Count(name => buildings.Any(buildingDefinition => buildingDefinition.Name == name && buildingDefinition.Tier == 1)) < 2)
            {
                return false;
            }

            return true;
        }

        public bool Build(string buildingName)
        {
            if (CurrentPhase != Phase.Action)
            {
                return false;
            }

            var building = buildings.FirstOrDefault(candidate => candidate.Name == buildingName);
            if (building == null || !CanBuild(building))
            {
                return false;
            }

            var paymentLog = ActivePlayer.Resources.Pay(building.Cost);
            ActivePlayer.Buildings.Add(building.Name);
            ActivePlayer.Clout += building.CloutReward;
            AddLog($"{ActivePlayer.Name} built {building.Name}.{paymentLog}");
            return true;
        }

        public void EndActionTurn()
        {
            if (CurrentPhase == Phase.Action)
            {
                AdvanceTurnOrPhase();
            }
        }

        public int ResolveStormRoll(int forcedRoll = 0)
        {
            if (CurrentPhase != Phase.Storm)
            {
                return 0;
            }

            var roll = forcedRoll is >= 1 and <= 10 ? forcedRoll : random.Next(1, 11);
            var movingStorm = roll <= 5;
            var direction = roll % 2 == 0 ? 1 : -1;
            var distance = movingStorm ? roll : roll - 5;
            var affected = new List<int>();

            for (var step = 0; step < distance; step++)
            {
                if (movingStorm)
                {
                    JunkStormOutpost = WrapOutpost(JunkStormOutpost + direction);
                    affected.Add(JunkStormOutpost);
                }
                else
                {
                    BiodomerOutpost = WrapOutpost(BiodomerOutpost + direction);
                }
            }

            if (movingStorm)
            {
                foreach (var outpostIndex in affected)
                {
                    DrawOutpostCard(outpostIndex);
                    foreach (var player in Players.Where(candidate => candidate.ExpeditionOutpost == outpostIndex))
                    {
                        HitByStorm(player);
                    }
                }
            }
            else
            {
                foreach (var player in Players.Where(candidate => candidate.ExpeditionOutpost == BiodomerOutpost))
                {
                    HitByBiodomers(player);
                }
            }

            AddLog($"Rolled {roll}: {(movingStorm ? "Junk Storm" : "Biodomers")} moved.");
            CurrentPhase = Phase.Reset;
            return roll;
        }

        public void ResetGeneration()
        {
            if (CurrentPhase != Phase.Reset)
            {
                return;
            }

            var winner = Players.FirstOrDefault(player => player.Clout >= 10 && player.Buildings.Any(name => buildings.First(candidate => candidate.Name == name).Tier == 3));
            if (winner != null)
            {
                Winner = winner.Name;
                CurrentPhase = Phase.GameOver;
                AddLog($"{Winner} wins the future of humanity!");
                return;
            }

            foreach (var player in Players)
            {
                player.Discard.AddRange(player.Hand);
                player.Hand.Clear();
                player.ExpeditionOutpost = -1;
                player.ExpeditionWorkers = 0;
                player.StormShield = false;
                player.BiodomeShield = false;
                player.Resources.ClearTemporary();
                Draw(player, 5);
            }

            Generation++;
            ActivePlayerIndex = 0;
            CurrentPhase = Phase.Expedition;
            AddLog($"Generation {Generation} begins.");
        }

        private void ApplyEffect(PlayerState player, Effect effect)
        {
            player.Clout += effect.Clout;
            player.Workers += effect.Workers;
            player.Resources.Alloy += effect.Alloy;
            player.Resources.Organics += effect.Organics;
            player.Resources.Plastoid += effect.Plastoid;
            player.Resources.Labor += effect.Labor;
            player.Resources.Defense += effect.Defense;
            player.StormShield |= effect.StormShield;
            player.BiodomeShield |= effect.BiodomeShield;
            Draw(player, effect.Draw);

            var opponent = Players.FirstOrDefault(candidate => candidate != player);
            if (opponent == null)
            {
                return;
            }

            opponent.Clout = Math.Max(1, opponent.Clout - effect.TargetCloutLoss);
            opponent.Workers = Math.Max(0, opponent.Workers - effect.TargetWorkerLoss);
        }

        private void HitByStorm(PlayerState player)
        {
            if (player.StormShield)
            {
                AddLog($"{player.Name}'s Storm Shield cancelled the Junk Storm.");
                return;
            }

            DrawPlayerCardToDestroyed(player);
            var lost = player.ExpeditionWorkers;
            player.Workers = Math.Max(0, player.Workers - lost);
            player.Clout = Math.Max(1, player.Clout - lost);
            AddLog($"{player.Name} lost {lost} worker(s) to the Junk Storm.");
        }

        private void HitByBiodomers(PlayerState player)
        {
            if (player.BiodomeShield)
            {
                AddLog($"{player.Name}'s defense cancelled the Biodomer attack.");
                return;
            }

            var lost = Math.Min(2, Math.Min(player.Workers, player.ExpeditionWorkers));
            player.Workers -= lost;
            player.Clout = Math.Max(1, player.Clout - lost);
            AddLog($"{player.Name} lost {lost} worker(s) to Biodomers.");
        }

        private PlayerState CreatePlayer(int index)
        {
            var character = Characters[index];
            var deck = StarterDeck.ToList();
            deck.AddRange(character == "Albus" ? new[] { "Alloy", "Plastoid" } : new[] { "Organics", "Organics" });

            return new PlayerState
            {
                Name = $"Player {index + 1}",
                Character = character,
                Deck = Shuffle(deck)
            };
        }

        private List<OutpostState> CreateOutposts()
        {
            return new List<OutpostState>
            {
                new("City Ruins", Shuffle(new[] { "Alloy", "Alloy", "Alloy", "Organics", "Organics", "Organics", "Plastoid", "Plastoid", "Plastoid", "Thieving Resources", "Thieving Resources", "Spreading Rumors", "Spreading Rumors" })),
                new("Suburbs", Shuffle(new[] { "Organics", "Organics", "Organics", "Organics", "Plastoid", "Plastoid", "Plastoid", "Dweller", "Dweller", "Resource Trader", "Resource Trader", "Silver-Tongued", "Silver-Tongued" })),
                new("Research Facility", Shuffle(new[] { "Plastoid", "Plastoid", "Plastoid", "Plastoid", "Alloy", "Alloy", "Alloy", "Expert Scavenger", "Expert Scavenger", "Adept Planner", "Adept Planner", "Preemptive Intelligence", "Preemptive Intelligence" })),
                new("Military Base", Shuffle(new[] { "Alloy", "Alloy", "Alloy", "Alloy", "Plastoid", "Plastoid", "Plastoid", "Soldier", "Soldier", "Weapon Outfitting", "Weapon Outfitting", "Defecting Workers", "Defecting Workers" })),
                new("Farmlands", Shuffle(new[] { "Organics", "Organics", "Organics", "Organics", "Organics", "Organics", "Organics", "Worker", "Worker", "Working Man's Man", "Working Man's Man", "Augmented Resource", "Augmented Resource" }))
            };
        }

        private List<string> Shuffle(IEnumerable<string> source)
        {
            return source.OrderBy(_ => random.Next()).ToList();
        }

        private void Draw(PlayerState player, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (player.Deck.Count == 0 && player.Discard.Count > 0)
                {
                    player.Deck = Shuffle(player.Discard);
                    player.Discard.Clear();
                }

                if (player.Deck.Count == 0)
                {
                    return;
                }

                player.Hand.Add(player.Deck[0]);
                player.Deck.RemoveAt(0);
            }
        }

        private void DrawPlayerCardToDestroyed(PlayerState player)
        {
            if (player.Deck.Count == 0 && player.Discard.Count > 0)
            {
                player.Deck = Shuffle(player.Discard);
                player.Discard.Clear();
            }

            if (player.Deck.Count == 0)
            {
                return;
            }

            player.Destroyed.Add(player.Deck[0]);
            player.Deck.RemoveAt(0);
        }

        private string DrawOutpostCard(int outpostIndex)
        {
            var deck = Outposts[outpostIndex].Deck;
            if (deck.Count == 0)
            {
                return null;
            }

            var card = deck[0];
            deck.RemoveAt(0);
            return card;
        }

        private void AdvanceTurnOrPhase()
        {
            if (ActivePlayerIndex < Players.Count - 1)
            {
                ActivePlayerIndex++;
                return;
            }

            ActivePlayerIndex = 0;
            CurrentPhase = CurrentPhase switch
            {
                Phase.Expedition => Phase.Action,
                Phase.Action => Phase.Storm,
                _ => CurrentPhase
            };
        }

        private int WrapOutpost(int index)
        {
            return (index + Outposts.Count) % Outposts.Count;
        }

        private void AddLog(string message)
        {
            Log.Insert(0, message);
            if (Log.Count > 12)
            {
                Log.RemoveAt(Log.Count - 1);
            }
        }

        private static Dictionary<string, CardDefinition> CreateCards()
        {
            return new Dictionary<string, CardDefinition>
            {
                ["Dweller"] = new("Dweller", CardType.Starter, new Effect { Clout = 1 }, new Effect { Clout = 2 }),
                ["Worker"] = new("Worker", CardType.Starter, new Effect { Labor = 1 }, new Effect { Labor = 2 }),
                ["Soldier"] = new("Soldier", CardType.Defense, new Effect { Defense = 1 }, new Effect { BiodomeShield = true }),
                ["Storm Shield"] = new("Storm Shield", CardType.Defense, new Effect { Draw = 1 }, new Effect { StormShield = true }),
                ["Alloy"] = new("Alloy", CardType.Resource, new Effect { Alloy = 1 }, new Effect { Alloy = 2 }),
                ["Organics"] = new("Organics", CardType.Resource, new Effect { Organics = 1 }, new Effect { Organics = 2 }),
                ["Plastoid"] = new("Plastoid", CardType.Resource, new Effect { Plastoid = 1 }, new Effect { Plastoid = 2 }),
                ["Thieving Resources"] = new("Thieving Resources", CardType.Attack, new Effect { Clout = 1 }, new Effect { Alloy = 1, Plastoid = 1 }),
                ["Spreading Rumors"] = new("Spreading Rumors", CardType.Attack, new Effect { TargetCloutLoss = 1 }, new Effect { TargetCloutLoss = 2 }),
                ["Resource Trader"] = new("Resource Trader", CardType.Passive, new Effect { Alloy = 1 }, new Effect { Alloy = 1, Plastoid = 1 }),
                ["Silver-Tongued"] = new("Silver-Tongued", CardType.Passive, new Effect { Clout = 1 }, new Effect { Clout = 1, Draw = 1 }),
                ["Expert Scavenger"] = new("Expert Scavenger", CardType.Passive, new Effect { Draw = 1 }, new Effect { Draw = 2 }),
                ["Adept Planner"] = new("Adept Planner", CardType.Passive, new Effect { Draw = 2 }, new Effect { Draw = 3 }),
                ["Preemptive Intelligence"] = new("Preemptive Intelligence", CardType.Defense, new Effect { Defense = 1, Draw = 1 }, new Effect { Defense = 2 }),
                ["Weapon Outfitting"] = new("Weapon Outfitting", CardType.Defense, new Effect { Defense = 1 }, new Effect { BiodomeShield = true }),
                ["Defecting Workers"] = new("Defecting Workers", CardType.Attack, new Effect { TargetWorkerLoss = 1 }, new Effect { TargetWorkerLoss = 2 }),
                ["Working Man's Man"] = new("Working Man's Man", CardType.Passive, new Effect { Workers = 1 }, new Effect { Workers = 2 }),
                ["Augmented Resource"] = new("Augmented Resource", CardType.Passive, new Effect { Organics = 1 }, new Effect { Organics = 2 })
            };
        }

        private static List<BuildingDefinition> CreateBuildings()
        {
            return new List<BuildingDefinition>
            {
                new("Farm", 1, new ResourcePool { Organics = 4, Labor = 1 }, 2),
                new("Military Base", 1, new ResourcePool { Alloy = 3, Plastoid = 1, Labor = 1 }, 2),
                new("Laboratory", 1, new ResourcePool { Alloy = 1, Plastoid = 3, Labor = 1 }, 2),
                new("Transportation Station", 1, new ResourcePool { Alloy = 2, Plastoid = 2, Labor = 1 }, 2),
                new("Town Hall", 1, new ResourcePool { Organics = 3, FlexibleAlloyPlastoid = 1, Labor = 1 }, 2),
                new("Apartment Building", 1, new ResourcePool { Organics = 3, FlexibleAlloyPlastoid = 1, Labor = 1 }, 2),
                new("Farming Center", 2, new ResourcePool { Organics = 6, Labor = 2 }, 2, "Farm", 6),
                new("Weapons Factory", 2, new ResourcePool { Alloy = 4, Plastoid = 3, Labor = 2 }, 2, "Military Base", 6),
                new("Internet Servers", 2, new ResourcePool { Alloy = 3, Plastoid = 4, Labor = 2 }, 2, "Laboratory", 6),
                new("Subway Station", 2, new ResourcePool { Organics = 2, FlexibleAlloyPlastoid = 3, Labor = 2 }, 2, "Transportation Station", 6),
                new("Terraforming Station", 3, new ResourcePool { Organics = 6, Alloy = 2, Plastoid = 2, Labor = 3 }, 0, "Farming Center", 10),
                new("Rocket Launch Pad", 3, new ResourcePool { Alloy = 6, Plastoid = 4, Labor = 3 }, 0, "Weapons Factory", 10),
                new("Deep Earth Drill", 3, new ResourcePool { Alloy = 4, Plastoid = 4, Organics = 2, Labor = 3 }, 0, "Subway Station", 10),
                new("Advanced Communications Array", 3, new ResourcePool { Alloy = 3, Plastoid = 6, Organics = 1, Labor = 3 }, 0, "Internet Servers", 10)
            };
        }
    }
}

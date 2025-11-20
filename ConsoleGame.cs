// BEROS - Complete IDE-Only Game (C# .NET 8)
// Target: Console + Simulated 3D World + Multiplayer (Local)
// Paste into Program.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BEROS
{
    #region === CORE ENGINE ===
    public static class Game
    {
        public static Player LocalPlayer;
        public static World CurrentWorld;
        public static System.Timers.Timer GameLoop;
        public static Random RNG = new Random();
        public static bool IsRunning = true;

        public static void Start()
        {
            Console.Title = "BEROS v1.0.0 - Gummy Bear Farming & Racing";
            Console.Clear();
            PrintHeader();

            CurrentWorld = new World();
            LocalPlayer = new Player("You", "brown_gummy_bear");
            CurrentWorld.SpawnPlayer(LocalPlayer);

            GameLoop = new System.Timers.Timer(100); // 10 FPS simulation
            GameLoop.Elapsed += Update;
            GameLoop.AutoReset = true;
            GameLoop.Start();

            InputLoop();
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   ____  _____ ____   ___  ____  
  | __ )| ____|  _ \ / _ \/ ___| 
  |  _ \|  _| | |_) | | | \___ \ 
  | |_) | |___|  _ <| |_| |___) |
  |____/|_____|_| \_\\___/|____/ 
  Gummy Bear Farming & Racing Adventure
            ");
            Console.ResetColor();
        }

        static async void InputLoop()
        {
            while (IsRunning)
            {
                var key = Console.ReadKey(true).Key;
                LocalPlayer.HandleInput(key);
                await Task.Delay(50);
            }
        }

        static void Update(object sender, ElapsedEventArgs e)
        {
            CurrentWorld.Update();
            Render();
        }

        static void Render()
        {
            Console.SetCursorPosition(0, 8);
            Console.WriteLine(CurrentWorld.GetVisual());
            Console.WriteLine($"\n{LocalPlayer.GetStatus()}");
            Console.WriteLine($"Zone: {CurrentWorld.CurrentZone.Name} | Time: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine("Controls: WASD=Move, Space=Jump, E=Interact, F=Special, R=Race, Q=Quit");
        }

        public static void Quit() => IsRunning = false;
    }
    #endregion

    #region === WORLD & ZONES ===
    public class World
    {
        public Zone CurrentZone;
        public Dictionary<string, Zone> Zones = new();
        public List<Player> Players = new();

        public World()
        {
            Zones["orchard_garden"] = new OrchardGarden();
            Zones["cotton_candy_fields"] = new CottonCandyFields();
            Zones["lollipop_forest"] = new LollipopForest();
            Zones["rainbow_racetrack"] = new RainbowRacetrack();
            CurrentZone = Zones["orchard_garden"];
        }

        public void SpawnPlayer(Player p)
        {
            Players.Add(p);
            p.Position = CurrentZone.SpawnPoint;
            p.CurrentZone = CurrentZone;
        }

        public void Update()
        {
            foreach (var zone in Zones.Values) zone.Update();
            foreach (var player in Players) player.Update();
        }

        public string GetVisual()
        {
            return CurrentZone.GetVisual(Players);
        }
    }

    public abstract class Zone
    {
        public string Name { get; protected set; }
        public Vector2 SpawnPoint = new(0, 0);
        public List<FarmPlot> Plots = new();
        public List<Interactable> Interactables = new();

        public virtual void Update() { }
        public abstract string GetVisual(List<Player> players);
        public virtual void OnPlayerEnter(Player p) => Console.WriteLine($"[Zone] Welcome to {Name}!");
    }

    public class OrchardGarden : Zone
    {
        public OrchardGarden()
        {
            Name = "Orchard Garden";
            SpawnPoint = new Vector2(2, 2);
            for (int i = 0; i < 6; i++) Plots.Add(new FarmPlot { Crop = Crop.AppleTree, Position = new Vector2(i % 3 * 3, i / 3 * 3) });
            Interactables.Add(new NPC("Brown Gummy Bear", "Hello! Water my apples!"));
        }

        public override string GetVisual(List<Player> players)
        {
            char[,] map = new char[10, 30];
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 30; x++)
                    map[y, x] = '.';

            // Draw plots
            foreach (var plot in Plots)
            {
                char c = plot.IsHarvestable ? 'A' : 'a';
                map[(int)plot.Position.Y, (int)plot.Position.X] = c;
            }

            // Draw player
            foreach (var p in players)
            {
                if (p.CurrentZone == this)
                    map[(int)p.Position.Y, (int)p.Position.X] = 'P';
            }

            string visual = "";
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 30; x++)
                    visual += map[y, x];
                visual += "\n";
            }
            return visual;
        }
    }

    // Other zones (simplified)
    public class CottonCandyFields : Zone { public CottonCandyFields() { Name = "Cotton Candy Fields"; } public override string GetVisual(List<Player> players) => "Fluffy pink fields..."; }
    public class LollipopForest : Zone { public LollipopForest() { Name = "Lollipop Forest"; } public override string GetVisual(List<Player> players) => "Swirling candy canes..."; }
    public class RainbowRacetrack : Zone
    {
        public bool RaceInProgress = false;
        public List<Player> Racers = new();

        public RainbowRacetrack()
        {
            Name = "Rainbow Racetrack";
            SpawnPoint = new Vector2(0, 5);
        }

        public void StartRace()
        {
            RaceInProgress = true;
            Racers.Clear();
            foreach (var p in Game.CurrentWorld.Players)
                if (p.CurrentZone == this) Racers.Add(p);
            Console.WriteLine($"[RACE] {Racers.Count} players starting!");
        }

        public override string GetVisual(List<Player> players)
        {
            string track = @"
    START ==== Rainbow Road ==== FINISH
    ";
            return track;
        }
    }
    #endregion

    #region === PLAYER & INPUT ===
    public class Player
    {
        public string Name;
        public string BearType;
        public Vector2 Position;
        public Zone CurrentZone;
        public Tool CurrentTool;
        public int Sparkles = 100;
        public int MagicKeys = 0;
        public int FarmingXP = 0;
        public Tractor Tractor;

        public Player(string name, string bearType)
        {
            Name = name;
            BearType = bearType;
            CurrentTool = new RainbowWateringCan();
            Tractor = bearType switch
            {
                "brown_gummy_bear" => new Tractor { Color = "Brown", Icon = "Apple", Trait = "Durable" },
                "pink_gummy_bear" => new Tractor { Color = "Pink", Icon = "Cotton", Trait = "Boost" },
                _ => new Tractor { Color = "Blue", Icon = "Lollipop", Trait = "Handling" }
            };
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W: Position.Y = Math.Max(0, Position.Y - 1); break;
                case ConsoleKey.S: Position.Y = Math.Min(9, Position.Y + 1); break;
                case ConsoleKey.A: Position.X = Math.Max(0, Position.X - 1); break;
                case ConsoleKey.D: Position.X = Math.Min(29, Position.X + 1); break;
                case ConsoleKey.E: Interact(); break;
                case ConsoleKey.F: UseSpecial(); break;
                case ConsoleKey.R: EnterRace(); break;
                case ConsoleKey.Q: Game.Quit(); break;
            }
        }

        void Interact()
        {
            var plot = CurrentZone.Plots.Find(p => Vector2.Distance(p.Position, Position) < 3);
            if (plot != null)
            {
                if (CurrentTool is RainbowWateringCan)
                    plot.Water();
                else if (plot.IsHarvestable)
                {
                    int reward = plot.Harvest();
                    Sparkles += reward;
                    FarmingXP += 10;
                    Console.WriteLine($"[Harvest] +{reward} Sparkles! XP +10");
                }
            }
        }

        void UseSpecial()
        {
            Console.WriteLine($"[Special] {BearType} uses ability!");
        }

        void EnterRace()
        {
            if (CurrentZone is RainbowRacetrack track)
            {
                Game.CurrentWorld.CurrentZone = track;
                CurrentZone = track;
                if (!track.RaceInProgress) track.StartRace();
            }
        }

        public void Update()
        {
            // Auto-grow crops in current zone
        }

        public string GetStatus()
        {
            int level = (int)Math.Sqrt(FarmingXP / 100);
            return $"Player: {Name} | Sparkles: {Sparkles} | Keys: {MagicKeys} | Farm Level: {level} | Tool: {CurrentTool.Name}";
        }
    }
    #endregion

    #region === FARMING ===
    public class FarmPlot
    {
        public Crop Crop;
        public float Growth = 0f;
        public bool IsWatered = false;
        public Vector2 Position;

        public bool IsHarvestable => Growth >= 1f;

        public void Water()
        {
            IsWatered = true;
            Console.WriteLine("[Water] Plot watered with rainbow can!");
        }

        public int Harvest()
        {
            int reward = Crop.SparklesReward;
            Growth = 0f;
            Console.WriteLine($"[Harvest] {Crop.Name} harvested!");
            return reward;
        }

        public void Update()
        {
            if (Crop == null) return;
            float rate = IsWatered ? 0.02f : 0.01f;
            Growth = Math.Min(1f, Growth + rate);
            IsWatered = false;
        }
    }

    public class Crop
    {
        public string Name;
        public int SparklesReward;
        public float GrowthTime;

        public static Crop AppleTree = new() { Name = "Apple Tree", SparklesReward = 15, GrowthTime = 60 };
    }
    #endregion

    #region === TOOLS & TRACTOR ===
    public abstract class Tool
    {
        public string Name;
        public virtual void Use() { }
    }

    public class RainbowWateringCan : Tool
    {
        public RainbowWateringCan() => Name = "Rainbow Watering Can";
    }

    public class Tractor
    {
        public string Color;
        public string Icon;
        public string Trait;
    }
    #endregion

    #region === INTERACTABLES ===
    public interface Interactable { void Interact(Player p); }

    public class NPC : Interactable
    {
        public string Name;
        public string Dialogue;

        public NPC(string name, string dialogue)
        {
            Name = name;
            Dialogue = dialogue;
        }

        public void Interact(Player p) => Console.WriteLine($"[NPC] {Name}: {Dialogue}");
    }
    #endregion

    #region === MATH ===
    public struct Vector2
    {
        public float X, Y;
        public Vector2(float x, float y) { X = x; Y = y; }
        public static float Distance(Vector2 a, Vector2 b) => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
    #endregion

    #region === PROGRAM ENTRY ===
    class Program
    {
        static void Main(string[] args)
        {
            Game.Start();
            while (Game.IsRunning) System.Threading.Thread.Sleep(100);
            Console.WriteLine("Thanks for playing BEROS!");
        }
    }
    #endregion
}

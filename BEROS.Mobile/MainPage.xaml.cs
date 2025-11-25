using SkiaSharp;
using SkiaSharp.Views.Maui;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEROS.Mobile;

public partial class MainPage : ContentPage
{
    private readonly GameState _game;
    private IDispatcherTimer? _gameLoop;
    private float _joystickX, _joystickY;

    public MainPage()
    {
        InitializeComponent();
        _game = new GameState();
        
        // Start game loop when loaded
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        // Start game loop at ~60 FPS
        _gameLoop = Dispatcher.CreateTimer();
        _gameLoop.Interval = TimeSpan.FromMilliseconds(16);
        _gameLoop.Tick += (s, e) => GameCanvas.InvalidateSurface();
        _gameLoop.Start();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var width = e.Info.Width;
        var height = e.Info.Height;
        
        canvas.Clear(new SKColor(135, 206, 235)); // Sky blue

        // Update game state
        _game.Update(_joystickX, _joystickY);

        // Draw game world
        DrawWorld(canvas, width, height);
        DrawFarmPlots(canvas, width, height);
        DrawPlayer(canvas, width, height);
        DrawParticles(canvas);

        // Update UI labels
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SparklesLabel.Text = $"✨ Sparkles: {_game.Player.Sparkles}";
            ZoneLabel.Text = $"🏡 {_game.CurrentZone}";
        });
    }

    private void DrawWorld(SKCanvas canvas, int width, int height)
    {
        using var groundPaint = new SKPaint
        {
            Color = new SKColor(34, 139, 34), // Forest green
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        
        // Draw ground (bottom half)
        canvas.DrawRect(0, height * 0.4f, width, height * 0.6f, groundPaint);

        // Draw decorative elements based on zone
        DrawZoneDecorations(canvas, width, height);
    }

    private void DrawZoneDecorations(SKCanvas canvas, int width, int height)
    {
        using var treePaint = new SKPaint
        {
            Color = new SKColor(46, 125, 50),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        // Draw some trees in background
        for (int i = 0; i < 5; i++)
        {
            float x = width * (0.1f + i * 0.2f);
            float y = height * 0.35f;
            
            // Tree trunk
            using var trunkPaint = new SKPaint
            {
                Color = new SKColor(139, 69, 19),
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(x - 5, y, 10, 40, trunkPaint);
            
            // Tree crown
            using var path = new SKPath();
            path.MoveTo(x - 30, y);
            path.LineTo(x + 30, y);
            path.LineTo(x, y - 50);
            path.Close();
            canvas.DrawPath(path, treePaint);
        }
    }

    private void DrawFarmPlots(SKCanvas canvas, int width, int height)
    {
        float plotSize = Math.Min(width, height) * 0.1f;
        float startX = width * 0.1f;
        float startY = height * 0.5f;

        foreach (var plot in _game.Plots)
        {
            float x = startX + plot.GridX * (plotSize + 20);
            float y = startY + plot.GridY * (plotSize + 20);

            // Draw soil
            using var soilPaint = new SKPaint
            {
                Color = plot.IsWatered ? new SKColor(101, 67, 33) : new SKColor(139, 90, 43),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            canvas.DrawRoundRect(x, y, plotSize, plotSize, 5, 5, soilPaint);

            // Draw crop if growing
            if (plot.Growth > 0)
            {
                float cropSize = plotSize * 0.3f + (plotSize * 0.4f * plot.Growth);
                using var cropPaint = new SKPaint
                {
                    Color = plot.IsHarvestable ? SKColors.Red : SKColors.LightGreen,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawCircle(x + plotSize / 2, y + plotSize / 2, cropSize / 2, cropPaint);

                // Draw apple emoji for ready crops
                if (plot.IsHarvestable)
                {
                    using var font = new SKFont
                    {
                        Size = plotSize * 0.5f
                    };
                    using var textPaint = new SKPaint
                    {
                        IsAntialias = true
                    };
                    canvas.DrawText("🍎", x + plotSize * 0.25f, y + plotSize * 0.7f, SKTextAlign.Left, font, textPaint);
                }
            }
        }
    }

    private void DrawPlayer(SKCanvas canvas, int width, int height)
    {
        float playerSize = Math.Min(width, height) * 0.08f;
        float x = width * 0.5f + _game.Player.X * 5;
        float y = height * 0.45f + _game.Player.Y * 5;

        // Constrain player position
        x = Math.Clamp(x, playerSize, width - playerSize);
        y = Math.Clamp(y, height * 0.35f, height * 0.85f);

        // Draw gummy bear body (brown)
        using var bearPaint = new SKPaint
        {
            Color = new SKColor(139, 69, 19), // Saddle brown for brown gummy bear
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        // Body
        canvas.DrawOval(x, y, playerSize * 0.6f, playerSize * 0.8f, bearPaint);

        // Head
        canvas.DrawCircle(x, y - playerSize * 0.7f, playerSize * 0.45f, bearPaint);

        // Ears
        canvas.DrawCircle(x - playerSize * 0.35f, y - playerSize * 0.95f, playerSize * 0.15f, bearPaint);
        canvas.DrawCircle(x + playerSize * 0.35f, y - playerSize * 0.95f, playerSize * 0.15f, bearPaint);

        // Eyes
        using var eyePaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawCircle(x - playerSize * 0.15f, y - playerSize * 0.75f, playerSize * 0.08f, eyePaint);
        canvas.DrawCircle(x + playerSize * 0.15f, y - playerSize * 0.75f, playerSize * 0.08f, eyePaint);

        // Smile
        using var smilePath = new SKPath();
        smilePath.MoveTo(x - playerSize * 0.15f, y - playerSize * 0.55f);
        smilePath.QuadTo(x, y - playerSize * 0.4f, x + playerSize * 0.15f, y - playerSize * 0.55f);
        using var smilePaint = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };
        canvas.DrawPath(smilePath, smilePaint);

        // Store player screen position for interactions
        _game.Player.ScreenX = x;
        _game.Player.ScreenY = y;
    }

    private void DrawParticles(SKCanvas canvas)
    {
        foreach (var particle in _game.Particles.ToList())
        {
            if (particle.Life <= 0) continue;

            // Rainbow colors for water particles
            var colors = new[] { SKColors.Red, SKColors.Orange, SKColors.Yellow, 
                                  SKColors.Green, SKColors.Blue, SKColors.Purple };
            var color = colors[Math.Abs(particle.GetHashCode()) % colors.Length];
            
            using var paint = new SKPaint
            {
                Color = color.WithAlpha((byte)(255 * particle.Life)),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };
            
            canvas.DrawCircle(particle.X, particle.Y, 5 * particle.Life, paint);
        }
    }

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        e.Handled = true;
        
        // Calculate joystick input from left side of screen
        float screenWidth = (float)GameCanvas.Width;
        
        if (e.Location.X < screenWidth * 0.4f) // Left side for joystick
        {
            if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
            {
                // Calculate relative movement (-1 to 1)
                _joystickX = (e.Location.X - screenWidth * 0.15f) / (screenWidth * 0.15f);
                _joystickY = (e.Location.Y - (float)GameCanvas.Height * 0.85f) / ((float)GameCanvas.Height * 0.1f);
                
                _joystickX = Math.Clamp(_joystickX, -1f, 1f);
                _joystickY = Math.Clamp(_joystickY, -1f, 1f);
            }
            else if (e.ActionType == SKTouchAction.Released)
            {
                _joystickX = 0;
                _joystickY = 0;
            }
        }
    }

    private void OnInteract(object? sender, EventArgs e)
    {
        _game.WaterNearbyPlots();
        SpawnWaterParticles();
    }

    private void OnRace(object? sender, EventArgs e)
    {
        _game.CurrentZone = "Rainbow Racetrack";
        _game.Player.InRace = true;
    }

    private void SpawnWaterParticles()
    {
        var random = new Random();
        for (int i = 0; i < 10; i++)
        {
            _game.Particles.Add(new Particle
            {
                X = _game.Player.ScreenX + (random.NextSingle() - 0.5f) * 50,
                Y = _game.Player.ScreenY + (random.NextSingle() - 0.5f) * 50,
                VelocityX = (random.NextSingle() - 0.5f) * 3,
                VelocityY = -random.NextSingle() * 2,
                Life = 1.0f
            });
        }
    }
}

// Game State Classes
public partial class GameState : ObservableObject
{
    public Player Player { get; } = new();
    public List<FarmPlot> Plots { get; } = new();
    public List<Particle> Particles { get; } = new();
    
    [ObservableProperty]
    private string _currentZone = "Orchard Garden";

    public GameState()
    {
        // Initialize 6 farm plots in a 3x2 grid
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Plots.Add(new FarmPlot { GridX = x, GridY = y, Growth = 0.5f });
            }
        }
    }

    public void Update(float joystickX, float joystickY)
    {
        // Update player position
        Player.X += joystickX * 0.5f;
        Player.Y += joystickY * 0.5f;

        // Constrain player bounds
        Player.X = Math.Clamp(Player.X, -50, 50);
        Player.Y = Math.Clamp(Player.Y, -30, 80);

        // Update plot growth
        foreach (var plot in Plots)
        {
            if (plot.Growth < 1.0f)
            {
                float growthRate = plot.IsWatered ? 0.002f : 0.0005f;
                plot.Growth = Math.Min(1.0f, plot.Growth + growthRate);
            }
            
            // Reset watered state over time
            if (plot.WateredTimer > 0)
            {
                plot.WateredTimer--;
                if (plot.WateredTimer <= 0)
                    plot.IsWatered = false;
            }
        }

        // Update particles
        foreach (var particle in Particles.ToList())
        {
            particle.X += particle.VelocityX;
            particle.Y += particle.VelocityY;
            particle.VelocityY += 0.1f; // Gravity
            particle.Life -= 0.02f;
            
            if (particle.Life <= 0)
                Particles.Remove(particle);
        }
    }

    public void WaterNearbyPlots()
    {
        foreach (var plot in Plots)
        {
            // Water plots that might be nearby (simplified distance check)
            float distance = MathF.Sqrt(MathF.Pow(plot.GridX - 1, 2) + MathF.Pow(plot.GridY, 2));
            if (distance < 2)
            {
                if (plot.IsHarvestable)
                {
                    // Harvest!
                    Player.Sparkles += 15;
                    plot.Growth = 0;
                }
                else
                {
                    plot.IsWatered = true;
                    plot.WateredTimer = 300; // 5 seconds at 60 FPS
                }
            }
        }
    }
}

public class Player
{
    public float X { get; set; }
    public float Y { get; set; }
    public float ScreenX { get; set; }
    public float ScreenY { get; set; }
    public int Sparkles { get; set; } = 100;
    public bool InRace { get; set; }
}

public class FarmPlot
{
    public int GridX { get; set; }
    public int GridY { get; set; }
    public float Growth { get; set; }
    public bool IsWatered { get; set; }
    public int WateredTimer { get; set; }
    public bool IsHarvestable => Growth >= 1.0f;
}

public class Particle
{
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float Life { get; set; } = 1.0f;
}

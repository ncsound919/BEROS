using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System.IO;
using Microsoft.AspNetCore.SignalR.Client;

namespace BEROS.Mobile;

public partial class MainPage : ContentPage
{
    private SKCanvasView canvasView;
    private SKBitmap playerSprite;
    private SKPoint playerPos = new SKPoint(100, 100);
    private List<Particle> particles = new List<Particle>();
    private HubConnection hubConnection;
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        canvasView = new SKCanvasView();
        canvasView.PaintSurface += OnPaintSurface;
        Content = canvasView;
        LoadAssets();
        ConnectToServer();
    }

    private void LoadAssets()
    {
        playerSprite = LoadAsset("player.png");
    }

    private SKBitmap LoadAsset(string name)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync($"assets/{name}").Result;
        return SKBitmap.Decode(stream);
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        canvas.DrawBitmap(playerSprite, playerPos);
        // Draw particles
        foreach (var p in particles.Where(p => p.life > 0))
        {
            canvas.DrawCircle(p.pos, 3, new SKPaint { Color = SKColors.Red });
        }
    }

    private void ConnectToServer()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/gamehub")
            .Build();
        hubConnection.StartAsync();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterBtn.Text = $"Clicked {count} times";
        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    class Particle
    {
        public SKPoint pos;
        public float life;
    }
}

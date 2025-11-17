#!/usr/bin/env python3
"""
AutoCoder (LLM-free release)
- Lightweight runtime: reads YAML tasks and renders Jinja2 templates into /out
- No LLM dependencies included
"""

import argparse
import json
import subprocess
import time
from pathlib import Path
from typing import Any, Dict

import yaml
from jinja2 import Environment, FileSystemLoader, StrictUndefined

ROOT = Path(__file__).resolve().parent
TPL_DIR = ROOT / "templates"
TASK_DIR = ROOT / "tasks"
OUT_DIR = ROOT / "out"
REPORTS_DIR = ROOT / "reports"
REPORTS_DIR.mkdir(exist_ok=True)


def load_yaml(p: Path) -> Dict[str, Any]:
    with p.open("r", encoding="utf-8") as f:
        return yaml.safe_load(f)


def render_file(env: Environment, tpl_name: str, context: Dict[str, Any]) -> str:
    tpl = env.get_template(tpl_name)
    return tpl.render(**context)


def write_text(path: Path, text: str):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8") as f:
        f.write(text)


def run(cmd) -> int:
    try:
        if isinstance(cmd, str):
            return subprocess.run(cmd, shell=True, check=False, cwd=ROOT).returncode
        else:
            return subprocess.run(cmd, check=False, cwd=ROOT).returncode
    except FileNotFoundError:
        return 127


def handle_task(
    task_path: Path, format_code: bool = True, run_tests: bool = False
) -> dict:
    env = Environment(
        loader=FileSystemLoader(TPL_DIR),
        undefined=StrictUndefined,
        trim_blocks=True,
        lstrip_blocks=True,
    )
    spec = load_yaml(task_path)
    task_id = spec.get("task_id", task_path.stem)
    files = spec.get("files", [])
    results = {"task_id": task_id, "generated": [], "errors": []}

    for f in files:
        tpl = f["template"]
        out_rel = f["output"]
        ctx = f.get("context", {})
        try:
            rendered = render_file(env, tpl, ctx)
            out_path = OUT_DIR / out_rel
            write_text(out_path, rendered)
            results["generated"].append(str(out_rel))
        except Exception as e:
            results["errors"].append({"file": out_rel, "error": str(e)})

    # format with black if available
    if format_code:
        run(["python", "-m", "black", str(OUT_DIR)])

    # optional pytest run
    if run_tests:
        run(["python", "-m", "pytest", str(OUT_DIR)])

    # optional build command
    build_cmd_str = spec.get("build_command")
    if build_cmd_str:
        build_cmd = build_cmd_str.split()
        build_result = run(build_cmd)
        if build_result == 0:
            print(f"[AutoCoder] Build successful for {task_id}")
        else:
            print(f"[AutoCoder] Build failed for {task_id} with code {build_result}")
        results["build_result"] = build_result

    # write a report
    report_path = REPORTS_DIR / f"{task_id}.json"
    with report_path.open("w", encoding="utf-8") as f:
        json.dump(results, f, indent=2)
    return results


def scan_once(format_code: bool, run_tests: bool):
    for p in TASK_DIR.glob("*.yaml"):
        done_flag = p.with_suffix(".done")
        if done_flag.exists():
            continue
        res = handle_task(p, format_code, run_tests)
        done_flag.write_text("ok")
        print(
            f"[AutoCoder] Task {p.name} -> {len(res['generated'])} files, {len(res['errors'])} errors."
        )


def watch_loop(interval: float, format_code: bool, run_tests: bool):
    print(f"[AutoCoder] Watching {TASK_DIR} every {interval}s. Ctrl+C to stop.")
    while True:
        scan_once(format_code, run_tests)
        time.sleep(interval)


def generate_from_blueprint(blueprint_path: Path):
    with blueprint_path.open("r", encoding="utf-8") as f:
        blueprint = json.load(f)
    # Generate files
    # 1. Create project directory
    proj_dir = OUT_DIR / "BEROS.Mobile"
    proj_dir.mkdir(parents=True, exist_ok=True)
    # 2. Generate csproj
    csproj = f'''<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net9.0-android;net9.0-ios</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>BEROS.Mobile</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <ApplicationTitle>BEROS</ApplicationTitle>
    <ApplicationId>com.company.beros</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
  </PropertyGroup>

  <ItemGroup>
    <MauiIcon Include="Resources\\AppIcon\\appicon.svg" ForegroundFile="Resources\\AppIcon\\appiconfg.svg" Color="#512BD4" />
    <MauiSplashScreen Include="Resources\\Splash\\splash.svg" Color="#512BD4" BaseSize="128,128" />
    <MauiImage Include="Resources\\Images\\*" />
    <MauiFont Include="Resources\\Fonts\\*" />
    <MauiAsset Include="Resources\\Raw\\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
  </ItemGroup>

  <ItemGroup>
'''
    for pkg in blueprint['beros_game_blueprint']['nuget_packages']:
        csproj += f'    <PackageReference Include="{pkg}" Version="latest" />\n'
    csproj += '''  </ItemGroup>

</Project>'''
    write_text(proj_dir / "BEROS.Mobile.csproj", csproj)
    # 3. Generate MainPage.xaml
    xaml = '''<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="BEROS.Mobile.MainPage"
             BackgroundColor="{DynamicResource SecondaryColor}">
    <ScrollView>
        <VerticalStackLayout
            Spacing="25"
            Padding="30,0"
            VerticalOptions="Center">
            <Image
                Source="dotnet_bot.png"
                SemanticProperties.Description="Cute dot net bot waving hi to you!"
                HeightRequest="200"
                HorizontalOptions="Center" />
            <Label
                Text="Hello, World!"
                SemanticProperties.HeadingLevel="Level1"
                FontSize="32"
                HorizontalOptions="Center" />
            <Label
                Text="Welcome to .NET Multi-platform App UI"
                SemanticProperties.HeadingLevel="Level2"
                SemanticProperties.Description="Welcome to dot net Multi platform App U I"
                FontSize="18"
                HorizontalOptions="Center" />
            <Button
                x:Name="CounterBtn"
                Text="Click me"
                SemanticProperties.Hint="Counts the number of times you click"
                Clicked="OnCounterClicked"
                HorizontalOptions="Center" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>'''
    write_text(proj_dir / "MainPage.xaml", xaml)
    # 4. Generate MainPage.xaml.cs
    cs = '''using Microsoft.Maui.Controls;
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
'''
    write_text(proj_dir / "MainPage.xaml.cs", cs)
    print("Generated Beros Android project")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument(
        "--once", action="store_true", help="Process pending tasks once and exit"
    )
    ap.add_argument(
        "--interval", type=float, default=3.0, help="Watch interval seconds"
    )
    ap.add_argument("--no-format", action="store_true", help="Skip code formatting")
    ap.add_argument("--test", action="store_true", help="Run pytest after generation")
    ap.add_argument("--blueprint", help="Blueprint JSON file to generate from")
    args = ap.parse_args()
    if args.blueprint:
        generate_from_blueprint(Path(args.blueprint))
    elif args.once:
        scan_once(format_code=not args.no_format, run_tests=args.test)
    else:
        watch_loop(args.interval, format_code=not args.no_format, run_tests=args.test)


if __name__ == "__main__":
    main()

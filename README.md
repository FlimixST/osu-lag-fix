# osu! Lag Fix

A lightweight application that reduces lag spikes in osu! when streaming on Discord

## What it does
- raises the priority of osu! processes to High when they are running
- lowers Discord's priority to Below Normal
- checks the state every 1 second

## Autostart
You can make the program run automatically on Windows startup:
1. Press **Win+R** and open the startup folder using one of these commands:
   - `shell:startup`
   - `%appdata%\Microsoft\Windows\Start Menu\Programs\Startup`
2. Place either a shortcut to `osulagfix.exe` or the `osulagfix.exe` file itself into the opened folder

## Requirements
- to build from source: .NET 8 SDK

## Notes
- if osu! is not running, the utility changes nothing
- administrator rights are usually not required. If you see an access error when changing priority, try running as administrator
- the utility does not modify files or the registry - only process priorities while it is running
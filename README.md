# Pong Championship - Retro Arcade Experience

Welcome to **Pong Championship**, a high-fidelity, premium retro arcade game written in C# using the native Windows Forms framework. 

This game was built from scratch using only core .NET APIs, and compiles directly on standard Windows environments via `csc.exe`. It features a fully responsive viewport, robust AI opponent settings, immersive synthesized audio sound effects, screen shake feedback, custom collision particle bursts, and persistent high score tracking.

## Game Play Modes
1. **Player vs AI**: Take on the computer in a single-player duel.
2. **Player vs Player (Local)**: Challenge a friend on the same keyboard in local PvP.
3. **AI vs AI (Watch Mode)**: Sit back and watch two computer opponents duel with perfect or standard physics tracking.

## Controls
- **Player 1 (Left / Blue)**: 
  - `W` to move paddle Up
  - `S` to move paddle Down
- **Player 2 (Right / Red)**: 
  - `Up Arrow` to move paddle Up
  - `Down Arrow` to move paddle Down
- **General Menu & Navigation**:
  - `1`, `2`, `3` to select Game Mode
  - `E`, `M`, `H`, `I` to select AI Difficulty (Easy, Medium, Hard, Impossible)
  - `S` to toggle sound effects on/off
  - `SPACE` to start a match or restart from game over
  - `ESC` to pause/resume an active game

## Technical Features
- **Preserved Aspect Ratio Viewport**: The rendering engine scales all entities dynamically using GDI+ matrix transformations. No matter how you resize the window, the game field maintains its 4:3 aspect ratio and centers itself with clean retro pillarbox/letterbox bars.
- **Synthesized Audio Engine**: The application generates its own `.wav` files programmatically on startup by writing raw RIFF WAV byte streams. It doesn't depend on external audio assets.
- **Particle Dynamics**: Impact collisions on walls or paddles trigger dynamic particle bursts that decay in alpha and scale over time.
- **Screen Shake**: Scoring triggers a visceral viewport shake effect to emphasize points won.
- **State Management**: Built-in state machine for Intro, Playing, Paused, and Game Over modes.
- **Persistent High Scores**: High scores are written to and loaded from a local configuration file.


<!-- Commit step 49 of 150 -->

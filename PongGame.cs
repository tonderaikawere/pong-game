using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace PongGame
{
    public class PongForm : Form
    {
        public enum GameState
        {
            Intro,
            Playing,
            Paused,
            GameOver
        }

        public enum PlayMode
        {
            PlayerVsAI,
            PlayerVsPlayer,
            AIVsAI
        }

        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            Impossible
        }

        private struct Particle
        {
            public float X;
            public float Y;
            public float Vx;
            public float Vy;
            public float Size;
            public int Life;
            public Color Color;
        }

        private const float LogicalWidth = 800f;
        private const float LogicalHeight = 600f;
        private const float PaddleWidth = 15f;
        private const float PaddleHeight = 90f;
        private const float BallSize = 12f;

        private System.Media.SoundPlayer soundHitPaddle;
        private System.Media.SoundPlayer soundHitWall;
        private System.Media.SoundPlayer soundScore;
        private System.Media.SoundPlayer soundGameOver;

        private const string WavHitPaddle = "hit_paddle.wav";
        private const string WavHitWall = "hit_wall.wav";
        private const string WavScore = "score.wav";
        private const string WavGameOver = "game_over.wav";

        private PlayMode playMode = PlayMode.PlayerVsAI;
        private Difficulty difficulty = Difficulty.Medium;
        private bool soundEnabled = true;
        private int highScore = 0;

        private GameState state = GameState.Intro;
        private int player1Score = 0;
        private int player2Score = 0;
        private const int WinningScore = 10;

        private float paddle1Y = 255f;
        private float paddle2Y = 255f;
        private float ballX = 394f;
        private float ballY = 294f;
        private float ballSpeedX = 6f;
        private float ballSpeedY = 4f;
        private const float BaseBallSpeedX = 6f;
        private float playerPaddleSpeed = 8f;

        private bool wPressed = false;
        private bool sPressed = false;
        private bool upPressed = false;
        private bool downPressed = false;

        private List<Particle> particles = new List<Particle>();
        private Random random = new Random();
        private int shakeDuration = 0;
        private float shakeIntensity = 0f;

        private Timer gameTimer;

        public PongForm()
        {
            this.Text = "PONG CHAMPIONSHIP - ARCADE CLASSIC";
            this.ClientSize = new Size(800, 600);

// Commit step 100 of 150

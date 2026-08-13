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
            this.MinimumSize = new Size(400, 300);
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitSoundFiles();
            LoadSoundPlayers();

            highScore = HighscoreManager.LoadHighScore();

            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void InitSoundFiles()
        {
            try
            {
                if (!File.Exists(WavHitPaddle))
                    SoundGenerator.GenerateBeep(WavHitPaddle, 440.0, 0.1);
                if (!File.Exists(WavHitWall))
                    SoundGenerator.GenerateBeep(WavHitWall, 220.0, 0.08);
                if (!File.Exists(WavScore))
                    SoundGenerator.GenerateScoreBeep(WavScore);
                if (!File.Exists(WavGameOver))
                    SoundGenerator.GenerateGameOverBeep(WavGameOver);
            }
            catch { }
        }

        private void LoadSoundPlayers()
        {
            try
            {
                soundHitPaddle = new System.Media.SoundPlayer(WavHitPaddle);
                soundHitWall = new System.Media.SoundPlayer(WavHitWall);
                soundScore = new System.Media.SoundPlayer(WavScore);
                soundGameOver = new System.Media.SoundPlayer(WavGameOver);

                soundHitPaddle.Load();
                soundHitWall.Load();
                soundScore.Load();
                soundGameOver.Load();
            }
            catch { }
        }

        private void PlaySound(System.Media.SoundPlayer player)
        {
            if (soundEnabled && player != null)
            {
                try
                {
                    player.Play();
                }
                catch { }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (state == GameState.Playing)
            {
                UpdatePhysics();
            }
            else
            {
                UpdateParticlesOnly();
            }
            this.Invalidate();
        }

        private void ResetBall(bool toPlayer1)
        {
            ballX = LogicalWidth / 2f - BallSize / 2f;
            ballY = LogicalHeight / 2f - BallSize / 2f;
            ballSpeedX = toPlayer1 ? -BaseBallSpeedX : BaseBallSpeedX;
            ballSpeedY = (float)(random.NextDouble() * 6.0 - 3.0);
        }

        private void ResetGame()
        {
            player1Score = 0;
            player2Score = 0;
            paddle1Y = (LogicalHeight - PaddleHeight) / 2f;
            paddle2Y = (LogicalHeight - PaddleHeight) / 2f;
            ResetBall(random.Next(2) == 0);
            particles.Clear();
        }

        private void TriggerScreenShake(int duration, float intensity)
        {
            shakeDuration = duration;
            shakeIntensity = intensity;
        }

        private void SpawnPaddleHitParticles(float x, float y, Color col)
        {
            for (int i = 0; i < 15; i++)
            {
                Particle p;
                p.X = x;
                p.Y = y;
                p.Vx = (float)(random.NextDouble() * 6.0 - 3.0) + (ballSpeedX > 0 ? -2f : 2f);
                p.Vy = (float)(random.NextDouble() * 6.0 - 3.0);
                p.Size = (float)(random.NextDouble() * 5.0 + 3.0);
                p.Life = 255;
                p.Color = col;
                particles.Add(p);
            }
        }

        private void SpawnScoreParticles(float x, float y, Color col)
        {
            for (int i = 0; i < 30; i++)
            {
                Particle p;
                p.X = x;
                p.Y = y;
                p.Vx = (float)(random.NextDouble() * 10.0 - 5.0);
                p.Vy = (float)(random.NextDouble() * 10.0 - 5.0);
                p.Size = (float)(random.NextDouble() * 6.0 + 4.0);
                p.Life = 255;
                p.Color = col;
                particles.Add(p);
            }
        }

        private void UpdatePhysics()
        {
            if (playMode != PlayMode.AIVsAI)
            {
                if (wPressed && paddle1Y > 0)
                {
                    paddle1Y -= playerPaddleSpeed;
                }
                if (sPressed && paddle1Y < LogicalHeight - PaddleHeight)
                {
                    paddle1Y += playerPaddleSpeed;
                }
            }
            else
            {
                RunAIMovement(ref paddle1Y, ballX, ballY, difficulty);
            }

            if (playMode == PlayMode.PlayerVsPlayer)
            {
                if (upPressed && paddle2Y > 0)
                {
                    paddle2Y -= playerPaddleSpeed;
                }
                if (downPressed && paddle2Y < LogicalHeight - PaddleHeight)
                {
                    paddle2Y += playerPaddleSpeed;
                }
            }
            else
            {
                RunAIMovement(ref paddle2Y, ballX, ballY, difficulty);
            }

            ballX += ballSpeedX;
            ballY += ballSpeedY;

            if (ballY <= 0)
            {
                ballY = 0;
                ballSpeedY = -ballSpeedY;
                PlaySound(soundHitWall);
                SpawnPaddleHitParticles(ballX, ballY, Color.White);
            }
            else if (ballY >= LogicalHeight - BallSize)
            {
                ballY = LogicalHeight - BallSize;
                ballSpeedY = -ballSpeedY;
                PlaySound(soundHitWall);
                SpawnPaddleHitParticles(ballX, ballY, Color.White);
            }

            if (ballSpeedX < 0 && ballX <= 40f + PaddleWidth && ballX >= 40f - BallSize)
            {
                if (ballY + BallSize >= paddle1Y && ballY <= paddle1Y + PaddleHeight)
                {
                    ballX = 40f + PaddleWidth;
                    ballSpeedX = -ballSpeedX;
                    ballSpeedX *= 1.05f;
                    float relativeIntersectY = (paddle1Y + (PaddleHeight / 2f)) - (ballY + (BallSize / 2f));
                    float normalizedIntersectY = relativeIntersectY / (PaddleHeight / 2f);
                    ballSpeedY = -normalizedIntersectY * 7f;

                    PlaySound(soundHitPaddle);
                    SpawnPaddleHitParticles(ballX, ballY + BallSize / 2f, Color.FromArgb(0, 102, 255));
                }
            }

            if (ballSpeedX > 0 && ballX + BallSize >= LogicalWidth - 40f - PaddleWidth && ballX + BallSize <= LogicalWidth - 40f + BallSize)
            {
                if (ballY + BallSize >= paddle2Y && ballY <= paddle2Y + PaddleHeight)
                {
                    ballX = LogicalWidth - 40f - PaddleWidth - BallSize;
                    ballSpeedX = -ballSpeedX;
                    ballSpeedX *= 1.05f;
                    float relativeIntersectY = (paddle2Y + (PaddleHeight / 2f)) - (ballY + (BallSize / 2f));
                    float normalizedIntersectY = relativeIntersectY / (PaddleHeight / 2f);
                    ballSpeedY = -normalizedIntersectY * 7f;

                    PlaySound(soundHitPaddle);
                    SpawnPaddleHitParticles(ballX + BallSize, ballY + BallSize / 2f, Color.FromArgb(255, 51, 51));
                }
            }

            if (ballX < 0)
            {
                player2Score++;
                PlaySound(soundScore);
                TriggerScreenShake(12, 10f);
                SpawnScoreParticles(20, ballY, Color.FromArgb(255, 51, 51));

                if (player2Score >= WinningScore)
                {
                    state = GameState.GameOver;
                    PlaySound(soundGameOver);
                    HighscoreManager.SaveHighScore(Math.Max(player1Score, player2Score));
                    highScore = HighscoreManager.LoadHighScore();
                }
                else
                {
                    ResetBall(false);
                }
            }
            else if (ballX > LogicalWidth)
            {
                player1Score++;
                PlaySound(soundScore);
                TriggerScreenShake(12, 10f);
                SpawnScoreParticles(LogicalWidth - 20, ballY, Color.FromArgb(0, 102, 255));

                if (player1Score >= WinningScore)
                {
                    state = GameState.GameOver;
                    PlaySound(soundGameOver);
                    HighscoreManager.SaveHighScore(Math.Max(player1Score, player2Score));
                    highScore = HighscoreManager.LoadHighScore();
                }
                else
                {
                    ResetBall(true);
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle p = particles[i];
                p.X += p.Vx;
                p.Y += p.Vy;
                p.Life -= 8;
                p.Size *= 0.95f;
                if (p.Life <= 0 || p.Size < 0.5f)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    particles[i] = p;
                }
            }
        }

        private void UpdateParticlesOnly()
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle p = particles[i];
                p.X += p.Vx;
                p.Y += p.Vy;
                p.Life -= 6;
                p.Size *= 0.96f;
                if (p.Life <= 0 || p.Size < 0.5f)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    particles[i] = p;
                }
            }

            if (state == GameState.Intro && random.Next(100) < 5)
            {
                float x = (float)(random.NextDouble() * LogicalWidth);
                float y = (float)(random.NextDouble() * LogicalHeight);
                Color col = random.Next(2) == 0 ? Color.FromArgb(0, 102, 255) : Color.FromArgb(255, 51, 51);
                Particle p;
                p.X = x;
                p.Y = y;
                p.Vx = (float)(random.NextDouble() * 2.0 - 1.0);
                p.Vy = (float)(random.NextDouble() * 2.0 - 1.0);
                p.Size = (float)(random.NextDouble() * 3.0 + 1.0);
                p.Life = 180;
                p.Color = col;
                particles.Add(p);
            }
        }

        private void RunAIMovement(ref float paddleY, float targetX, float targetY, Difficulty diff)
        {
            float centerPaddle = paddleY + PaddleHeight / 2f;
            float diffY = targetY - centerPaddle;
            float aiSpeed = 0f;

            switch (diff)
            {
                case Difficulty.Easy:
                    aiSpeed = 3.2f;
                    if (Math.Abs(diffY) > 25)
                    {
                        if (diffY > 0) paddleY += aiSpeed;
                        else paddleY -= aiSpeed;
                    }
                    break;
                case Difficulty.Medium:
                    aiSpeed = 4.8f;
                    if (Math.Abs(diffY) > 15)
                    {
                        if (diffY > 0) paddleY += aiSpeed;
                        else paddleY -= aiSpeed;
                    }
                    break;
                case Difficulty.Hard:
                    aiSpeed = 7.0f;
                    if (Math.Abs(diffY) > 5)
                    {
                        if (diffY > 0) paddleY += aiSpeed;
                        else paddleY -= aiSpeed;
                    }
                    break;
                case Difficulty.Impossible:
                    paddleY = targetY - PaddleHeight / 2f;
                    break;
            }

            if (paddleY < 0) paddleY = 0;
            if (paddleY > LogicalHeight - PaddleHeight) paddleY = LogicalHeight - PaddleHeight;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (state == GameState.Playing)
            {
                if (e.KeyCode == Keys.W) wPressed = true;
                if (e.KeyCode == Keys.S) sPressed = true;
                if (e.KeyCode == Keys.Up) upPressed = true;
                if (e.KeyCode == Keys.Down) downPressed = true;

                if (e.KeyCode == Keys.Escape)
                {
                    state = GameState.Paused;
                }
            }
            else if (state == GameState.Paused)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    state = GameState.Playing;
                }
            }
            else if (state == GameState.Intro)
            {
                if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1) playMode = PlayMode.PlayerVsAI;
                if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2) playMode = PlayMode.PlayerVsPlayer;
                if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3) playMode = PlayMode.AIVsAI;

                if (e.KeyCode == Keys.E) difficulty = Difficulty.Easy;
                if (e.KeyCode == Keys.M) difficulty = Difficulty.Medium;
                if (e.KeyCode == Keys.H) difficulty = Difficulty.Hard;
                if (e.KeyCode == Keys.I) difficulty = Difficulty.Impossible;

                if (e.KeyCode == Keys.S) soundEnabled = !soundEnabled;

                if (e.KeyCode == Keys.Space)
                {
                    ResetGame();
                    state = GameState.Playing;
                }
            }
            else if (state == GameState.GameOver)
            {
                if (e.KeyCode == Keys.Space)
                {
                    state = GameState.Intro;
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) wPressed = false;
            if (e.KeyCode == Keys.S) sPressed = false;
            if (e.KeyCode == Keys.Up) upPressed = false;
            if (e.KeyCode == Keys.Down) downPressed = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float targetAspect = LogicalWidth / LogicalHeight;
            float currentAspect = (float)this.ClientSize.Width / this.ClientSize.Height;
            float viewportWidth, viewportHeight, viewportX, viewportY;

            if (currentAspect > targetAspect)
            {
                viewportHeight = this.ClientSize.Height;
                viewportWidth = viewportHeight * targetAspect;
                viewportX = (this.ClientSize.Width - viewportWidth) / 2f;
                viewportY = 0;
            }
            else
            {
                viewportWidth = this.ClientSize.Width;
                viewportHeight = viewportWidth / targetAspect;
                viewportX = 0;
                viewportY = (this.ClientSize.Height - viewportHeight) / 2f;
            }

            g.Clear(Color.FromArgb(10, 10, 10));

            System.Drawing.Drawing2D.GraphicsState baseState = g.Save();

            if (state == GameState.Playing && shakeDuration > 0)
            {
                float dx = (float)(random.NextDouble() * 2.0 - 1.0) * shakeIntensity;
                float dy = (float)(random.NextDouble() * 2.0 - 1.0) * shakeIntensity;
                g.TranslateTransform(dx, dy);
                shakeDuration--;
            }

            g.TranslateTransform(viewportX, viewportY);
            g.ScaleTransform(viewportWidth / LogicalWidth, viewportHeight / LogicalHeight);

            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(18, 18, 20)))
            {
                g.FillRectangle(bgBrush, 0, 0, LogicalWidth, LogicalHeight);
            }

            using (Pen borderPen = new Pen(Color.FromArgb(50, 50, 50), 4f))
            {
                g.DrawRectangle(borderPen, 2, 2, LogicalWidth - 4, LogicalHeight - 4);
            }

            using (Pen netPen = new Pen(Color.FromArgb(60, 60, 65), 3f))
            {
                netPen.DashPattern = new float[] { 10f, 10f };
                g.DrawLine(netPen, LogicalWidth / 2f, 0, LogicalWidth / 2f, LogicalHeight);
            }

            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                using (SolidBrush pBrush = new SolidBrush(Color.FromArgb(p.Life, p.Color)))
                {
                    g.FillRectangle(pBrush, p.X, p.Y, p.Size, p.Size);
                }
            }

            switch (state)
            {
                case GameState.Intro:
                    DrawIntroScreen(g);
                    break;
                case GameState.Playing:
                    DrawPlayScreen(g);
                    break;
                case GameState.Paused:
                    DrawPlayScreen(g);
                    DrawPauseOverlay(g);
                    break;
                case GameState.GameOver:
                    DrawGameOverScreen(g);
                    break;
            }

            g.Restore(baseState);
        }

        private void DrawIntroScreen(Graphics g)
        {
            using (Font titleFont = new Font("Courier New", 42f, FontStyle.Bold))
            using (SolidBrush blueBrush = new SolidBrush(Color.FromArgb(0, 102, 255)))
            using (SolidBrush redBrush = new SolidBrush(Color.FromArgb(255, 51, 51)))
            {
                g.DrawString("PONG", titleFont, blueBrush, 180f, 60f);
                g.DrawString("CHAMPIONSHIP", titleFont, redBrush, 330f, 60f);
            }

            using (Font subFont = new Font("Consolas", 12f, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                g.DrawString("SELECT PLAY MODE:", subFont, textBrush, 150f, 170f);
                DrawOption(g, "[1] PLAYER VS AI", playMode == PlayMode.PlayerVsAI, 170f, 200f);
                DrawOption(g, "[2] PLAYER VS PLAYER (LOCAL)", playMode == PlayMode.PlayerVsPlayer, 170f, 225f);
                DrawOption(g, "[3] AI VS AI (WATCH MODE)", playMode == PlayMode.AIVsAI, 170f, 250f);

                g.DrawString("AI DIFFICULTY:", subFont, textBrush, 150f, 300f);
                DrawOption(g, "[E] EASY", difficulty == Difficulty.Easy, 170f, 330f);
                DrawOption(g, "[M] MEDIUM", difficulty == Difficulty.Medium, 270f, 330f);
                DrawOption(g, "[H] HARD", difficulty == Difficulty.Hard, 390f, 330f);
                DrawOption(g, "[I] IMPOSSIBLE", difficulty == Difficulty.Impossible, 490f, 330f);

                g.DrawString("SOUND SYSTEM:", subFont, textBrush, 150f, 380f);
                DrawOption(g, soundEnabled ? "[S] SOUND: ON" : "[S] SOUND: OFF", soundEnabled, 170f, 410f);

                using (Font blinkFont = new Font("Consolas", 16f, FontStyle.Bold))
                using (SolidBrush startBrush = new SolidBrush(Color.White))
                {
                    g.DrawString("PRESS [SPACE] TO START GAME", blinkFont, startBrush, 240f, 480f);
                }

                g.DrawString("P1: W/S Keys  |  P2: Up/Down Arrow Keys  |  ESC: Pause Game", subFont, textBrush, 160f, 540f);
                g.DrawString("HIGH SCORE: " + highScore, subFont, textBrush, 320f, 120f);
            }
        }

        private void DrawOption(Graphics g, string text, bool selected, float x, float y)
        {
            Color col = selected ? Color.FromArgb(0, 255, 100) : Color.FromArgb(120, 120, 120);
            using (Font f = new Font("Consolas", 11f, selected ? FontStyle.Bold : FontStyle.Regular))
            using (SolidBrush b = new SolidBrush(col))
            {
                g.DrawString(text, f, b, x, y);
            }
        }

        private void DrawPlayScreen(Graphics g)
        {
            using (Font scoreFont = new Font("Courier New", 48f, FontStyle.Bold))
            using (SolidBrush blueBrush = new SolidBrush(Color.FromArgb(0, 102, 255)))
            using (SolidBrush redBrush = new SolidBrush(Color.FromArgb(255, 51, 51)))
            {
                g.DrawString(player1Score.ToString(), scoreFont, blueBrush, LogicalWidth / 2f - 120f, 30f);
                g.DrawString(player2Score.ToString(), scoreFont, redBrush, LogicalWidth / 2f + 50f, 30f);
            }

            using (SolidBrush blueBrush = new SolidBrush(Color.FromArgb(0, 102, 255)))
            using (SolidBrush redBrush = new SolidBrush(Color.FromArgb(255, 51, 51)))
            {
                g.FillRectangle(blueBrush, 40f, paddle1Y, PaddleWidth, PaddleHeight);
                g.FillRectangle(redBrush, LogicalWidth - 40f - PaddleWidth, paddle2Y, PaddleWidth, PaddleHeight);
            }

            using (SolidBrush ballBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(ballBrush, ballX, ballY, BallSize, BallSize);
            }
        }

        private void DrawPauseOverlay(Graphics g)
        {
            using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            {
                g.FillRectangle(dimBrush, 0, 0, LogicalWidth, LogicalHeight);
            }

            using (Font titleFont = new Font("Courier New", 36f, FontStyle.Bold))
            using (Font subFont = new Font("Consolas", 14f, FontStyle.Regular))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                g.DrawString("GAME PAUSED", titleFont, whiteBrush, 250f, 220f);
                g.DrawString("Press ESC to Resume Play", subFont, whiteBrush, 280f, 300f);

// Commit step 148 of 150

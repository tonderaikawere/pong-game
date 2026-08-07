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

// Commit step 97 of 150

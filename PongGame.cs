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

// Commit step 96 of 150

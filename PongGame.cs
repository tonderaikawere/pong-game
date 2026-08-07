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

// Commit step 95 of 150

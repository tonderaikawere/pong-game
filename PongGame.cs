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

// Commit step 94 of 150

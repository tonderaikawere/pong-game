using System;
using System.IO;

namespace PongGame
{
    public static class HighscoreManager
    {
        private const string FileName = "highscores.txt";

        public static int LoadHighScore()
        {
            try
            {
                if (File.Exists(FileName))
                {

// Commit step 58 of 150

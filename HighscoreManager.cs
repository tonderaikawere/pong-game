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
                    string content = File.ReadAllText(FileName);
                    int score;
                    if (int.TryParse(content.Trim(), out score))
                    {
                        return score;
                    }
                }
            }
            catch
            {
                // Ignore load errors and default to 0
            }
            return 0;
        }

        public static void SaveHighScore(int newScore)
        {
            try
            {
                int currentHighScore = LoadHighScore();
                if (newScore > currentHighScore)
                {

// Commit step 65 of 150

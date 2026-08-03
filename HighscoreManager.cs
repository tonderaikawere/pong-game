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

// Commit step 61 of 150

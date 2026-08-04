using System;
using System.IO;

namespace PongGame
{
    public static class SoundGenerator
    {
        public static void GenerateBeep(string filePath, double frequency, double durationSeconds)
        {
            int sampleRate = 22050;
            short bitsPerSample = 16;
            short channels = 1;
            int numSamples = (int)(sampleRate * durationSeconds);
            int dataLength = numSamples * channels * (bitsPerSample / 8);
            
            using (FileStream fs = new FileStream(filePath, FileMode.Create))

// Commit step 70 of 150

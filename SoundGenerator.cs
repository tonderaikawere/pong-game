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
            using (BinaryWriter wr = new BinaryWriter(fs))
            {
                wr.Write(new char[] { 'R', 'I', 'F', 'F' });
                wr.Write(36 + dataLength);
                wr.Write(new char[] { 'W', 'A', 'V', 'E' });
                wr.Write(new char[] { 'f', 'm', 't', ' ' });
                wr.Write(16); // Subchunk1Size
                wr.Write((short)1); // AudioFormat (PCM)
                wr.Write(channels);

// Commit step 71 of 150

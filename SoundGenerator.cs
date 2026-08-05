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
                wr.Write(sampleRate);
                wr.Write(sampleRate * channels * (bitsPerSample / 8)); // ByteRate
                wr.Write((short)(channels * (bitsPerSample / 8))); // BlockAlign
                wr.Write(bitsPerSample);
                wr.Write(new char[] { 'd', 'a', 't', 'a' });
                wr.Write(dataLength);

                for (int i = 0; i < numSamples; i++)
                {
                    double t = (double)i / sampleRate;
                    double angle = 2.0 * Math.PI * frequency * t;
                    short sample = (short)(Math.Sin(angle) * 16000);
                    wr.Write(sample);
                }
            }
        }

        public static void GenerateScoreBeep(string filePath)
        {
            int sampleRate = 22050;
            short bitsPerSample = 16;
            short channels = 1;
            double dur1 = 0.15;
            double dur2 = 0.25;
            int numSamples1 = (int)(sampleRate * dur1);
            int numSamples2 = (int)(sampleRate * dur2);
            int totalSamples = numSamples1 + numSamples2;
            int dataLength = totalSamples * channels * (bitsPerSample / 8);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter wr = new BinaryWriter(fs))
            {
                wr.Write(new char[] { 'R', 'I', 'F', 'F' });
                wr.Write(36 + dataLength);
                wr.Write(new char[] { 'W', 'A', 'V', 'E' });
                wr.Write(new char[] { 'f', 'm', 't', ' ' });
                wr.Write(16);
                wr.Write((short)1);
                wr.Write(channels);
                wr.Write(sampleRate);
                wr.Write(sampleRate * channels * (bitsPerSample / 8));
                wr.Write((short)(channels * (bitsPerSample / 8)));
                wr.Write(bitsPerSample);
                wr.Write(new char[] { 'd', 'a', 't', 'a' });
                wr.Write(dataLength);

                for (int i = 0; i < numSamples1; i++)
                {
                    double t = (double)i / sampleRate;
                    double angle = 2.0 * Math.PI * 523.25 * t; // C5
                    short sample = (short)(Math.Sin(angle) * 16000);
                    wr.Write(sample);
                }
                for (int i = 0; i < numSamples2; i++)
                {
                    double t = (double)i / sampleRate;
                    double angle = 2.0 * Math.PI * 659.25 * t; // E5
                    short sample = (short)(Math.Sin(angle) * 16000);
                    wr.Write(sample);
                }
            }
        }

        public static void GenerateGameOverBeep(string filePath)
        {
            int sampleRate = 22050;
            short bitsPerSample = 16;
            short channels = 1;
            double duration = 0.8;
            int numSamples = (int)(sampleRate * duration);
            int dataLength = numSamples * channels * (bitsPerSample / 8);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter wr = new BinaryWriter(fs))
            {
                wr.Write(new char[] { 'R', 'I', 'F', 'F' });
                wr.Write(36 + dataLength);
                wr.Write(new char[] { 'W', 'A', 'V', 'E' });
                wr.Write(new char[] { 'f', 'm', 't', ' ' });
                wr.Write(16);
                wr.Write((short)1);
                wr.Write(channels);
                wr.Write(sampleRate);
                wr.Write(sampleRate * channels * (bitsPerSample / 8));
                wr.Write((short)(channels * (bitsPerSample / 8)));
                wr.Write(bitsPerSample);
                wr.Write(new char[] { 'd', 'a', 't', 'a' });
                wr.Write(dataLength);

                double phase = 0;
                for (int i = 0; i < numSamples; i++)
                {
                    double progress = (double)i / numSamples;
                    double currentFreq = 400.0 * (1.0 - progress) + 100.0;
                    phase += 2.0 * Math.PI * currentFreq / sampleRate;
                    short sample = (short)(Math.Sin(phase) * 16000);
                    wr.Write(sample);
                }
            }
        }
    }
}

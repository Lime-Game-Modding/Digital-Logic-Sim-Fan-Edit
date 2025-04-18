using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FanEdit.Sound
{
    public static class SoundManager
    {
        public static List<SoundPlayer> activePlayers = new();


        public static void Reset()
        {
            activePlayers = new();
        }
        public static void SetActiveSoundWave(SoundShape soundShape, float frequency, int volumeMultiplier, int ID)
        {
            SoundPlayer soundPlayer;
            switch (soundShape)
            {
                case SoundShape.Sine:
                { 
                    soundPlayer = new SineWavePlayer(frequency, volumeMultiplier, ID);
                    break;
                }

                case SoundShape.Square:
                {
                    soundPlayer = new SquareWavePlayer(frequency, volumeMultiplier, ID);
                    break;
                }

                case SoundShape.Triangle:
                {
                    soundPlayer = new TriangleWavePlayer(frequency, volumeMultiplier, ID);
                    break;
                }

                case SoundShape.Sawtooth:
                {
                    soundPlayer = new SawtoothWavePlayer(frequency, volumeMultiplier, ID);
                    break;
                }
                default:
                    soundPlayer = new SineWavePlayer(frequency, volumeMultiplier, ID);
                    break;
            }
            activePlayers.Add(soundPlayer);
        }

        public static void PlayWave(SoundShape shape, float frequency, int volumeMultiplier, int ID)
        {
            SetActiveSoundWave(shape, frequency, volumeMultiplier, ID);
        }

        public static void StopWave(int ID)
        {
            for(int i = 0; i<activePlayers.Count; i++)
            {
                if(activePlayers[i].uniqueID == ID)
                {
                    activePlayers[i].Stop();
                    activePlayers.RemoveAt(i);
                }
            }
        }
    }

    public abstract class SoundPlayer
    {
        public int uniqueID;

        public int timeIndex = 0;

        public int volumeMultiplier;

        public float frequency = 440;

        public float sampleRate = 44100;

        public float waveLengthInSeconds = 2f;

        public abstract void GenerateAudioFilterData(ref float[] data, int channels);
        public abstract void Play();
        public abstract void Stop();
    }

    public class SineWavePlayer : SoundPlayer
    {
        public SineWavePlayer(float _frequency, int _volumeMultiplier, int _uniqueID, float _sampleRate = 44100, float _waveLengthInSeconds = 2)
        {
            this.frequency = _frequency;
            this.volumeMultiplier = _volumeMultiplier;
            this.uniqueID = _uniqueID;
            this.sampleRate = _sampleRate;
            this.waveLengthInSeconds = _waveLengthInSeconds;
        }

        public override void GenerateAudioFilterData(ref float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] += CreateSine(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier;
                timeIndex++;

                if (channels == 2)
                { data[i + 1] += CreateSine(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier; }
                //if timeIndex gets too big, reset it to 0
                if (timeIndex >= (sampleRate * waveLengthInSeconds))
                {
                    timeIndex = 0;
                }
            }
        }

        //Creates a sinewave
        public float CreateSine(int timeIndex, float frequency, float sampleRate)
        {
            return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
        }

        public override void Play()
        {
            timeIndex = 0;  //resets timer before playing sound
            DLS.Game.UnityMain.instance.PlaySound(out timeIndex);
        }

        public override void Stop()
        {
            DLS.Game.UnityMain.instance.StopPlaySound();
        }
    }

    public class SquareWavePlayer : SoundPlayer
    {
        public SquareWavePlayer(float _frequency, int _volumeMultiplier, int _uniqueID, float _sampleRate = 44100, float _waveLengthInSeconds = 2)
        {
            this.frequency = _frequency;
            this.volumeMultiplier = _volumeMultiplier;
            this.uniqueID = _uniqueID;
            this.sampleRate = _sampleRate;
            this.waveLengthInSeconds = _waveLengthInSeconds;
        }

        public override void GenerateAudioFilterData(ref float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] += CreateSquare(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier;
                timeIndex++;

                if (channels == 2)
                { data[i + 1] += CreateSquare(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier; }
                //if timeIndex gets too big, reset it to 0
                if (timeIndex >= (sampleRate * waveLengthInSeconds))
                {
                    timeIndex = 0;
                }
            }
        }

        //Creates a sinewave
        public float CreateSquare(int timeIndex, float frequency, float sampleRate)
        {
            return Mathf.Sign(Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate));
        }

        public override void Play()
        {
            timeIndex = 0;  //resets timer before playing sound
            DLS.Game.UnityMain.instance.PlaySound(out timeIndex);
        }

        public override void Stop()
        {
            DLS.Game.UnityMain.instance.StopPlaySound();
        }
    }

    public class TriangleWavePlayer : SoundPlayer
    {
        public TriangleWavePlayer(float _frequency, int _volumeMultiplier, int _uniqueID, float _sampleRate = 44100, float _waveLengthInSeconds = 2)
        {
            this.frequency = _frequency;
            this.volumeMultiplier = _volumeMultiplier;
            this.uniqueID = _uniqueID;
            this.sampleRate = _sampleRate;
            this.waveLengthInSeconds = _waveLengthInSeconds;
        }

        public override void GenerateAudioFilterData(ref float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] += CreateTriangle(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier;
                timeIndex++;

                if (channels == 2)
                { data[i + 1] += CreateTriangle(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier; }
                //if timeIndex gets too big, reset it to 0
                if (timeIndex >= (sampleRate * waveLengthInSeconds))
                {
                    timeIndex = 0;
                }
            }
        }

        //Creates a sinewave
        public float CreateTriangle(int timeIndex, float frequency, float sampleRate)
        {
            float t = timeIndex * frequency / sampleRate;
            return 1f - 4f * (float)Mathf.Abs(Mathf.Round(t - 0.25f) - (t - 0.25f)); ;
        }

        public override void Play()
        {
            timeIndex = 0;  //resets timer before playing sound
            DLS.Game.UnityMain.instance.PlaySound(out timeIndex);
        }

        public override void Stop()
        {
            DLS.Game.UnityMain.instance.StopPlaySound();
        }
    }

    public class SawtoothWavePlayer : SoundPlayer
    {
        public SawtoothWavePlayer(float _frequency, int _volumeMultiplier, int _uniqueID, float _sampleRate = 44100, float _waveLengthInSeconds = 2)
        {
            this.frequency = _frequency;
            this.volumeMultiplier = _volumeMultiplier;
            this.uniqueID = _uniqueID;
            this.sampleRate = _sampleRate;
            this.waveLengthInSeconds = _waveLengthInSeconds;
        }

        public override void GenerateAudioFilterData(ref float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] += CreateSawtooth(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier;
                timeIndex++;

                if (channels == 2)
                { data[i + 1] += CreateSawtooth(timeIndex, frequency, sampleRate) * 0.02f * volumeMultiplier; }
                //if timeIndex gets too big, reset it to 0
                if (timeIndex >= (sampleRate * waveLengthInSeconds))
                {
                    timeIndex = 0;
                }
            }
        }

        public float CreateSawtooth(int timeIndex, float frequency, float sampleRate)
        {
            float t = timeIndex * frequency / sampleRate;
            return 2f * (t - (float)Mathf.Floor(t + 0.5f));
        }

        static float Fract(float t)
        {
            return t - Mathf.Floor(t);
        }

        public override void Play()
        {
            timeIndex = 0;  //resets timer before playing sound
            DLS.Game.UnityMain.instance.PlaySound(out timeIndex);
        }

        public override void Stop()
        {
            DLS.Game.UnityMain.instance.StopPlaySound();
        }
    }

    public enum SoundShape
    {
        Sine,
        Square,
        Triangle,
        Sawtooth
    }
}

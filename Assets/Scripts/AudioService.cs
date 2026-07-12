using System;
using UnityEngine;

namespace LighthouseMatch3
{
    public sealed class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        private AudioSource _source;
        private AudioClip _swap;
        private AudioClip _match;
        private AudioClip _win;
        private AudioClip _lose;

        private void Awake()
        {
            Instance = this;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.volume = .32f;
            _swap = Tone("Swap", 360, .05f, .10f);
            _match = Tone("Match", 600, .10f, .12f);
            _win = Tone("Win", 880, .24f, .14f);
            _lose = Tone("Lose", 180, .18f, .12f);
        }

        public void PlaySwap() => Play(_swap);
        public void PlayMatch() => Play(_match);
        public void PlayWin() => Play(_win);
        public void PlayLose() => Play(_lose);

        private void Play(AudioClip clip)
        {
            if (SaveService.Progress != null && SaveService.Progress.SoundEnabled) _source.PlayOneShot(clip);
        }

        private static AudioClip Tone(string name, float frequency, float duration, float volume)
        {
            const int rate = 44100;
            int samples = Mathf.CeilToInt(rate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float envelope = 1f - i / (float)samples;
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / rate) * volume * envelope;
            }
            var clip = AudioClip.Create(name, samples, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}


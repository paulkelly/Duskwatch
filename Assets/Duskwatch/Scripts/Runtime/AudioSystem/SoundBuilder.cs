using UnityEngine;

namespace AudioSystem
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        Vector3 position = Vector3.zero;
        bool randomPitch;
        bool setVolume;
        float volume = 1f;

        public SoundBuilder(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            this.randomPitch = true;
            return this;
        }
        
        public SoundBuilder WithVolume(float volume)
        {
            this.setVolume = true;
            this.volume = volume;
            return this;
        }

        public void Play(SoundData soundData)
        {
            if (soundData == null)
            {
                Debug.LogError("SoundData is null");
                return;
            }

            if (!soundData.CanPlay())
            {
                return;
            }

            if (!soundManager.CanPlaySound(soundData)) return;

            SoundEmitter soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (setVolume)
            {
                soundEmitter.SetVolume(volume);
            }

            if (soundData.frequentSound)
            {
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);
            }

            soundEmitter.Play();
        }

        public SoundEmitter PlayLooping(SoundData soundData)
        {
            SoundEmitter soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (soundData.frequentSound)
            {
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);
            }

            soundEmitter.Play();
            return soundEmitter;
        }
    }
}
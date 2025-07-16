using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using VARLab.CloudSave;

namespace VARLab.RespiratoryTherapy
{
    [CloudSaved]
    [JsonObject(MemberSerialization.OptIn)]
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource DialogAudioSource;
        public AudioSource EffectsAudioSource;
        [Header("Audio Mixer")]
        public AudioMixer MainAudioMixer;
        
        [FormerlySerializedAs("isMute")] [JsonProperty]
        public bool soundOn;

        [JsonProperty] 
        public float volumeLevel;
        
        private SoundPlayOptions playbackOptions;
        private AudioSource audioSource;
        
        private const string masterVolume = "Volume";
        private const float minVolume = -80.0f;
        
        #region Sound Options
        
        public static SoundPlayOptions DefaultSoundPlayOptions = new ()
        {
            Channel = SoundChannel.Dialog,
            Volume = 1f,
            Loop = false,
            StopCurrentClip = true
        };
        
        #endregion

        /// <summary>
        /// Plays an audio with optional playback settings. 
        /// If no playback options are provided, it defaults to the predefined options.
        /// </summary>
        /// <param name="audioClip">The audio clipt to be played.</param>
        /// <param name="soundPlayOptions">Optional playback settings. Defaults to the system's default
        /// options if null.</param>
        public void PlaySound(AudioClip audioClip, SoundPlayOptions? soundPlayOptions = null)
        {
            SetupAudioSourceSettings(soundPlayOptions);
            
            if(!audioSource)
                return;
            
            if (playbackOptions.StopCurrentClip)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        /// <summary>
        /// Stops the currently playing sound, if any, and resets the audio source settings.
        /// </summary>
        /// <param name="soundPlayOptions">Optional sound settings that can be passed to configure the audio
        /// source before stopping.</param>
        public void StopSound(SoundPlayOptions? soundPlayOptions = null)
        {
            SetupAudioSourceSettings(soundPlayOptions);
            audioSource.Stop();
            audioSource.clip = null;
        }

        /// Pauses the currently playing sound and optionally adjusts audio settings before pausing.
        /// </summary>
        /// <param name="soundPlayOptions">Optional sound settings that can be passed to configure the audio source
        /// before pausing.</param>
        public void PauseSound(SoundPlayOptions? soundPlayOptions = null)
        {
            SetupAudioSourceSettings(soundPlayOptions);
            audioSource.Pause();
        }
        
        /// <summary>
        /// Resumes playback of a paused sound and optionally adjusts audio settings before resuming.
        /// </summary>
        /// <param name="soundPlayOptions">Optional sound settings that can be passed to configure the audio
        /// source before resuming playback.</param>
        public void UnPauseSound(SoundPlayOptions? soundPlayOptions = null)
        {
            SetupAudioSourceSettings(soundPlayOptions);
            audioSource.UnPause();
        }
        
        /// <summary>
        /// Toggles the mute state for all audio sources (dialog, background, and effects).
        /// If sound is currently playing, it will be muted.
        /// </summary>
        public void HandleMuteSound(bool status)
        {
            MainAudioMixer.SetFloat(masterVolume, status ? DefaultSoundPlayOptions.Volume : minVolume);

            soundOn = status;
        }

        public void SetVolume(string group, float value)
        {
            MainAudioMixer.SetFloat(group, value);
            volumeLevel = value;
        }

        /// <summary>
        /// Configures and returns an AudioSource based on the specified playback options.
        /// Sets the volume and looping behavior of the audio source, and ensures that
        /// the current audio clip is stopped if looping is enabled.
        /// </summary>
        /// <param name="playbackOptions">The options that dictate how the audio source should be set up.</param>
        /// <returns>The configured AudioSource for the specified playback channel.</returns>
        private void SetupAudioSourceSettings(SoundPlayOptions? soundPlayOptions)
        {
            playbackOptions = soundPlayOptions ?? DefaultSoundPlayOptions;
            audioSource = GetAudioSourceForChannel(playbackOptions.Channel);
            audioSource.volume = playbackOptions.Volume;
            audioSource.loop = playbackOptions.Loop;
            
            //In order to loop the audio, playbackOptions.StopCurrentClip should be true.
            if (playbackOptions.Loop)
            {
                playbackOptions.StopCurrentClip = true;
            }
        }

        /// <summary>
        /// Retrieves the appropriate AudioSource based on the specified sound channel.
        /// If the channel is Background, Effect, or ItemInfo, it returns the corresponding AudioSource.
        /// If the AudioSource is not found, it logs a warning message.
        /// </summary>
        /// <param name="channel">The sound channel for which to get the AudioSource.</param>
        /// <returns>The AudioSource associated with the specified channel, or the default DialogAudioSource
        /// if not found.</returns>
        private AudioSource GetAudioSourceForChannel(SoundChannel channel)
        {
            AudioSource audioSource = channel switch
            {
                SoundChannel.Effect => EffectsAudioSource,
                SoundChannel.Dialog => DialogAudioSource,
                _ => DialogAudioSource
            };

            if (!audioSource)
            {
                Debug.LogWarning("Audio Source not found");
            }
            
            return audioSource;
        }
    }
}

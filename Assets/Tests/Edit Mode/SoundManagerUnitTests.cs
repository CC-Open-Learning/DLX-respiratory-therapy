using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using VARLab.RespiratoryTherapy;

namespace Tests.EditMode
{
    public class SoundManagerUnitTests
    {
        private GameObject soundManagerObject;
        private SoundManager soundManager;
        private AudioSource dialogSource;
        private AudioSource effectsAudioSource;
        private AudioClip testClip;

        [SetUp]
        public void Setup()
        {
            // Create a GameObject and attach the SoundManager component
            soundManagerObject = new("Sound Manager Object");
            soundManager = soundManagerObject.AddComponent<SoundManager>();

            // Create AudioSources and assign them to the SoundManager
            dialogSource = soundManagerObject.AddComponent<AudioSource>();
            effectsAudioSource = soundManagerObject.AddComponent<AudioSource>();
            soundManager.MainAudioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/AudioMixer.mixer");


            soundManager.DialogAudioSource = dialogSource;
            soundManager.EffectsAudioSource = effectsAudioSource;

            // Create a test audio clip
            testClip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
        }

        [TearDown]
        public void Teardown()
        {
            testClip = null;
            effectsAudioSource = null;
            dialogSource = null;
            soundManager = null;
            soundManagerObject = null;
        }

        /// <summary>
        /// Test ensures that when we play a sound and provide no options, that the default
        /// options are used
        /// </summary>
        [Test]
        public void PlaySound_UsesDefaultOptions_WhenNoOptionsProvided()
        {
            // Arrange
            SoundPlayOptions defaultOptions = SoundManager.DefaultSoundPlayOptions;

            // Act
            soundManager.PlaySound(testClip);

            // Assert
            Assert.AreEqual(testClip, dialogSource.clip); // Check if the clip is set
            Assert.AreEqual(defaultOptions.Volume, dialogSource.volume); // Volume should be default
            Assert.AreEqual(defaultOptions.Loop, dialogSource.loop); // Loop should be default
            Assert.IsTrue(dialogSource.isPlaying); // Check if audio is playing
        }

        /// <summary>
        /// Test ensures that when we play a sound and provide sound options, it uses them
        /// </summary>
        [Test]
        public void PlaySound_RespectsProvidedSoundPlayOptions()
        {
            // Arrange
            SoundPlayOptions customOptions = new SoundPlayOptions
            {
                Channel = SoundChannel.Dialog,
                Volume = 0.5f,
                Loop = true,
                StopCurrentClip = true
            };

            // Act
            soundManager.PlaySound(testClip, customOptions);

            // Assert
            Assert.AreEqual(testClip, dialogSource.clip); // Check if the clip is set in BackgroundAudioSource
            Assert.AreEqual(customOptions.Volume, dialogSource.volume); // Volume should match custom options
            Assert.AreEqual(customOptions.Loop, dialogSource.loop); // Loop should match custom options
            Assert.IsTrue(dialogSource.isPlaying); // Check if audio is playing
        }

        /// <summary>
        /// Test ensures that when we stop an audio clip, it actually stops playing
        /// </summary>
        [Test]
        public void StopSound_StopsAudioPlayback()
        {
            // Arrange
            soundManager.PlaySound(testClip); // Play sound first

            // Act
            soundManager.StopSound();

            // Assert
            Assert.IsFalse(dialogSource.isPlaying); // Audio should stop
            Assert.IsNull(dialogSource.clip); // Clip should be cleared
        }

        /// <summary>
        /// Test ensures that when we pause an audio clip, it actually pauses
        /// </summary>
        [Test]
        public void PauseSound_PausesAudioSource()
        {
            // Arrange
            dialogSource.clip = testClip;
            dialogSource.Play();

            // Act
            soundManager.PauseSound();

            // Assert
            Assert.IsTrue(dialogSource.isPlaying == false);
        }

        /// <summary>
        /// Test ensures that when we unpause an audio clip, it continues playing
        /// </summary>
        [Test]
        public void UnPauseSound_ResumesAudioSource()
        {
            // Arrange
            dialogSource.clip = testClip;
            dialogSource.Play();
            soundManager.PauseSound();

            // Act
            soundManager.UnPauseSound();

            // Assert
            Assert.IsTrue(dialogSource.isPlaying);
        }

        /// <summary>
        /// Test ensures that when we unnute an audio clip, it is playing
        /// </summary>
        [Test]
        public void HandleMuteSound_UnmutesAudioWhenFalse()
        {
            // Act
            soundManager.HandleMuteSound(false);

            // Assert
            float volume;
            soundManager.MainAudioMixer.GetFloat("Volume", out volume);
            Assert.AreEqual(0f, volume);  // Assuming 0f is the default volume in the DefaultSoundPlayOptions
        }
    }
}
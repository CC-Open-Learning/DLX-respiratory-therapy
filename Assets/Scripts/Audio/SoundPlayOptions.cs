namespace VARLab.RespiratoryTherapy
{
    public struct SoundPlayOptions 
    {
        private SoundChannel? channel;
        private float? volume;
        private bool? loop;
        private bool? stopCurrentClip;

        public SoundChannel Channel
        {
            get => channel ?? SoundManager.DefaultSoundPlayOptions.Channel;
            set => channel = value;
        }
        
        public float Volume
        {
            get => volume ?? SoundManager.DefaultSoundPlayOptions.Volume;
            set => volume = value;
        }
        
        public bool Loop
        {
            get => loop ?? SoundManager.DefaultSoundPlayOptions.Loop;
            set => loop = value;
        }
        
        public bool StopCurrentClip
        {
            get => stopCurrentClip ?? SoundManager.DefaultSoundPlayOptions.StopCurrentClip;
            set => stopCurrentClip = value;
        }
    }
}

namespace IOT_MyHome.Audio
{
    using System;
    using ManagedBass;
    using System.Diagnostics.Contracts;

    class Player
    {
        private MediaPlayer MPlayer;

        internal Player(string location)
        {
            Bass.Init();
            MPlayer = new MediaPlayer
            {
                Loop = true
            };
        }

        internal async void SetFileName(string filename)
        {
            if (filename == null)
            {
                return;
            }

            try
            {
                await MPlayer.LoadAsync(filename);
                MPlayer.Play();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sets volume between 0 and 100 (%).
        /// </summary>
        /// <param name="vol"></param>
        internal void SetVolume(int vol)
        {
            Contract.Requires(vol >= 0 && vol <= 100, "Volume must be between 0 and 100");

            MPlayer.Volume = (double)vol / 100;
        }
    }
}

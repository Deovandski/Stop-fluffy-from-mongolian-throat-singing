using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;

namespace MonitorFluffy.Utilities
{
    /// <summary>
    /// Meant For Handling Audio, including deciding which to play
    /// </summary>
    internal class AudioPlayer
    {
        private string[] recordings = null;
        private static string AudioPath = "Recordings/";

        internal AudioPlayer()
        {
            recordings = Directory.GetFiles(AudioPath);

            if (recordings.Length == 0)
            {
                throw new Exception("Recordings not found.");
            }
        }

        public string PlayRandomAudio()
        {
            int selection = new Random().Next(0, recordings.Length - 1);
            string inAudio = recordings[selection];
            SoundPlayer player = new SoundPlayer(inAudio);
            player.Play();
            return inAudio;
        }
    }
}

using System.IO;
using System.Media;

namespace GreenLumaGUI
{
    public static class SoundManager
    {
        private static readonly SoundPlayer soundPlayer = new SoundPlayer();


        public static void PlaySound(Stream soundStream)
        {
            soundPlayer.Stream = soundStream;
            soundPlayer.Play();
        }
    }
}

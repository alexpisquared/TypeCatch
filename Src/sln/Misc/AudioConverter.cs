using NAudio.Wave;
using System.IO;

namespace TypingWpf.Misc
{
    public class AudioConverter
	{
		public static void DoFolder(string dir)
		{
			foreach (var mp3 in Directory.GetFiles(dir, "*.mp3"))
			{
				var wav = mp3.Replace(".mp3", ".wav");

				if (!File.Exists(wav))
					ConvertMp3ToWav(mp3, wav);
			}
		}
		public static void ConvertMp3ToWav(string _inPath_, string _outPath_)
		{
			using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
			{
				using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
				{
					WaveFileWriter.CreateWaveFile(_outPath_, pcm);
				}
			}
		}
	}
}

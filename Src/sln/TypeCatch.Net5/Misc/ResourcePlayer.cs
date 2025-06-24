using System.Media;
using System.Runtime.InteropServices;

namespace TypeCatch.Net5.Misc;

public class ResourcePlayer
{
  readonly SoundPlayer _soundPlayer = new();
  readonly Random _rnd = new(DateTime.Now.Millisecond);

  public ResourcePlayer() => SystemVolume = ushort.MaxValue / 10;
  public void Test()
  {
    _soundPlayer.Stream = Properties.Resources.Bye___I_ll_be_back;

    SystemVolume = ushort.MaxValue; _soundPlayer.PlaySync();
    SystemVolume = ushort.MaxValue / 3; _soundPlayer.PlaySync();
    SystemVolume = ushort.MaxValue / 10; _soundPlayer.PlaySync();
  }
  public void PlayAudioQue() { _soundPlayer.Stream = Properties.Resources.AudioQue1; SystemVolume = ushort.MaxValue / 3; _soundPlayer.Play(); }
  public void PlayHelloSound()
  {

    switch (_rnd.Next(4))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Hello_01; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Hello___Computer; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Hello___Knocking_on_Door; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Hello___Typewriter; break;
    }
    SystemVolume = ushort.MaxValue / 3;
    _soundPlayer.Play();
  }
  public void PlayByeByeSound()
  {
    switch (_rnd.Next(6))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Bad___Police; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Bye_Bye; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Bye___Good_Bye; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Bye___Good_Bye__Arnold_; break;
      case 4: _soundPlayer.Stream = Properties.Resources.Bye___Hasta_La_Vista_Baby; break;
      case 5: _soundPlayer.Stream = Properties.Resources.Bye___I_ll_be_back; break;
        //cas6e 0: //obnoxious:        _resourcePlayer.Stream = TypeCatch.Net5.Properties.Resources.Bad___B_B_suxx2; break;
        //cas7e 1: //very disturbing:  _resourcePlayer.Stream = TypeCatch.Net5.Properties.Resources.Bad___Cry; break;
        //cas8e 2: //obnoxious:        _resourcePlayer.Stream = TypeCatch.Net5.Properties.Resources.Bad___Noseblow; break;
        //case 5: //obnoxious:        _resourcePlayer.Stream = TypeCatch.Net5.Properties.Resources.Bye___Apple; break;
    }

    _soundPlayer.PlaySync();
  }
  public void PlaySessionFinish_Good()
  {
    switch (_rnd.Next(15))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Good_01; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Good_02; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Good___All_right; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Good___Applause; break;
      case 4: _soundPlayer.Stream = Properties.Resources.Good___Applause_01; break;
      case 5: _soundPlayer.Stream = Properties.Resources.Good___Applause_02; break;
      case 6: _soundPlayer.Stream = Properties.Resources.Good___Excellent; break;
      case 7: _soundPlayer.Stream = Properties.Resources.Good___Fanfare; break;
      case 8: _soundPlayer.Stream = Properties.Resources.Good___Pleasure; break;
      case 9: _soundPlayer.Stream = Properties.Resources.Good___Wine; break;
      case 10: _soundPlayer.Stream = Properties.Resources.Good___Yea; break;
      case 11: _soundPlayer.Stream = Properties.Resources.Good___Yeah; break;
      case 12: _soundPlayer.Stream = Properties.Resources.Good___Yes_01; break;
      case 13: _soundPlayer.Stream = Properties.Resources.Good___Yes_02; break;
      case 14: _soundPlayer.Stream = Properties.Resources.Good___Yes_B_B; break;
    }

    _soundPlayer.PlaySync();
  }
  public void PlaySessionFinish_Baad()
  {
    switch (_rnd.Next(26))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Laugh_01; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Laugh_02; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Laugh_03; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Laugh_04; break;
      case 4: _soundPlayer.Stream = Properties.Resources.Laugh_05; break;
      case 5: _soundPlayer.Stream = Properties.Resources.Laugh_06; break;
      case 6: _soundPlayer.Stream = Properties.Resources.Laugh_07; break;
      case 7: _soundPlayer.Stream = Properties.Resources.Laugh_08; break;
      case 8: _soundPlayer.Stream = Properties.Resources.Laugh___Baby; break;
      case 9: _soundPlayer.Stream = Properties.Resources.Laugh___B_B; break;
      case 10: _soundPlayer.Stream = Properties.Resources.Laugh___Evil_giggle; break;
      case 11: _soundPlayer.Stream = Properties.Resources.Laugh___Female_01; break;
      case 12: _soundPlayer.Stream = Properties.Resources.Laugh___Female_02; break;
      case 13: _soundPlayer.Stream = Properties.Resources.Laugh___Group; break;
      case 14: _soundPlayer.Stream = Properties.Resources.Laugh___Haha; break;
      case 15: _soundPlayer.Stream = Properties.Resources.Laugh___Hahaha; break;
      case 16: _soundPlayer.Stream = Properties.Resources.Laugh___Hilari1; break;
      case 17: _soundPlayer.Stream = Properties.Resources.Laugh___Hilari2; break;
      case 18: _soundPlayer.Stream = Properties.Resources.Laugh___Hohooouu; break;
      case 19: _soundPlayer.Stream = Properties.Resources.Laugh___Male_01; break;
      case 20: _soundPlayer.Stream = Properties.Resources.Laugh___Male_02; break;
      case 21: _soundPlayer.Stream = Properties.Resources.Laugh___Male_03; break;
      case 22: _soundPlayer.Stream = Properties.Resources.Laugh___Male_04; break;
      case 23: _soundPlayer.Stream = Properties.Resources.Laugh___Male_05; break;
      case 24: _soundPlayer.Stream = Properties.Resources.Laugh___Snicker; break;
      case 25: _soundPlayer.Stream = Properties.Resources.Laugh___Witch_Cackle; break;
    }

    _soundPlayer.PlaySync();
  }
  public void PlayОw()
  {
    switch (_rnd.Next(46))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Ow_01; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Ow_02; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Ow_03; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Ow_04; break;
      case 4: _soundPlayer.Stream = Properties.Resources.Ow___Aaahh; break;
      case 5: _soundPlayer.Stream = Properties.Resources.Ow___Burp_01; break;
      case 6: _soundPlayer.Stream = Properties.Resources.Ow___Burp_02; break;
      case 7: _soundPlayer.Stream = Properties.Resources.Ow___B_B_1; break;
      case 8: _soundPlayer.Stream = Properties.Resources.Ow___B_B_2; break;
      case 9: _soundPlayer.Stream = Properties.Resources.Ow___B_B_3; break;
      case 10: _soundPlayer.Stream = Properties.Resources.Ow___B_B_4; break;
      case 11: _soundPlayer.Stream = Properties.Resources.Ow___B_B_fart1; break;
      case 12: _soundPlayer.Stream = Properties.Resources.Ow___B_B_fart2; break;
      case 13: _soundPlayer.Stream = Properties.Resources.Ow___Car; break;
      case 14: _soundPlayer.Stream = Properties.Resources.Ow___Car_Horn; break;
      case 15: _soundPlayer.Stream = Properties.Resources.Ow___Cough_01; break;
      case 16: _soundPlayer.Stream = Properties.Resources.Ow___Cough_02; break;
      case 17: _soundPlayer.Stream = Properties.Resources.Ow___Cough_03; break;
      case 18: _soundPlayer.Stream = Properties.Resources.Ow___Cow; break;
      case 19: _soundPlayer.Stream = Properties.Resources.Ow___Crush_01; break;
      case 20: _soundPlayer.Stream = Properties.Resources.Ow___Crush_02; break;
      case 21: _soundPlayer.Stream = Properties.Resources.Ow___Cuckoo; break;
      case 22: _soundPlayer.Stream = Properties.Resources.Ow___Dog; break;
      case 23: _soundPlayer.Stream = Properties.Resources.Ow___Fart_01; break;
      case 24: _soundPlayer.Stream = Properties.Resources.Ow___Fart_02; break;
      case 25: _soundPlayer.Stream = Properties.Resources.Ow___Fast_fart; break;
      case 26: _soundPlayer.Stream = Properties.Resources.Ow___Female_01; break;
      case 27: _soundPlayer.Stream = Properties.Resources.Ow___Female_02; break;
      case 28: _soundPlayer.Stream = Properties.Resources.Ow___Female_03; break;
      case 29: _soundPlayer.Stream = Properties.Resources.Ow___Gargle; break;
      case 30: _soundPlayer.Stream = Properties.Resources.Ow___Goat; break;
      case 31: _soundPlayer.Stream = Properties.Resources.Ow___Male_01; break;
      case 32: _soundPlayer.Stream = Properties.Resources.Ow___Male_02; break;
      case 33: _soundPlayer.Stream = Properties.Resources.Ow___Male_03; break;
      case 34: _soundPlayer.Stream = Properties.Resources.Ow___Male_04; break;
      case 35: _soundPlayer.Stream = Properties.Resources.Ow___Male_05; break;
      case 36: _soundPlayer.Stream = Properties.Resources.Ow___No; break;
      case 37: _soundPlayer.Stream = Properties.Resources.Ow___Noo; break;
      case 38: _soundPlayer.Stream = Properties.Resources.Ow___Ooff; break;
      case 39: _soundPlayer.Stream = Properties.Resources.Ow___Ouch; break;
      case 40: _soundPlayer.Stream = Properties.Resources.Ow___Pig; break;
      case 41: _soundPlayer.Stream = Properties.Resources.Ow___Sheep; break;
      case 42: _soundPlayer.Stream = Properties.Resources.Ow___Sneeze; break;
      case 43: _soundPlayer.Stream = Properties.Resources.Ow___Throat; break;
      case 44: _soundPlayer.Stream = Properties.Resources.Ow___Uh_oh; break;
      case 45: _soundPlayer.Stream = Properties.Resources.Ow___Vinni_Pooh; break;
    }

    SystemVolume = ushort.MaxValue / 52;
    _soundPlayer.Play();
    //too soopn - affects this play: SystemVolume = ushort.MaxValue / 2;
  }
  public void PlayStart()
  {
    switch (_rnd.Next(22))
    {
      case 0: _soundPlayer.Stream = Properties.Resources.Start_01; break;
      case 1: _soundPlayer.Stream = Properties.Resources.Start_02; break;
      case 2: _soundPlayer.Stream = Properties.Resources.Start_03; break;
      case 3: _soundPlayer.Stream = Properties.Resources.Start_04; break;
      case 4: _soundPlayer.Stream = Properties.Resources.Start_05; break;
      case 5: _soundPlayer.Stream = Properties.Resources.Start_06; break;
      case 6: _soundPlayer.Stream = Properties.Resources.Start___Ambient; break;
      case 7: _soundPlayer.Stream = Properties.Resources.Start___Arcade_Alarm; break;
      case 8: _soundPlayer.Stream = Properties.Resources.Start___Arcade_Chirp_Descend; break;
      case 9: _soundPlayer.Stream = Properties.Resources.Start___Arcade_Power_Up; break;
      case 10: _soundPlayer.Stream = Properties.Resources.Start___Bjaz; break;
      case 11: _soundPlayer.Stream = Properties.Resources.Start___Bulp; break;
      case 12: _soundPlayer.Stream = Properties.Resources.Start___Engine; break;
      case 13: _soundPlayer.Stream = Properties.Resources.Start___Gong; break;
      case 14: _soundPlayer.Stream = Properties.Resources.Start___Headshk; break;
      case 15: _soundPlayer.Stream = Properties.Resources.Start___Horse; break;
      case 16: _soundPlayer.Stream = Properties.Resources.Start___Let_s_Do_It; break;
      case 17: _soundPlayer.Stream = Properties.Resources.Start___Oops; break;
      case 18: _soundPlayer.Stream = Properties.Resources.Start___Oracle; break;
      case 19: _soundPlayer.Stream = Properties.Resources.Start___Sad_Guitar; break;
      case 20: _soundPlayer.Stream = Properties.Resources.Start___Spring; break;
      case 21: _soundPlayer.Stream = Properties.Resources.Start___Upper; break;
    }

    SystemVolume = ushort.MaxValue / 3;
    _soundPlayer.Play();
  }

  public ushort SystemVolume
  {
    get
    {
      _ = waveOutGetVolume(nint.Zero, out var dwVolume);

      return (ushort)(dwVolume & 0x0000ffff);
    }
    set
    {
      var newVolumeAllChannels = (uint)value & 0x0000ffff | (uint)value << 16; // Set the same volume for both the left and the right channels
      _ = waveOutSetVolume(nint.Zero, newVolumeAllChannels);
    }
  }

  [DllImport("winmm.dll")] public static extern int waveOutGetVolume(nint hwo, out uint dwVolume);
  [DllImport("winmm.dll")] public static extern int waveOutSetVolume(nint hwo, uint dwVolume);
}
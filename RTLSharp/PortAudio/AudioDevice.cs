///////////////////////////////////////////////////////////////////////////////////
///    C# RTLSDR                                                                ///
///    Copyright(C) 2016 Lucas Teske                                            ///
///                                                                             ///
///    This program is free software: you can redistribute it and/or modify     ///
///    it under the terms of the GNU General Public License as published by     ///
///    the Free Software Foundation, either version 3 of the License, or        ///
///    any later version.                                                       ///
///                                                                             ///
///    This program is distributed in the hope that it will be useful,          ///
///    but WITHOUT ANY WARRANTY; without even the implied warranty of           ///
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              ///
///    GNU General Public License for more details.                             ///
///                                                                             ///
///    You should have received a copy of the GNU General Public license        ///
///    along with this program.If not, see<http://www.gnu.org/licenses/>.       ///
///////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using static RTLSharp.PortAudio.PortAudio;

namespace RTLSharp.PortAudio {
  /// <summary>
  /// PortAudio Device Data
  /// </summary>
  public class AudioDevice {
    #region Static Helper Methods
    public static List<AudioDevice> GetDevices(AudioDeviceDirection direction) {
      List<AudioDevice> audioDevices = new List<AudioDevice>();
      int count = Pa_GetDeviceCount();
      int defaultIn = Pa_GetDefaultInputDevice();
      int defaultOut = Pa_GetDefaultOutputDevice();

      for (int i = 0; i < count; i++) {
        PaDeviceInfo deviceInfo = Pa_GetDeviceInfo(i);
        AudioDeviceDirection deviceDirection = deviceInfo.maxInputChannels > 0 ? (deviceInfo.maxOutputChannels > 0 ? AudioDeviceDirection.InputOutput : AudioDeviceDirection.Input) : AudioDeviceDirection.Output;
        if (deviceDirection == direction || deviceDirection == AudioDeviceDirection.InputOutput) {
          PaHostApiInfo hostInfo = Pa_GetHostApiInfo(deviceInfo.hostApi);
          audioDevices.Add(new AudioDevice(deviceInfo.name, hostInfo.name, i, deviceDirection, i == defaultIn || i == defaultOut));
        }
      }

      return audioDevices;
    }
    #endregion
    #region Constructor
    public AudioDevice() {}
    public AudioDevice(string name, string host, int idx, AudioDeviceDirection direction, bool isDefault) {
      this.Name = name;
      this.Host = host;
      this.Index = idx;
      this.Direction = direction;
      this.IsDefault = isDefault;
    }
    #endregion
    #region Methods
    public override string ToString() {
      return "[" + Host + "] " + Name;
    }
#endregion
    #region Properties
    public int Index { get; set; }
    public string Name { get; set; }
    public string Host { get; set; }
    public AudioDeviceDirection Direction { get; set; }
    public bool IsDefault { get; set; }
    #endregion
  }
}

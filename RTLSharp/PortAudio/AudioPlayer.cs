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

using RTLSharp.Callbacks;
using System;
using System.Runtime.InteropServices;
using static RTLSharp.Callbacks.ManagedCallbacks;
using static RTLSharp.PortAudio.PortAudio;

namespace RTLSharp.PortAudio {
  /// <summary>
  /// Simple Audio Player using PortAudio
  /// </summary>
  public class AudioPlayer {
    #region Fields
    private IntPtr _streamHandle;
    private GCHandle _gcHandle;
    private readonly SamplesAvailableEvent _bufferNeeded;
    private unsafe readonly PaStreamCallbackDelegate _paCallback = PaStreamCallback;
    #endregion
    #region Constructors
    public AudioPlayer(int deviceIndex, double sampleRate, uint framesPerBuffer, SamplesAvailableEvent samplesAvailableCallback) {
      _bufferNeeded = samplesAvailableCallback;

      PaStreamParameters ouputParams = new PaStreamParameters();
      ouputParams.device = deviceIndex;
      ouputParams.channelCount = 2;
      ouputParams.suggestedLatency = 0;
      ouputParams.sampleFormat = PaSampleFormat.paFloat32;

      PaError paErr = Pa_IsFormatSupported(IntPtr.Zero, ref ouputParams, sampleRate);
      if (paErr != PaError.paNoError) {
        throw new ApplicationException(paErr.ToString());
      }

      _gcHandle = GCHandle.Alloc(this);

      paErr = Pa_OpenStream(out _streamHandle, IntPtr.Zero, ref ouputParams, sampleRate, framesPerBuffer, PaStreamFlags.paNoFlag, _paCallback, (IntPtr)_gcHandle);

      if (paErr != PaError.paNoError) {
        _gcHandle.Free();
        throw new ApplicationException(paErr.ToString());
      }

      paErr = Pa_StartStream(_streamHandle);
      if (paErr != PaError.paNoError) {
        Pa_CloseStream(_streamHandle);
        _gcHandle.Free();
        throw new ApplicationException(paErr.ToString());
      }
    }
    #endregion
    #region Methods
    public void Dispose() {
      if (_streamHandle != IntPtr.Zero) {
        Pa_StopStream(_streamHandle);
        Pa_CloseStream(_streamHandle);
        _streamHandle = IntPtr.Zero;
      }
      _gcHandle.Free();
    }
    #endregion
    #region Private Methods
    private unsafe static PaStreamCallbackResult PaStreamCallback(float* input, float* output, uint frameCount, ref PaStreamCallbackTimeInfo timeInfo, PaStreamCallbackFlags statusFlags, IntPtr userData) {
      GCHandle gcHandle = GCHandle.FromIntPtr(userData);
      if (!gcHandle.IsAllocated) {
        return PaStreamCallbackResult.paAbort;
      }
      AudioPlayer instance = (AudioPlayer)gcHandle.Target;

      try {
        instance._bufferNeeded?.Invoke(new SamplesEventArgs(output, (int)frameCount));
      } catch {
        return PaStreamCallbackResult.paAbort;
      }

      return PaStreamCallbackResult.paContinue;
    }
    #endregion
  }
}

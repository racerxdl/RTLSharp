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

using RTLSharp.Types;
using System;
using System.Runtime.InteropServices;
using System.Text;
using static RTLSharp.Callbacks.NativeCallbacks;

namespace RTLSharp {
  /// <summary>
  /// librtlsdr Native Methods
  /// </summary>
  public class NativeMethods {
    private const string LibRtlSdr = "librtlsdr\\librtlsdr.dll";
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_cancel_async(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_close(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern uint rtlsdr_get_center_freq(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern uint rtlsdr_get_device_count();
      public static string rtlsdr_get_device_name(uint index) => Marshal.PtrToStringAnsi(rtlsdr_get_device_name_native(index));
      [DllImport(LibRtlSdr, EntryPoint = "rtlsdr_get_device_name", CallingConvention = CallingConvention.Cdecl)]
      private static extern IntPtr rtlsdr_get_device_name_native(uint index);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_device_usb_strings(uint index, StringBuilder manufact, StringBuilder product, StringBuilder serial);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_freq_correction(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern uint rtlsdr_get_sample_rate(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_tuner_gain(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_tuner_gains(IntPtr dev, [In, Out] int[] gains);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern TunerType rtlsdr_get_tuner_type(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_usb_strings(IntPtr dev, StringBuilder manufact, StringBuilder product, StringBuilder serial);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_get_xtal_freq(IntPtr dev, out uint rtlFreq, out uint tunerFreq);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_open(out IntPtr dev, uint index);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_read_async(IntPtr dev, RtlSdrReadAsyncCallback cb, IntPtr ctx, uint bufNum, uint bufLen);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_read_sync(IntPtr dev, IntPtr buf, int len, out int nRead);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_reset_buffer(IntPtr dev);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_agc_mode(IntPtr dev, int on);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_center_freq(IntPtr dev, uint freq);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_direct_sampling(IntPtr dev, int on);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_freq_correction(IntPtr dev, int ppm);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_offset_tuning(IntPtr dev, int on);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_sample_rate(IntPtr dev, uint rate);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_testmode(IntPtr dev, int on);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_tuner_gain(IntPtr dev, int gain);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_tuner_gain_ext(IntPtr dev, int lna_gain, int mixer_gain, int vga_gain);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_tuner_gain_mode(IntPtr dev, int manual);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_set_xtal_freq(IntPtr dev, uint rtlFreq, uint tunerFreq);
      [DllImport(LibRtlSdr, CallingConvention = CallingConvention.Cdecl)]
      public static extern int rtlsdr_wait_async(IntPtr dev, RtlSdrReadAsyncCallback cb, IntPtr ctx);
    }
}

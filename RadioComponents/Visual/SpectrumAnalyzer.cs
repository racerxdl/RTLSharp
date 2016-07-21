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

using RTLSharp.Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RadioComponents.Visual {
  /// <summary>
  /// Spectrum Analyzer Component for Windows Forms
  /// <para>Based on SDRSharp Component</para>
  /// </summary>
  public class SpectrumAnalyzer : UserControl, IComponent {
    #region Fields
    private Timer _workTimer = new Timer();
    private Graphics _graphics;
    private Bitmap _bgBuffer;
    private Bitmap _buffer;
    private byte[] _spectrumData;
    private byte[] _spectrumDataMin;
    private float[] _spectrumEnvelope;
    private float[] _temp;
    private Point[] _envelopePoints;
    private float _scale = 1f;
    private bool spectrumAvailable = true;
    private int _displayOffset = 0;
    private int _displayRange = 130;
    private long _centerFrequency;
    private long _spectrumWidth;
    private LinearGradientBrush _gradientBrush;
    #endregion
    #region Constructors
    public SpectrumAnalyzer() {
      _workTimer.Interval = 50;
      _workTimer.Tick += _workTimer_Tick;
      _workTimer.Enabled = true;

      _bgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
      _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
      _gradientBrush = new LinearGradientBrush(new Rectangle(30, 30, _buffer.Width - 30, _buffer.Height - 30), Color.White, Color.Black, LinearGradientMode.Vertical);

      _graphics = Graphics.FromImage(_buffer);

      int points = _buffer.Width - 60;
      _spectrumData = new byte[points];
      _spectrumEnvelope = new float[points];
      _temp = new float[points];
      _spectrumDataMin = new byte[points];
      _envelopePoints = new Point[points + 2];

      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      UpdateStyles();
    }
    #endregion
    #region Methods
    public unsafe void RenderSpectrum(float* spectrumData, int length) {
      updateSpectrum(spectrumData, length, _scale, 0);
    }
    #endregion
    #region Protected Methods
    protected override void OnPaint(PaintEventArgs e) {
      if ((ClientRectangle.Width <= 60) || (ClientRectangle.Height <= 60)) {
        e.Graphics.Clear(Color.Red);
      } else {
        ConfigureGraphics(e.Graphics);
        e.Graphics.DrawImageUnscaled(_bgBuffer, 0, 0);
        e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
      }
    }
    protected override void OnResize(EventArgs e) {
      base.OnResize(e);
      if ((ClientRectangle.Width > 60) && (ClientRectangle.Height > 60)) {
        _buffer.Dispose();
        _graphics.Dispose();

        _bgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
        _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
        _gradientBrush = new LinearGradientBrush(new Rectangle(30, 30, _buffer.Width - 30, _buffer.Height - 30), Color.White, Color.Black, LinearGradientMode.Vertical);
        _graphics = Graphics.FromImage(_buffer);

        ConfigureGraphics(this._graphics);
        int num = this._buffer.Width - 60;
        int points = _buffer.Width - 60;
        _spectrumData = new byte[points];
        _spectrumEnvelope = new float[points];
        _temp = new float[points];
        _spectrumDataMin = new byte[points];
        _envelopePoints = new Point[points + 2];
        spectrumAvailable = true;
        updateBackground();
      }
    }
    #endregion
    #region Private Methods
    private static void ConfigureGraphics(Graphics graphics) {
      graphics.CompositingMode = CompositingMode.SourceOver;
      graphics.CompositingQuality = CompositingQuality.HighSpeed;
      graphics.SmoothingMode = SmoothingMode.None;
      graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
      graphics.InterpolationMode = InterpolationMode.Low;
    }
    private void _workTimer_Tick(object sender, EventArgs e) {
      lock (_spectrumData) {
        if (spectrumAvailable) {
          _graphics.Clear(Color.Transparent);
          float lScale = ((float)(_buffer.Width - 60)) / _spectrumData.Length;
          float lScale2 = (_buffer.Height - 60) / 255f;

          for (int m = 0; m < _spectrumData.Length; m++) {
            byte power = _spectrumData[m];
            int x = (int)(30f + (m * lScale));
            int y = (_buffer.Height - 30) - ((int)(power * lScale2));
            _envelopePoints[m + 1].X = x;
            _envelopePoints[m + 1].Y = y;
          }

          _envelopePoints[0].X = 30;
          _envelopePoints[0].Y = (_buffer.Height - 30) + 1;
          _envelopePoints[_envelopePoints.Length - 1].X = _buffer.Width - 30;
          _envelopePoints[_envelopePoints.Length - 1].Y = (_buffer.Height - 30) + 1;
          _graphics.FillPolygon(_gradientBrush, _envelopePoints);
          Invalidate();
          spectrumAvailable = false;
        }
      }
    }
    private string getFrequencyDisplay(long frequency) {
      return string.Format("{0,0000}", frequency / 1e6);
    }
    private void updateBackground() {
      Graphics graphics = Graphics.FromImage(_bgBuffer);
      ConfigureGraphics(graphics);
      graphics.Clear(Color.Black);

      Pen pen = new Pen(Color.FromArgb(80, 80, 80));
      Pen pen2 = new Pen(Color.DarkGray);
      Font font = new Font("Arial", 8f);
      SolidBrush brush = new SolidBrush(Color.Silver);
      pen.DashStyle = DashStyle.Dash;

      // Draw Vertical
      int position = 10;
      long finalFreqStrSize;
      string frequencyDisplay = getFrequencyDisplay(this._centerFrequency + ((long)(_spectrumWidth * 0.5f)));
      float freqStrSize = graphics.MeasureString(frequencyDisplay, font).Width + 30f;
      long freqStrSize2 = (long)((_buffer.Width - 60) / freqStrSize);
      int i = 2;

      do {
        i = (i == 2) ? 5 : 2;
        position *= i;
        finalFreqStrSize = ((int)((_spectrumWidth) / _scale)) / position;
      }

      while (finalFreqStrSize > freqStrSize2);
      if (finalFreqStrSize > 0L) {
        if ((finalFreqStrSize * 5L) < freqStrSize2) {
          finalFreqStrSize *= 5L;
          position /= 5;
        }
        if ((finalFreqStrSize * 2L) < freqStrSize2) {
          position /= 2;
        }
      }

      finalFreqStrSize = freqStrSize2 * 2L;
      float steps = (((_buffer.Width - 60) * position) * _scale) / (_spectrumWidth);
      float delta = (((_centerFrequency % (position)) * (_buffer.Width - 60)) * _scale) / (_spectrumWidth);
      for (long k = -finalFreqStrSize / 2L; k < (finalFreqStrSize / 2L); k += 1L) {
        float pos = ((((_buffer.Width - 60) * 0.5f) + 30f) + (steps * k)) - delta;
        if ((pos >= 30f) && (pos <= (_buffer.Width - 30))) {
          graphics.DrawLine(pen, pos, 30f, pos, _buffer.Height - 30);
          graphics.DrawLine(pen2, pos, _buffer.Height - 30, pos, _buffer.Height - 25);
          string text = getFrequencyDisplay((_centerFrequency + (k * position)) - (_centerFrequency % position));
          float width = graphics.MeasureString(text, font).Width;
          pos -= width * 0.5f;
          graphics.DrawString(text, font, brush, pos, (_buffer.Height - 30) + 8f);
        }
      }

      // Draw Horizontal

      int height = (int)graphics.MeasureString("100", font).Height;
      int scaleDisplayRange = 1;
      int displayRange = _displayRange;
      int drawHeight = _buffer.Height - 60;
      if (drawHeight < (height * displayRange)) {
        scaleDisplayRange = 5;
        displayRange = _displayRange / scaleDisplayRange;
      }
      if ((drawHeight / displayRange) < height) {
        scaleDisplayRange = 10;
        displayRange = _displayRange / scaleDisplayRange;
      }
      float range = (_buffer.Height - 60f) / displayRange;
      for (i = 1; i <= displayRange; i++) {
        graphics.DrawLine(pen, 30, (_buffer.Height - 30) - ((int)(i * range)), _buffer.Width - 30, (_buffer.Height - 30) - ((int)(i * range)));
      }

      int offsetStep = (_displayOffset / 10) * 10;
      for (int j = 0; j <= displayRange; j++) {
        string str3 = (offsetStep - ((displayRange - j) * scaleDisplayRange)).ToString();
        SizeF ef3 = graphics.MeasureString(str3, font);
        graphics.DrawString(str3, font, brush, (30f - ef3.Width), (((_buffer.Height - 30) - (j * range)) - (ef3.Height * 0.5f)));
      }

      using (pen = new Pen(Color.DarkGray)) {
        graphics.DrawLine(pen, 30, 30, 30, _buffer.Height - 30);
        graphics.DrawLine(pen, 30, _buffer.Height - 30, _buffer.Width - 30, _buffer.Height - 30);
      }
    }
    private unsafe void updateSpectrum(float* spectrumData, int length, float scale, float offset) {
      int displayOffset = (_displayOffset / 10) * 10;
      int displayRange = (_displayRange / 10) * 10;

      fixed (float* tempRef = _temp)
      {
        fixed (float* smoothedSpectrumEnvelopeRef = _spectrumEnvelope)
        {
          fixed (byte* spectrumDataRef = _spectrumData)
          {
            FFT.Smooth(spectrumData, tempRef, length, _spectrumEnvelope.Length, scale, offset);
            Utils.memcpy((void*)smoothedSpectrumEnvelopeRef, (void*)tempRef, _spectrumEnvelope.Length * 4);
            FFT.Scale(smoothedSpectrumEnvelopeRef, spectrumDataRef, _spectrumEnvelope.Length, (displayOffset - displayRange), displayOffset);
          }
        }
      }
      spectrumAvailable = true;
    }
    #endregion
    #region Properties
    public int DisplayOffset {
      get { return _displayOffset; }
      set {
        if (_displayOffset != value) {
          _displayOffset = value;
          updateBackground();
        }
      }
    }
    public int DisplayRange {
      get { return _displayRange; }
      set {
        if (_displayRange != value) {
          _displayRange = value;
          updateBackground();
        }
      }
    }
    public long Frequency {
      get { return _centerFrequency; }
      set {
        if (_centerFrequency != value) {
          _centerFrequency = value;
          updateBackground();
          spectrumAvailable = true;
        }
      }
    }
    public long SampleRate {
      get { return _spectrumWidth; }
      set {
        if (_spectrumWidth != value) {
          _spectrumWidth = value;
          updateBackground();
        }
      }
    }
    public float SpectrumScale {
      get { return _scale; }
      set {
        if (_scale != value) {
          _scale = value;
          updateBackground();
        }
      }
    }
    #endregion
  }
}

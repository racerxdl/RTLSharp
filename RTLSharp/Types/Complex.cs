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

using System;
using System.Runtime.InteropServices;

namespace RTLSharp.Types {
  /// <summary>
  /// Complex Number Representation as Struct
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Complex {
    #region Fields
    public float real;
    public float imag;
    #endregion
    #region Constructors
    public Complex(float real, float imag) {
      this.real = real;
      this.imag = imag;
    }
    public Complex(Complex c) {
      real = c.real;
      imag = c.imag;
    }
    #endregion
    #region Methods
    public float Modulus() => (float)Math.Sqrt(ModulusSquared());
    public float ModulusSquared() => (real * real) + (imag * imag);
    public float Argument() => (float)Math.Atan2(imag, real);
    public Complex Conjugate() => new Complex(real, -imag);
    public Complex Normalize() => (this * (1f / Modulus()));
    public override string ToString() => string.Format("{0} + {1}i", real, imag);
    public static Complex FromAngle(double angle) => new Complex((float)Math.Cos(angle), (float)Math.Sin(angle));
    public override int GetHashCode() => real.GetHashCode() * 211 + imag.GetHashCode();
    public bool Equals(Complex obj) => this == obj;
    public override bool Equals(object obj) => (obj.GetType() == typeof(Complex)) && Equals((Complex)obj);
    #endregion
    #region Operators
    public static bool operator ==(Complex a, Complex b) => a.imag == b.imag && a.real == b.real;
    public static bool operator !=(Complex a, Complex b) => !(a == b);
    public static Complex operator +(Complex a, Complex b) => new Complex(a.real + b.real, a.imag + b.imag);
    public static Complex operator -(Complex a, Complex b) => new Complex(a.real - b.real, a.imag - b.imag);
    public static Complex operator *(Complex a, Complex b) => new Complex( (a.real * b.real) - (a.imag * b.imag), (a.imag * b.real) + (a.real * b.imag) );
    public static Complex operator *(Complex a, float b) => new Complex(a.real * b, a.imag * b);
    public static Complex operator /(Complex a, Complex b) {
      float num = 1f / ((b.real * b.real) + (b.imag * b.imag));
      return new Complex(((a.real * b.real) + (a.imag * b.imag)) * num, ((a.imag * b.real) - (a.real * b.imag)) * num);
    }
    public static Complex operator /(Complex a, float b) {
      b = 1f / b;
      return new Complex(a.real * b, a.imag * b);
    }
    public static Complex operator ~(Complex a) => a.Conjugate();
    public static implicit operator Complex(float d) => new Complex(d, 0f);
    #endregion
  }
}

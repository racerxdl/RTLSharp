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
using System.Threading;

namespace RTLSharp.Types {
  public class Locker {
    #region Fields
    private bool _state;
    private bool _waiting;
    #endregion
    #region Constructors / Destructors
    public Locker() : this(false) { }
    public Locker(bool initialState) {
      _state = initialState;
    }
    ~Locker() {
      Dispose();
    }
    #endregion
    #region Methods
    public void Dispose() {
      Unlock();
      GC.SuppressFinalize(this);
    }
    public void Reset() {
      lock (this) {
        _state = false;
      }
    }
    public void Wait() {
      lock (this) {
        if (!_state) {
          _waiting = true;
          try {
            Monitor.Wait(this);
          } finally {
            _waiting = false;
          }
        }
        _state = false;
      }
    }
    public void Unlock() {
      lock (this) {
        _state = true;
        if (_waiting) {
          Monitor.Pulse(this);
        }
      }
    }
    #endregion
  }
}

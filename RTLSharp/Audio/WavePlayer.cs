using RTLSharp.Base;
using System;
using System.Timers;
using RTLSharp.Base.Callbacks;
using RTLSharp.Types;
using System.Diagnostics;
using System.Threading;

namespace RTLSharp.Audio {
  public class WavePlayer: Frontend {

    private static readonly uint samplingPeriod = 1000 / 60; // In ms

    private Thread _worker;
    private Wave _wave;
    private uint _readSamples;
    private DataBuffer _complexBuffer;
    private unsafe Complex* _complexArray;
    private SamplesEventArgs _eventArgs;
    private int _workerInterval;
    private bool _workerRunning = false;

    public event ManagedCallbacks.SamplesAvailableEvent SamplesAvailable;

    public WavePlayer(Wave wav) {
      init(wav, (wav.SampleRate / 1000) * samplingPeriod);
    }

    public WavePlayer(Wave wav, uint readSamples) {
      init(wav, readSamples);
    }

    private unsafe void init(Wave wav, uint readSamples) {
      _wave = wav;
      _readSamples = readSamples > 0 ? readSamples : 1;
      _complexBuffer = DataBuffer.Create((int)readSamples, sizeof(Complex));
      _complexArray = (Complex*)_complexBuffer;

      _eventArgs = new SamplesEventArgs();
      _eventArgs.IsComplex = true;
      _eventArgs.Length = (int)readSamples;
      _eventArgs.ComplexBuffer = _complexArray;
      
      _workerInterval = (int) ( (_readSamples * 1000) / wav.SampleRate );
    }

    private unsafe void WorkerMethod() {
      while (_workerRunning) {
        if (SamplesAvailable != null) {
          _wave.Data.readComplexes(_complexArray, _readSamples);
          SamplesAvailable(_eventArgs);
          runCount++;
          c();
        }

        Thread.Sleep(_workerInterval);
      }

      Console.WriteLine("Stopping WavePlayer Thread");
    }

      Stopwatch sw = Stopwatch.StartNew();
    uint runCount = 0;


    public void c() {
      if (runCount == 100) {
        sw.Stop();
        Console.WriteLine("Avg Time: {0}", sw.ElapsedMilliseconds / runCount);
        runCount = 0;
        sw.Reset();
        sw.Start();
      }

      if (!sw.IsRunning) {
        sw.Start();
      }
    }

    public string Name {
      get { return "RTLSharp Wave Player"; }
    }
  
    public uint Frequency {
      get { return _wave.Frequency; }
      set { }
    }

    public uint SampleRate {
      get { return _wave.SampleRate; }
    }

    public void Start() {
      Console.WriteLine("Starting playing Wave!");
      _wave.Data.Close();
      _wave.Data.Open();
      _worker = new Thread(new ThreadStart(WorkerMethod));
      _worker.Priority = ThreadPriority.Highest;
      _workerRunning = true;
      _worker.Start();
    }

    public void Stop() {
      _workerRunning = false;
      _worker.Join();
      _worker = null;
      _wave.Data.Close();
    }
  }
}

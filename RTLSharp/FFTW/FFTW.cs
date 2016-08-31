namespace RTLSharp.FFTW {
  public class FFTW {
    public const int FFTW_FORWARD = (-1);
    public const int FFTW_BACKWARD = (+1);

    public const double FFTW_NO_TIMELIMIT = (-1.0);

    public const uint FFTW_MEASURE = (0U);
    public const uint FFTW_DESTROY_INPUT = (1 << 0);
    public const uint FFTW_UNALIGNED = (1 << 1);
    public const uint FFTW_CONSERVE_MEMORY = (1 << 2);
    public const uint FFTW_EXHAUSTIVE = (1 << 3);
    public const uint FFTW_PRESERVE_INPUT = (1 << 4);
    public const uint FFTW_PATIENT = (1 << 5);
    public const uint FFTW_ESTIMATE = (1 << 6);
    public const uint FFTW_WISDOM_ONLY = (1 << 21);
  }
}

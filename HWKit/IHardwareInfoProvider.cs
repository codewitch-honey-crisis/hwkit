namespace HWKit
{
    public sealed class PublishReadingEventArgs
    {
        public PublishReadingEventArgs(string path, Func<float> getter)
        {
            Path = path;
            Getter = getter;
        }
        public string Path { get; }
        public Func<float> Getter { get; }
    }
    public delegate void PublishReadingEventHandler(object sender, PublishReadingEventArgs e);
    public sealed class RevokeReadingEventArgs
    {
        public RevokeReadingEventArgs(string path)
        {
            Path = path;
        }
        public string Path { get; }
    }
    public delegate void RevokeReadingEventHandler(object sender, RevokeReadingEventArgs e);
    public interface IHardwareInfoProvider : IDisposable
    {
        event PublishReadingEventHandler PublishReading;
        event RevokeReadingEventHandler RevokeReading;
        uint RefreshInterval { get; set; }
        void Start();
        void Stop();
    }
}

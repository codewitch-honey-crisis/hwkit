namespace HWKit
{
    public class HardwareInfoProviderBase : IHardwareInfoProvider
    {
        private bool disposedValue;

        public event PublishReadingEventHandler? PublishReading;
        public event RevokeReadingEventHandler? RevokeReading;

        protected virtual uint GetRefreshInterval()
        {
            return 0;
        }
        protected virtual void SetRefreshInterval(uint value)
        {

        }
        public uint RefreshInterval
        {
            get { return GetRefreshInterval(); }
            set { SetRefreshInterval(value); }
        }
        protected virtual void OnStart()
        {

        }
        protected virtual void OnStop()
        {

        }
        protected virtual void Publish(string path, Func<float> getter)
        {
            PublishReading?.Invoke(this, new PublishReadingEventArgs(path, getter));
        }
        protected virtual void Revoke(string path)
        {
            RevokeReading?.Invoke(this, new RevokeReadingEventArgs(path));
        }
        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HardwareInfoProviderBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            // GC.SuppressFinalize(this);
        }
    }
}

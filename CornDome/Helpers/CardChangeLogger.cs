using CornDome.Repository;

namespace CornDome.Helpers
{
    public class CardChangeLogger(Config config) : ILogger, ICardChangeLogger
    {
        private static readonly object _lock = new();

        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
                                TState state, Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(config.AppData.CardModifiedLogs, $"{DateTime.Now}: {formatter(state, exception)}{Environment.NewLine}");
                }
            }
        }

        public void LogCardChange(string message)
        {
            Log(LogLevel.Information, eventId: new EventId(1000), state: "", formatter: (state, ex) => message, exception: null);
        }
    }
}

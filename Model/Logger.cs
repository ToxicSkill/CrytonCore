using CrytonCore.Enums;
using CrytonCore.Interfaces;

namespace CrytonCore.Model
{
    public class Logger : ILogger
    {
        public ELogger.Level LogLevel { get; private set; } = ELogger.Level.Information;
        public string Message { get; private set; }

        public Logger() : this("Unrecognized error", ELogger.Level.Error) { }

        public Logger(string message, ELogger.Level level)
        {
            SetMessage(message);
            SetLevel(level);
        }

        public string GetLevelName()
        {
            return ELogger.EnumToString(LogLevel);
        }

        public Logger GetLogger()
        {
            return this;
        }

        public string GetMessage()
        {
            return Message;
        }

        public void SetLevel(ELogger.Level level)
        {
            LogLevel = level;
        }

        public void SetMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            Message = message;
        }

        ELogger.Level ILogger.GetLevel()
        {
            return LogLevel;
        }
    }
}

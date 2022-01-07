using CrytonCore.Enums;
using CrytonCore.Model;

namespace CrytonCore.Interfaces
{
    internal interface ILogger
    {
        Logger GetLogger();

        void SetMessage(string message);

        void SetLevel(ELogger.Level level);

        string GetLevelName();

        ELogger.Level GetLevel();

        string GetMessage();
    }
}

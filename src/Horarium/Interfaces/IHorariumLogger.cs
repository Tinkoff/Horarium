using System;

namespace Horarium.Interfaces
{
    public interface IHorariumLogger
    {
        void Debug(string msg);

        void Debug(Exception ex);

        void Error(Exception ex);

        void Error(string message, Exception ex);
    }
}
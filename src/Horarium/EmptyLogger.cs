using System;
using Horarium.Interfaces;

namespace Horarium
{
    public class EmptyLogger : IHorariumLogger
    {
        public void Debug(string msg)
        {
            
        }

        public void Debug(Exception ex)
        {

        }

        public void Error(Exception ex)
        {
            
        }

        public void Error(string message, Exception ex)
        {

        }
    }
}
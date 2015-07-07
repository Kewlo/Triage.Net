using System;

namespace Triage.Api.Domain
{
    public abstract class Event
    {
        protected Event()
        {
            Date = new DateTime();
        }
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public class Measure : Event
    {
        public double Value { get; set; }
    }

    public class Message : Event
    {
        public virtual MessageType Type { get; set; }
        public dynamic Data { get; set; }
    }

    public class ErrorMessage : Message
    {
        public override MessageType Type { get { return MessageType.Error;} }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string StackTrace { get; set; }
        public dynamic Context { get; set; }

    }

}
using System;

namespace Triage.Api.Domain
{
    public abstract class Event
    {
        protected Event()
        {
            Date = DateTime.Now;
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
        public decimal Value { get; set; }
    }

    public class MeasureSummary
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Hour { get; set; }
        public decimal Sum { get; set; }
        public int Count { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Average { get; set; }
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
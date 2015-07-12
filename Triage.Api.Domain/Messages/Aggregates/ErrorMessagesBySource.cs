using System;

namespace Triage.Api.Domain.Messages.Aggregates
{
    public class ErrorMessagesBySource
    {
        public string LastestErrorId { get; set; }
        public string Title { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
        public int Hour { get; set; }
        public int Count { get; set; }
    }
}
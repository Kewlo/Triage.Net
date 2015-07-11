using System;

namespace Triage.Api.Domain.Messages
{
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
}
using System.Linq;
using Raven.Client.Indexes;
using Triage.Api.Domain;
using Triage.Api.Domain.Diagnostics;
using Triage.Api.Domain.Messages;

namespace Triage.Persistence.Indexes
{
    public class MeasureSummaryIndex : AbstractIndexCreationTask<Measure, MeasureSummary>
    {
        public MeasureSummaryIndex()
        {
            Map = docs => from doc in docs
                select new
                {
                    doc.Title,
                    Date = doc.Date.Date,
                    Hour = doc.Date.Hour,
                    Sum = doc.Value,
                    Min = doc.Value,
                    Max = doc.Value,
                    Average = 0,
                    Count = 1
                };

            Reduce = results => from result in results
                group result by new
                {
                    result.Title,
                    result.Date,
                    result.Hour
                }
                into agg
				let count = agg.Sum(x => x.Count)
                let min = agg.Min(x => x.Min)
                let max = agg.Max(x => x.Max)
                let sum = agg.Sum(x => x.Sum)
                select new
                {
                    agg.Key.Title,
                    agg.Key.Date,
                    agg.Key.Hour,
                    Sum = sum,
                    Min = min,
                    Max = max,
                    Average = sum / count,
                    Count = count
                };
        }
    }
}
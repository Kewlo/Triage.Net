using System.Linq;
using Raven.Client.Indexes;
using Triage.Api.Domain.Messages;
using Triage.Api.Domain.Messages.Aggregates;

namespace Triage.Persistence.Indexes
{
    public class ErrorMessagesBySourceIndex : AbstractIndexCreationTask<ErrorMessage, ErrorMessagesBySource>
    {
        public ErrorMessagesBySourceIndex()
        {
            Map = docs => from doc in docs
                select new
                {
                    LastestErrorId = doc.Id,
                    doc.Title,
                    doc.StackTrace,
                    Date = doc.Date.Date,
                    Hour = doc.Date.Hour,
                    Count = 1
                };

            Reduce = results => from result in results
                group result by new
                {
                    result.StackTrace,
                    result.Date,
                    result.Hour
                }
                into agg
                let lastError = agg.Last()
                select new
                {
                    lastError.LastestErrorId,
                    lastError.Title,
                    agg.Key.StackTrace,
                    agg.Key.Date,
                    agg.Key.Hour,
                    Count = agg.Sum(x => x.Count)
                };
        }
    }
}
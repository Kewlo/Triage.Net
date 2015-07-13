using System;
using System.Collections.Generic;
using System.Linq;
using Triage.Api.Domain.Diagnostics;
using Triage.Api.Domain.Messages;
using Triage.Api.Domain.Messages.Aggregates;
using Triage.Business.Messages;
using Triage.Persistence.Context;
using Triage.Persistence.Indexes;

namespace Triage.Business
{
    public interface IEventLogBusiness
    {
        void LogError(ErrorMessage errorMessage);
        void LogWarning(Message message);
        void LogMessage(Message message);
        void LogMeasure(Measure measure);
        IList<Message> GetMessages();
        IList<Measure> GetMeasures();
        void Setup(int count);
        IEnumerable<MeasureSummary> GetSummary();
        IEnumerable<ErrorMessage> GetErrorMessages(DateTime? startDateTime = null, int top = 100);
        IEnumerable<ErrorMessagesBySource> GetErrorsBySource(DateTime? startDateTime = null, int? hour = null);
        void ClearErrorMessages();
    }


    public class EventLogBusiness: IEventLogBusiness
    {
        private readonly IDbContextFactory _dbContextFactory;

        public EventLogBusiness(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void LogError(ErrorMessage errorMessage)
        {
            errorMessage.Type = MessageType.Error;
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity((Message)errorMessage);
                dbContext.SaveChanges();
            }
        }

        public void LogWarning(Message message)
        {
            message.Type = MessageType.Warning;
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMessage(Message message)
        {
            message.Type = MessageType.Info;
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMeasure(Measure measure)
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(measure);
                dbContext.SaveChanges();
            }
        }

        public IList<Message> GetMessages()
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<Message>()
                    .ToList();
            }
        }

        public IList<Measure> GetMeasures()
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                var today = DateTime.Now.Date;
                return dbContext
                    .Query<Measure>()
                    .Where(x => x.Date.Date == today)
                    .Take(100)
                    .ToList();
            }
        }

        public void Setup(int count)
        {
            var random = new Random();

            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                for (int i = 0; i < count; i++)
                {

                    dbContext.AddEntity(new Measure
                    {
                        Title = "Test Measure 2",
                        Value = random.Next(1, 100)
                    });
                }

                dbContext.SaveChanges();
            }
        }
        
        public void ClearErrorMessages()
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                var messages = dbContext
                    .Query<ErrorMessage>()
                    .ToList();

                messages.ForEach(dbContext.DeleteEntity);

                dbContext.SaveChanges();
            }
        }

        public IEnumerable<ErrorMessage> GetErrorMessages(DateTime? startDateTime = null, int top = 100)
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<ErrorMessage>()
                    .If(startDateTime.HasValue, query => query.Where(error => startDateTime <= error.Date))
                    .OrderByDescending(x => x.Date)
                    .Take(top)
                    .ToList();
            }
        }

        public IEnumerable<ErrorMessagesBySource> GetErrorsBySource(DateTime? startDateTime = null, int? hour = null)
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<ErrorMessagesBySource, IErrorMessagesBySourceIndex>()
                    .If(startDateTime.HasValue, query => query.Where(error => startDateTime <= error.Date))
                    .If(hour.HasValue, query => query.Where(error => error.Hour == hour))
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.Count)
                    .ToList();
            }
        }

        public IEnumerable<MeasureSummary> GetSummary()
        {
            using (var dbContext = _dbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<MeasureSummary, IMeasureSummaryIndex>()
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }

        }
    }

    public static class QueryableExtensions
    {
        public static IQueryable<T> If<T>(this IQueryable<T> queryable, bool condition, Func<IQueryable<T>, IQueryable<T>> expression)
        {
            if (condition)
            {
                return expression(queryable);
            }

            return queryable;
        }
    }
}
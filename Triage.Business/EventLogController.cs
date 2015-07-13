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
    public interface IEventLogController
    {
        void LogError(ErrorMessage errorMessage);
        void LogWarning(Message message);
        void LogMessage(Message message);
        void LogMeasure(Measure measure);
        IList<Message> GetMessages();
        IList<Measure> GetMeasures();
        void Setup(int count);
        IEnumerable<MeasureSummary> GetSummary();
        IEnumerable<ErrorMessagesBySource> GetErrorsBySource();
        void ClearErrorMessages();
    }


    public class EventLogController: IEventLogController
    {
        private readonly ITriageDbContextFactory _triageDbContextFactory;
        private readonly ILogHub _logHub;

        public EventLogController(ITriageDbContextFactory triageDbContextFactory, ILogHub logHub)
        {
            _triageDbContextFactory = triageDbContextFactory;
            _logHub = logHub;
        }

        public void LogError(ErrorMessage errorMessage)
        {
            errorMessage.Type = MessageType.Error;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity((Message)errorMessage);
                dbContext.SaveChanges();

                _logHub.SendNewMessages(new [] { errorMessage});
            }
        }

        public void LogWarning(Message message)
        {
            message.Type = MessageType.Warning;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMessage(Message message)
        {
            message.Type = MessageType.Info;
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(message);
                dbContext.SaveChanges();
            }
        }

        public void LogMeasure(Measure measure)
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                dbContext.AddEntity(measure);
                dbContext.SaveChanges();
            }
        }

        public IList<Message> GetMessages()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<Message>()
                    .ToList();
            }
        }

        public IList<Measure> GetMeasures()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
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

            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
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
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                var messages = dbContext
                    .Query<ErrorMessage>()
                    .ToList();

                messages.ForEach(dbContext.DeleteEntity);

                dbContext.SaveChanges();
            }
        }

        public IEnumerable<ErrorMessagesBySource> GetErrorsBySource()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<ErrorMessagesBySource, IErrorMessagesBySourceIndex>()
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }

        }

        public IEnumerable<MeasureSummary> GetSummary()
        {
            using (var dbContext = _triageDbContextFactory.CreateTriageDbContext())
            {
                return dbContext
                    .Query<MeasureSummary, IMeasureSummaryIndex>()
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }

        }
    }

}
using System;
using System.Configuration;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Triage.Api.Domain;
using Triage.Api.Domain.Messages;
using Triage.Persistence.Indexes;

namespace Triage.Persistence.Context
{
    public interface IDbContext : IDisposable
    {
        IQueryable<T> Query<T>();
        void AddEntity<T>(T entity);
        void DeleteEnitity<T>(T entity);
    }

    public interface ITriageDbContext : IDbContext
    {
        IQueryable<TEntity> Query<TEntity, TIndex>() where TIndex : AbstractIndexCreationTask, new();
        void SaveChanges();
    }

    public class TriageDbContext : ITriageDbContext
    {
        internal IDocumentSession DocumentSession { get; private set; }

        internal TriageDbContext(IDocumentSession documentSession)
        {
            DocumentSession = documentSession;
        }

        public IQueryable<T> Query<T>()
        {
            return DocumentSession.Query<T>();
        }

        public IQueryable<TEntity> Query<TEntity, TIndex>() where TIndex : AbstractIndexCreationTask, new()
        {
            return DocumentSession.Query<TEntity, TIndex>();
        }

        public void AddEntity<T>(T entity)
        {
            DocumentSession.Store(entity);
        }

        public void DeleteEnitity<T>(T entity)
        {
            DocumentSession.Delete(entity);
        }

        public void SaveChanges()
        {
            DocumentSession.SaveChanges();
        }

        public void Dispose()
        {
            DocumentSession.Dispose();
        }
    }

    public interface ITriageDbContextFactory
    {
        ITriageDbContext CreateTriageDbContext();
    }

    public class TriageDbContextFactory : ITriageDbContextFactory
    {
        ~TriageDbContextFactory()
        {
            try
            {
                if(_documentStore.IsValueCreated)
                    _documentStore.Value.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private readonly Lazy<EmbeddableDocumentStore> _documentStore = new Lazy<EmbeddableDocumentStore>(InitializeDb);

        private static EmbeddableDocumentStore InitializeDb()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                DefaultDatabase = "Triage",
                DataDirectory = ConfigurationManager.AppSettings["Triage.RavenDbDirectory"] ?? "~/../Triage.Database"
            };
            documentStore.Initialize();

            documentStore.Conventions.FindTypeTagName = type =>
                type.IsSubclassOf(typeof (Message))
                    ? DocumentConvention.DefaultTypeTagName(typeof (Message))
                    : DocumentConvention.DefaultTypeTagName(type);


            IndexCreation.CreateIndexes(typeof(MeasureSummaryIndex).Assembly, documentStore);
            return documentStore;
        }

        public ITriageDbContext CreateTriageDbContext()
        {
            return new TriageDbContext(_documentStore.Value.OpenSession("Triage"));
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json.Utilities;
using Triage.Api.Domain.Messages;
using Triage.Persistence.Indexes;

namespace Triage.Persistence.Context
{
    public interface IDbContext : IDisposable
    {
        IQueryable<T> Query<T>();
        void AddEntity<T>(T entity);
        void DeleteEntity<T>(T entity);
    }

    public interface ITriageDbContext : IDbContext
    {
        IQueryable<TEntity> Query<TEntity, TIndex>() where TIndex : IDbIndex;
        void SaveChanges();
    }

    public interface IDbIndex
    {
    }

    public class TriageDbContext : ITriageDbContext
    {
        internal IDocumentSession DocumentSession { get; private set; }
        protected Dictionary<Type, IDbIndex> DbIndexes { get; set; }

        internal TriageDbContext(IDocumentSession documentSession, IEnumerable<IDbIndex> dbIndexes)
        {
            DocumentSession = documentSession;
            var dbIndexType = typeof (IDbIndex);
            DbIndexes = dbIndexes
                .SelectMany(index => index.GetType().GetAllInterfaces().Select(Interface => new { Interface, Index = index}))
                .Where(x => x.Interface != dbIndexType)
                .ToDictionary(x => x.Interface, x => x.Index);
        }

        public IQueryable<T> Query<T>()
        {
            return DocumentSession.Query<T>();
        }

        public IQueryable<TEntity> Query<TEntity, TIndex>() where TIndex : IDbIndex
        {
            var indexType = typeof (TIndex);
            if (DbIndexes.ContainsKey(indexType) == false)
            {
                throw new InvalidOperationException(string.Format("Could not resolve index '{0}'. Check dependency injection configuration", indexType.FullName));
            }

            var ravenIndex = DbIndexes[indexType] as AbstractIndexCreationTask;
            if (ravenIndex == null)
            {
                throw new InvalidOperationException(string.Format("Index '{0}' must inherit from Raven.Client.Indexes.AbstractIndexCreationTask", indexType.FullName));
            }

            return DocumentSession.Query<TEntity>(ravenIndex.IndexName);
        }

        public void AddEntity<T>(T entity)
        {
            DocumentSession.Store(entity);
        }

        public void DeleteEntity<T>(T entity)
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

    public interface IDbContextFactory
    {
        ITriageDbContext CreateTriageDbContext();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IEnumerable<IDbIndex> _dbIndexes;

        public DbContextFactory(IEnumerable<IDbIndex> dbIndexes)
        {
            _dbIndexes = dbIndexes;
        }

        ~DbContextFactory()
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
            return new TriageDbContext(_documentStore.Value.OpenSession("Triage"), _dbIndexes);
        }
    }

}

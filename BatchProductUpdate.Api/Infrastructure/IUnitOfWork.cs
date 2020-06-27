using System.Data;

namespace BatchProductUpdate.Api.Infrastructure
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }

        void Commit();

        void Rollback();
    }
}

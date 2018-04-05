using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebAppP2P.Core.Database.Queries
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    public interface IQueryHandler<TQuery> where TQuery : IQuery
    {
        void Handle(TQuery query);
    }
}

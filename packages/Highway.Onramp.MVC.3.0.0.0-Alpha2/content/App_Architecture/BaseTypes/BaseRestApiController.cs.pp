// [[Highway.Onramp.MVC]]
using Highway.Data;
using Highway.Data.PrebuiltQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace $rootnamespace$.App_Architecture.BaseTypes
{
    public abstract class BaseRestApiController<TId, TEntity> : BaseApiController
        where TId : struct, global::System.IEquatable<TId>
        where TEntity : class, IIdentifiable<TId>
    {
        protected IRepository repo;
        protected RestOperations ops;

        public BaseRestApiController(IRepository repo, RestOperations ops)
        {
            this.repo = repo;
            this.ops = ops;
        }

        [Flags]
        public enum RestOperations
        {
            GetAll = 1,
            GetOne = 2,
            Post = 4,
            Put = 8,
            Delete = 16,
            ReadOnly = 3,
            WriteOnly = 28,
            All = 31
        }


        protected virtual void CopyEntityValues(TEntity source, TEntity destination)
        {
        }

        public virtual IEnumerable<TEntity> Get()
        {
            if (ops.HasFlag(RestOperations.GetAll) == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            return repo.Find(new FindAll<TEntity>());
        }

        public virtual TEntity Get(TId id)
        {
            if (ops.HasFlag(RestOperations.GetOne) == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            return repo.Find(new GetById<TId, TEntity>(id));
        }

        public virtual void Post([FromBody]TEntity entity)
        {
            if (ops.HasFlag(RestOperations.Post) == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            repo.Context.Add(entity);
            repo.Context.Commit();
        }

        public virtual void Put(TId id, [FromBody]TEntity entity)
        {
            if (ops.HasFlag(RestOperations.Put) == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            var updateTarget = repo.Find(new GetById<TId, TEntity>(id));
            CopyEntityValues(entity, updateTarget);
            repo.Context.Commit();
        }

        public virtual void Delete(TId id)
        {
            if (ops.HasFlag(RestOperations.Delete) == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            var original = repo.Find(new GetById<TId, TEntity>(id));
            repo.Context.Remove(original);
            repo.Context.Commit();
        }
    }
}
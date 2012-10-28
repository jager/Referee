using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data;
using System.Web.Security;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using Referee.Helpers;

namespace Referee.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected RefereeContext db;
        internal DbSet<TEntity> dbSet;
        private string EntityName = String.Empty;

        public GenericRepository(RefereeContext context)
        {
            db = context;
            dbSet = context.Set<TEntity>();
            EntityName = typeof(TEntity).Name;
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                string IncludeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var incProp in IncludeProperties.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(incProp);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            db.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void UpdateProfile(TEntity newEntity, TEntity oldEntity)
        {
            db.Entry(oldEntity).CurrentValues.SetValues(newEntity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entity)
        {
            if (db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public bool UpdateUserNameAndEmail(string currentEmail, string newEmail)
        {
            try
            {
                MembershipUser memUser = Membership.GetUser(currentEmail);
                memUser.Email = newEmail;
                Membership.UpdateUser(memUser);
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE aspnet_Users SET UserName = @NewEmail, ");
                sb.Append("LoweredUserName = LOWER(@NewEmail) ");
                sb.Append("WHERE UserName = @CurrentEmail");

                ArrayList al = new ArrayList();

                al.Add(new SqlParameter("@CurrentEmail", currentEmail));
                al.Add(new SqlParameter("@NewEmail", newEmail));
                SqlParameter[] pArray = (SqlParameter[])al.ToArray(typeof(SqlParameter));
                return (db.Database.ExecuteSqlCommand(sb.ToString(), pArray) == 1);
            }
            catch (System.Configuration.Provider.ProviderException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /*
        private IQueryable<TEntity> PrepareWhere(IQueryable<TEntity> q)
        {
            IQueryable<TEntity> query = q;
            
            switch (EntityName)
            {
                case "League":
                    Func<Referee.Models.League, bool> filter = TEntity => TEntity.Visible;
                    query = q.Where(filter);
                    break;
                default:

                    break;
            }
            return query;
        }
         * */
    }
}
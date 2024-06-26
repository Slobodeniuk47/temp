﻿using Core.Entities;

namespace Core.Interfaces
{
    public interface IGenericRepository<TEntity, T> where TEntity : class, IEntity<T>
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetById(T id);
        Task<TEntity> GetByName(string name);

        Task Create(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(T id);
        Task Delete(TEntity entity);
    }
}

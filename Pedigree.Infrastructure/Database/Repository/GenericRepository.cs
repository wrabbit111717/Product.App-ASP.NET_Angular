﻿using Microsoft.EntityFrameworkCore;
using Pedigree.Core.Data;
using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pedigree.Core.Extensions;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>, IDisposable where TEntity: class, IEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> Total()
        {
            return await _dbContext.Set<TEntity>().CountAsync();
        }

        public async Task<IEnumerable<TEntity>> GetPaginated(int page, int size)
        {
            return await _dbContext.Set<TEntity>()
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TEntity> GetByIndex(int index)
        {
            return await _dbContext.Set<TEntity>()
                .Skip(index)
                .Take(1)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public async Task<int> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task Update(int id, TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            var entry = _dbContext.Entry(entity);
            entry.Property("CreatedAt").IsModified = false;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }

        public Task<IEnumerable<TEntity>> GetPaginatedWitTotal(int page, int size)
        {
            throw new NotImplementedException();
        }
    }
}

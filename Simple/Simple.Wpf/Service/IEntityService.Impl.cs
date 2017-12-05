﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simple.Wpf.Model;

namespace Simple.Wpf.Service
{
    internal class EntityService : IEntityService
    {
        public async Task DeleteAsync(int entityId)
        {
            using (var context = new MainDbContext())
            {
                context.Entities.Remove(context.Entities.Find(entityId));
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Entity>> AllAsync()
        {
            using (var context = new MainDbContext())
            {
                return await Task.Run(() => context.Entities.AsNoTracking().ToList());
            }
        }

        public async Task<Entity> SaveAsync(Entity entity)
        {
            using (var context = new MainDbContext())
            {
                var s = context.Entities.SingleOrDefault(e => e.Id == entity.Id) ?? context.Entities.Add(new Entity());
                s.Name = entity.Name;

                await context.SaveChangesAsync();
                context.Entry(s).State = System.Data.Entity.EntityState.Detached;

                return s;
            }
        }

        public async Task<bool> NameDuplicateAsync(int entityId, string entityName)
        {
            using (var context = new MainDbContext())
            {
                return await Task.Run(() => context.Entities.Any(e => e.Id != entityId && e.Name.ToUpperInvariant() == entityName.ToUpperInvariant()));
            }
        }
    }
}
using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    /// <summary>
    /// Generic repository implementation
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Lấy tất cả entities
        /// </summary>
        /// <returns>Danh sách entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Lấy entity theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity hoặc null</returns>
        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Tìm entities theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện tìm kiếm</param>
        /// <returns>Danh sách entities thỏa mãn điều kiện</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Thêm entity mới
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Entity đã được thêm</returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        /// <param name="entity">Entity cần cập nhật</param>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Xóa entity
        /// </summary>
        /// <param name="entity">Entity cần xóa</param>
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Xóa entity theo ID
        /// </summary>
        /// <param name="id">ID của entity cần xóa</param>
        public virtual async Task RemoveByIdAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Remove(entity);
            }
        }
    }
}

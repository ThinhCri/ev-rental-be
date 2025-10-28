using System.Linq.Expressions;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho generic repository
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Lấy tất cả entities
        /// </summary>
        /// <returns>Danh sách entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy entity theo ID
        /// </summary>
        /// <param name="id">ID của entity</param>
        /// <returns>Entity hoặc null</returns>
        Task<T?> GetByIdAsync(object id);

        /// <summary>
        /// Tìm entities theo điều kiện
        /// </summary>
        /// <param name="predicate">Điều kiện tìm kiếm</param>
        /// <returns>Danh sách entities thỏa mãn điều kiện</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Thêm entity mới
        /// </summary>
        /// <param name="entity">Entity cần thêm</param>
        /// <returns>Entity đã được thêm</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Cập nhật entity
        /// </summary>
        /// <param name="entity">Entity cần cập nhật</param>
        void Update(T entity);

        /// <summary>
        /// Xóa entity
        /// </summary>
        /// <param name="entity">Entity cần xóa</param>
        void Remove(T entity);

        /// <summary>
        /// Xóa entity theo ID
        /// </summary>
        /// <param name="id">ID của entity cần xóa</param>
        Task RemoveByIdAsync(object id);
    }
}
<<<<<<< HEAD
=======

>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88

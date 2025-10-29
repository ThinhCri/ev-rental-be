namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    /// <summary>
    /// Interface cho service xử lý file upload
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Upload file ảnh căn cước công dân
        /// </summary>
        /// <param name="file">File ảnh</param>
        /// <param name="userId">ID của user</param>
        /// <returns>URL của file đã upload</returns>
        Task<string> UploadCccdImageAsync(IFormFile file, int userId);

        /// <summary>
        /// Xóa file ảnh
        /// </summary>
        /// <param name="filePath">Đường dẫn file</param>
        /// <returns>Kết quả xóa file</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Kiểm tra file có hợp lệ không
        /// </summary>
        /// <param name="file">File cần kiểm tra</param>
        /// <returns>File có hợp lệ hay không</returns>
        bool IsValidImageFile(IFormFile file);

        /// <summary>
        /// Kiểm tra ảnh có phải là ảnh bằng lái xe hợp lệ không
        /// </summary>
        /// <param name="file">File ảnh cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        Task<(bool IsValid, string ErrorMessage)> ValidateLicenseImageAsync(IFormFile file);
    }
}

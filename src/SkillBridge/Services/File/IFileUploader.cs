using SkillBridge.Models.Enums;

namespace SkillBridge.Services.File
{
    /// <summary>
    /// Interface for a file uploader service that handles uploading, retrieving, and deleting files
    /// in Supabase storage buckets.
    /// </summary>
    public interface IFileUploader
    {
        /// <summary>
        /// Retrieves a file stored in Supabase Storage.
        /// For public files (images), this returns a permanent URL.
        /// For private files (CVs), this returns a signed URL valid for a limited time.
        /// </summary>
        /// <param name="filePath">The path or name of the file in the bucket.</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>A task that represents the asynchronous operation, containing the URL of the file.</returns>
        Task<string> GetFileAsync(string filePath, FileType type);

        /// <summary>
        /// Uploads a file to Supabase Storage.
        /// Validates the file type and extension before uploading.
        /// </summary>
        /// <param name="file">The file to upload (from IFormFile).</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing the public URL (for images)
        /// or signed URL (for CVs) of the uploaded file.
        /// </returns>
        Task<string> UploadFileAsync(IFormFile file, FileType type);

        /// <summary>
        /// Deletes a file from the appropriate Supabase bucket.
        /// Extracts the file name from the URL and removes it from storage.
        /// </summary>
        /// <param name="fileUrl">The full URL of the file to delete.</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteFileAsync(string fileUrl, FileType type);
    }

}

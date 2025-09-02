using Auth0.ManagementApi.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using SkillBridge.Data;
using SkillBridge.Models.Enums;
using SkillBridge.Services.ProjectAssignment;
using SkillBridge.Services.UserProfile;
using Supabase;

namespace SkillBridge.Services.File
{
    /// <summary>
    /// Implementation of the file uploader service
    /// </summary>
    public class SupabaseBucketFileUploader : IFileUploader
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<SupabaseBucketFileUploader> _logger;

        private readonly string[] _allowedImageTypes = { "image/jpeg", "image/png" };
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly string[] _allowedCvTypes = { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        private readonly string[] _allowedCvExtensions = { ".pdf", ".doc", ".docx" };



        public SupabaseBucketFileUploader(
            Supabase.Client supabaseClient, ILogger<SupabaseBucketFileUploader> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        /// <summary>
        /// Deletes a file from the appropriate Supabase bucket.
        /// Extracts the file name from the provided URL and removes it from the storage bucket.
        /// </summary>
        /// <param name="fileUrl">The full URL of the file to delete.</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileUrl"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the file name cannot be extracted from the URL.</exception>
        public async Task DeleteFileAsync(string fileUrl, FileType type)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));
            }

            var bucket = GetBucketName(type);
            var storage = _supabaseClient.Storage.From(bucket);

            // Extract the file name from the URL
            // For public URL: last segment of the path
            var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogInformation("Could not extract file name from URL: {Url}", fileUrl);
                throw new InvalidOperationException("Could not extract file name from URL.");
            }

            // Remove the file from the bucket
            await storage.Remove(new List<string> { fileName });
            _logger.LogInformation("File deleted successfully. URL: {Url}", fileUrl);

        }

        /// <summary>
        /// Returns the URL of a file stored in Supabase Storage.
        /// For public images, this is a permanent URL; for private CVs, this is a signed URL valid for a limited time.
        /// </summary>
        /// <param name="fileName">The name of the file in the bucket.</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>The URL of the file.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the file type is unknown.</exception>
        public async Task<string> GetFileAsync(string fileName, FileType type)
        {
            var bucket = GetBucketName(type);
            var storage = _supabaseClient.Storage.From(bucket);

            if (type == FileType.Image && !string.IsNullOrEmpty(fileName))
            {
                // Public bucket → return permanent public URL
                return storage.GetPublicUrl(fileName);
            }
            else if (type == FileType.CV && !string.IsNullOrEmpty(fileName))
            {
                var signedUrlResponse = string.Empty;
                // Private bucket → generate signed URL (valid for 1 hour)
                signedUrlResponse = await storage.CreateSignedUrl(fileName, 3600);
                return storage.GetPublicUrl(fileName);
            }
            _logger.LogInformation("Unknown file type. File Name: {FileName}", fileName);
            throw new ArgumentOutOfRangeException(nameof(type), "Unknown file type");
        }

        /// <summary>
        /// Uploads a file to Supabase Storage, either an image or a CV, depending on <paramref name="type"/>.
        /// Validates the file type and extension before uploading.
        /// </summary>
        /// <param name="file">The file to upload (from IFormFile).</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>The public URL (for images) or signed URL (for CVs) of the uploaded file.</returns>
        /// <exception cref="ArgumentException">Thrown if the file is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the file type or extension is invalid.</exception>
        public async Task<string> UploadFileAsync(IFormFile file, FileType type)
        {
            // Validate first
            ValidateFile(file, type);

            var bucket = GetBucketName(type);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            var storage = _supabaseClient.Storage.From(bucket);

            await storage.Upload(fileBytes, fileName, new Supabase.Storage.FileOptions
            {
                CacheControl = "3600",
                Upsert = false,
            });

            _logger.LogInformation("File uploaded successfully. URL: {PublicUrl}", fileName);
            return fileName;
        }

        /// <summary>
        /// Returns the corresponding bucket name for the given file type.
        /// </summary>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <returns>The name of the Supabase bucket to use.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the file type is unknown.</exception>
        private string GetBucketName(FileType type)
        {
            return type switch
            {
                FileType.Image => "images",
                FileType.CV => "files",
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        /// <summary>
        /// Validates the uploaded file against allowed MIME types and extensions for images or CVs.
        /// Throws an exception if the file is not allowed.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <param name="type">The type of file (Image or CV).</param>
        /// <exception cref="ArgumentException">Thrown if the file is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the file type or extension is invalid.</exception>
        private void ValidateFile(IFormFile file, FileType type)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var contentType = file.ContentType.ToLower();
            var extension = Path.GetExtension(file.FileName).ToLower();

            switch (type)
            {
                case FileType.Image:
                    if (!_allowedImageTypes.Contains(contentType) || !_allowedImageExtensions.Contains(extension))
                        throw new InvalidOperationException("Invalid image type. Allowed: JPG, PNG, GIF.");
                    break;

                case FileType.CV:
                    if (!_allowedCvTypes.Contains(contentType) || !_allowedCvExtensions.Contains(extension))
                        throw new InvalidOperationException("Invalid CV type. Allowed: PDF, DOC, DOCX.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown file type.");
            }
        }

    }
}

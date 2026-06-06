using Microsoft.AspNetCore.Http;

namespace TechMoveLogisticSystem.Api.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAgreementPdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("A PDF file is required.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            //only PDF files are allowed for signed agreements
            if (extension != ".pdf")
            {
                throw new ArgumentException("Only PDF files are allowed for signed agreements.");
            }

            // uses a unique file name to avoid overwriting existing files
            var savedFileName = $"{Guid.NewGuid()}{extension}";

            var uploadFolder = Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                "SignedAgreements");

            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, savedFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return savedFileName;
        }

        public string GetAgreementFilePath(string savedFileName)
        {
            // it rebuilds the file path when the user wants to download the agreement
            return Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                "SignedAgreements",
                savedFileName);
        }
    }
}
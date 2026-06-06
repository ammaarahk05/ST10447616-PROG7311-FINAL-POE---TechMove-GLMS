using Microsoft.AspNetCore.Http;

namespace TechMoveLogisticSystem.Api.Services
{
    public interface IFileStorageService
    {
        // Saves only valid PDF files and returns the saved file name
        Task<string> SaveAgreementPdfAsync(IFormFile file);

        // Builds the full path so the API can download the saved agreement
        string GetAgreementFilePath(string savedFileName);
    }
}
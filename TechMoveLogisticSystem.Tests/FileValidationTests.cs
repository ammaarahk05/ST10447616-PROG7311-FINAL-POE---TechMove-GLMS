using Xunit;
using System.IO;

namespace TechMoveLogisticSystem.Tests
{
    public class FileValidationTests
    {
        // mimics file validation logic
        private bool IsPdf(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() == ".pdf";
        }

        // test 1:PDF is valid
        [Fact]
        public void FileUpload_ValidPdf_ReturnsTrue()
        {
            var result = IsPdf("contract.pdf");

            Assert.True(result);
        }

        //test 2: Invalid file (eg: .txt or .exe)
        [Fact]
        public void FileUpload_ExeFile_ReturnsFalse()
        {
            var result = IsPdf("malware.exe");

            Assert.False(result);
        }

        //Test 3: Invalid (.jpg)file 
        [Fact]
        public void FileUpload_JpgFile_ReturnsFalse()
        {
            var result = IsPdf("image.jpg");

            Assert.False(result);
        }

        //test 4: Edge case, doesnt have extension
        [Fact]
        public void FileUpload_NoExtension_ReturnsFalse()
        {
            var result = IsPdf("file");

            Assert.False(result);
        }
    }
}
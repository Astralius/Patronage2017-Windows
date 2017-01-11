using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Explorer.Services.UnitTests
{
    public class GetFilesTests
    {
        #region Boundary Conditions

        [Fact]
        public void ReturnNullWhenPathIsNull()
        {
            var result = FileService.GetFiles(null);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnNullWhenPathIsBlank()
        {
            var result = FileService.GetFiles("");
            Assert.Null(result);
        }

        #endregion

        #region Other Conditions

        [Fact]
        public void ReturnNullWhenPathDoesNotExist()
        {
            var result = FileService.GetFiles("Foo:\\Bar");
            Assert.True(result == null, "Foo:\\Bar does not exist, so GetFiles should return null!");
        }

        [Fact]
        public void ReturnNullWhenPathIsInvalid()
        {
            List<FileInfo> result;

            result = FileService.GetFiles("~~szłem łąką, kwiaty pachły~~");
            Assert.True(result == null, "~~szłem łąką, kwiaty pachły~~ is definitely not a valid path, so GetFiles should return null!");
        }

        [Fact]
        public void ReturnSomethingWhenPathIsValid()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var result = FileService.GetFiles("C:\\Program Files\\Common Files");
                Assert.True(result != null, "C:\\Program Files\\Common Files should be a valid path under Windows OS!");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var result = FileService.GetFiles("/etc");
                Assert.True(result != null, "/etc should be a valid path under Linux OS!");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var result = FileService.GetFiles("/etc");
                Assert.True(result != null, "/etc should be a valid path under Mac OSX!");
            }
        }

        [Fact]
        public void ReturnCorrectNumberOfItems()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string testDir = null;

            #region Windows mockup

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create test files hierarchy
                testDir = currentDirectory + "\\TestDir";

                Directory.CreateDirectory(testDir + "\\SubDir1");
                Directory.CreateDirectory(testDir + "\\SubDir2");
                File.Create(testDir + "\\Test1.txt").Dispose();
                File.Create(testDir + "\\Test2.txt").Dispose();
                File.Create(testDir + "\\SubDir1\\Test3.txt").Dispose();
                File.Create(testDir + "\\SubDir2\\Test4.txt").Dispose();      
            }

            #endregion

            #region Linux/OSX mockup

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Create test files hierarchy
                testDir = currentDirectory + "/TestDir";

                Directory.CreateDirectory(testDir + "/SubDir1");
                Directory.CreateDirectory(testDir + "/SubDir2");
                File.Create(testDir + "/Test1.txt").Dispose();
                File.Create(testDir + "/Test2.txt").Dispose();
                File.Create(testDir + "/SubDir1/Test3.txt").Dispose();
                File.Create(testDir + "/SubDir2/Test4.txt").Dispose();
            }

            #endregion

            if (testDir == null) throw new System.Exception("Unsupported Operating System!");

            // Grab the result
            var result = FileService.GetFiles(currentDirectory + "\\TestDir");

            // Test the result
            Assert.True(result.Count == 4, "GetFiles() should've found 4 files in the test path, but it has not!");        

            Directory.Delete(testDir, true);
        }

        #endregion
    }
}

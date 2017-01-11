using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Explorer.Services.UnitTests
{
    public class GetFileInfoTests
    {
        #region Boundary Conditions

        [Fact]
        public void ReturnNullWhenPathIsNull()
        {
            var result = FileService.GetFileInfo(null);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnNullWhenPathIsBlank()
        {
            var result = FileService.GetFileInfo("");
            Assert.Null(result);
        }

        #endregion

        #region Other Conditions

        [Fact]
        public void ReturnNullWhenPathDoesNotExist()
        {
            var result = FileService.GetFiles("Foo:\\Bar.txt");
            Assert.True(result == null, "Foo:\\Bar.txt does not exist, so GetFileInfo should return null!");
        }

        [Fact]
        public void ReturnNullWhenPathIsInvalid()
        {
            List<FileInfo> result;

            result = FileService.GetFiles("!@#$%^&*()");
            Assert.True(result == null, "!@#$%^&*() is not a valid file path, so GetFileInfo should return null!");
        }

        [Fact]
        public void ReturnCorrectItemInformation()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string testFilePath = null;

            // Create test file path, platform-dependently
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                testFilePath = currentDirectory + "\\Testfile.test";       
                 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                testFilePath = currentDirectory + "/Testfile.test";

            // Create the file itself
            FileStream stream = File.Create(testFilePath);
            stream.Flush();
            stream.Dispose();

            // Set mockup metadata
            var info = new FileInfo(testFilePath);
            DateTime now = DateTime.UtcNow;
            info.CreationTime = now;
            info.LastWriteTime = now;

            // Vertify data received through the tested method
            var modelInfo = FileService.GetFileInfo(testFilePath);

            Assert.True(modelInfo.Name == info.Name, 
                "The obtained file name (" + modelInfo.Name + ")\n" +
                "does not match the actual value (" + info.Name + ")");

            Assert.True(modelInfo.FullPath == testFilePath, 
                "The obtained file path (" + modelInfo.FullPath + ")\n" + 
                "does not match the actual value (" + testFilePath + ")");

            Assert.True(modelInfo.FullPath == info.FullName,
                "The obtained file path (" + modelInfo.FullPath + ")\n" + 
                "does not match the actual value (" + info.FullName + ")");

            Assert.True(modelInfo.DateCreated.Equals(info.CreationTime), 
                "The obtained creation time (" + modelInfo.DateCreated + ")\n" + 
                "does not match the actual value (" + info.CreationTime + ")");

            Assert.True(modelInfo.DateModified.Equals(info.LastWriteTime),
                "The obtained last modification time (" + modelInfo.DateModified + ")\n" + 
                "does not match the actual value (" + info.LastWriteTime + ")");

            File.Delete(testFilePath);
        }
        #endregion
    }
}

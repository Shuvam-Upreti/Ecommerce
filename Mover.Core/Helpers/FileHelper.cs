using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Exceptions;

namespace Mover.Core.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _imagePath;

        public FileHelper(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this._hostingEnvironment = hostingEnvironment;
            _imagePath = configuration["ImageSettings:DestinationFolder"];
        }
        public bool IsImageValid(string file_name)
        {
            var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif" };
            var extension = Path.GetExtension(file_name).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;
            return true;
        }

        public bool IsExcelFileValid(string file_name)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls", ".docs", "doc" };
            var extension = Path.GetExtension(file_name).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;
            return true;
        }

        public async Task<string> SaveImageAndGetFileName(IFormFile Attachment, string destination_folder, string file_prefix = "")
        {
            Random random = new Random();
            string file_name = "";
            if (string.IsNullOrWhiteSpace(file_prefix))
            {
                file_name = Path.GetFileNameWithoutExtension(Attachment.FileName).Replace(' ', '-') + random.Next(1, 1232384943) + Path.GetExtension(Attachment.FileName);
            }
            else
            {
                file_name = file_prefix.Replace(' ', '-') + random.Next(1, 1232384943) + Path.GetExtension(Attachment.FileName);
            }

            var filePath = Path.Combine(destination_folder, file_name);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Attachment.CopyToAsync(stream);
            }
            return file_name;
        }

        public string MoveImageFromTempPathToDestination(string image_name, string destination_folder)
        {
            var destinationPath = Path.Combine(_hostingEnvironment.WebRootPath, destination_folder);

            bool doDestinationDirectoryExists = System.IO.Directory.Exists(destinationPath);

            if (!doDestinationDirectoryExists)
                System.IO.Directory.CreateDirectory(destinationPath);

            var tempFolder = Path.GetTempPath();

            File.Move($@"{tempFolder}/{image_name}", $@"{destinationPath}/{image_name}");

            return $@"{destinationPath}\{image_name}";

        }
        public async Task DeleteImageAsync(string imageUrl,string imagePath)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, imageUrl);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Delete the image from the server
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception accordingly
                    throw new CustomException($"Error deleting image: {ex.Message}");
                }
            }
            else
            {
                throw new CustomException("Image not found on server.");
            }
        }
    }
}

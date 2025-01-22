using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Helpers
{
    public interface IFileHelper
    {
        bool IsImageValid(string file_name);
        bool IsExcelFileValid(string file_name);
        Task<string> SaveImageAndGetFileName(IFormFile Attachment, string destination_folder, string file_prefix = "");
        string MoveImageFromTempPathToDestination(string image_name, string destination_folder);
        Task DeleteImageAsync(string imageUrl,string imagePath);
    }
}

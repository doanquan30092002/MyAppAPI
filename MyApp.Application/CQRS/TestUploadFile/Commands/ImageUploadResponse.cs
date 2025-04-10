using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.TestUploadFile.Commands
{
    public class ImageUploadResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
        public Image Image { get; set; }
    }
}

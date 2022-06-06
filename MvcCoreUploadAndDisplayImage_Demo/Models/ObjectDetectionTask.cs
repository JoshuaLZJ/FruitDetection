using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace MvcCoreUploadAndDisplayImage_Demo.Models
{
    public class ObjectDetectionTask
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please choose an image")]
        public string InputPicture { get; set; }

        public string ProcessedImage { get; set; }

        public ObjectDetectionTask()
        {
            
        }
    }
}

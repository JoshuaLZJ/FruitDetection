using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace MvcCoreUploadAndDisplayImage_Demo.ViewModels
{
    public class ObjectDetectionViewModel
    {
        [Required(ErrorMessage = "Please choose an image")]
        [Display(Name = "Input Image")]
        public IFormFile InputImage { get; set; }

        //[Display(Name = "Processed Image")]
        //public Image ProcessedImage { get; set; }
    }
}

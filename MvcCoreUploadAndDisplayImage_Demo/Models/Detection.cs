using System;
using System.Collections.Generic;

namespace MvcCoreUploadAndDisplayImage_Demo.Models
{
    public class Detection
    {
        public IEnumerable<float[]> detection_boxes { get; set; }
        public IEnumerable<float[]> detection_scores { get; set; }
        public IEnumerable<float[]> detection_classes { get; set; }
    }
}

using System;
using Microsoft.ML.Data;

namespace MvcCoreUploadAndDisplayImage_Demo.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using MvcCoreUploadAndDisplayImage_Demo.DataStructures;
using MvcCoreUploadAndDisplayImage_Demo.YoloParser;
using MvcCoreUploadAndDisplayImage_Demo.Models;
using System.Drawing;

namespace MvcCoreUploadAndDisplayImage_Demo
{
    public class FasterRCNNScorer
    {
        private readonly ITransformer MLModel;

        private IList<YoloBoundingBox> _boundingBoxes = new List<YoloBoundingBox>();

        public FasterRCNNScorer(ITransformer MLModel)
        {
            this.MLModel = MLModel;
        }

        public struct ImageNetSettings
        //this struct is not used
        {
            public const int imageHeight = 640;
            public const int imageWidth = 640;
        }

        public struct LabelToClass
        {
            public static readonly string[] labels = { "Empty", "Apple" };
            public static Color[] classColors = new Color[]
        {
            Color.Khaki,
            Color.Fuchsia,
            Color.Silver,
            Color.RoyalBlue,
            Color.Green,
            Color.DarkOrange,
            Color.Purple,
            Color.Gold,
            Color.Red,
            Color.Aquamarine,
            Color.Lime,
            Color.AliceBlue,
            Color.Sienna,
            Color.Orchid,
            Color.Tan,
            Color.LightPink,
            Color.Yellow,
            Color.HotPink,
            Color.OliveDrab,
            Color.SandyBrown,
            Color.DarkTurquoise
        };
        }


        //public struct TinyYoloModelSettings
        //{
        //    // for checking Tiny yolo2 Model input and  output  parameter names,
        //    //you can use tools like Netron, 
        //    // which is installed by Visual Studio AI Tools

        //    // input tensor name
        //    public const string ModelInput = "image";

        //    // output tensor name
        //    public const string ModelOutput = "grid";
        //}

        //private ITransformer LoadModel(string modelLocation)
        //{
        //    //Console.WriteLine("Read model");
        //    //Console.WriteLine($"Model location: {modelLocation}");
        //    //Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})");

        //    var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

        //    var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
        //        .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "image"))
        //        .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
        //        .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { TinyYoloModelSettings.ModelOutput }, inputColumnNames: new[] { TinyYoloModelSettings.ModelInput }));

        //    var model = pipeline.Fit(data);

        //    return model;
        //}

        private Detection PredictDataUsingModel(IDataView testData, ITransformer model)
        {
            //Console.WriteLine($"Images location: {imagesFolder}");
            //Console.WriteLine("");
            //Console.WriteLine("=====Identify the objects in the images=====");
            //Console.WriteLine("");

            IDataView scoredData = model.Transform(testData);
            Detection detection = new Detection();

            detection.detection_boxes = scoredData.GetColumn<float[]>(TensorFlowModelSettings.detection_boxes);
            detection.detection_scores = scoredData.GetColumn<float[]>(TensorFlowModelSettings.detection_scores);
            detection.detection_classes = scoredData.GetColumn<float[]>(TensorFlowModelSettings.detection_classes);

            //IEnumerable<float[]> testing = scoredData.GetColumn<float[]>("StatefulPartitionedCall:0");

            return detection;
        }

        public Detection Score(IDataView data)
        {
            var model = MLModel;

            return PredictDataUsingModel(data, model);
        }
    }
}

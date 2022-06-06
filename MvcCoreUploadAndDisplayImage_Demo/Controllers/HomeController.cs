using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreUploadAndDisplayImage_Demo.Data;
using MvcCoreUploadAndDisplayImage_Demo.Models;
using MvcCoreUploadAndDisplayImage_Demo.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using MvcCoreUploadAndDisplayImage_Demo.YoloParser;
using MvcCoreUploadAndDisplayImage_Demo.DataStructures;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using Microsoft.ML.Data;


namespace MvcCoreUploadAndDisplayImage_Demo.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly MLContext _mlContext;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, MLContext mlContext)
        {
            dbContext = context;
            webHostEnvironment = hostEnvironment;
            // Get the injected objects
            _mlContext = mlContext;
        }

        // added methods
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private static Image DrawBoundingBox(string inputImageLocation, List<List<float[]>> filteredBoundingBoxes)
        {
            Image image = Image.FromFile(inputImageLocation);

            string[] labels = FasterRCNNScorer.LabelToClass.labels;
            Color[] classColors = FasterRCNNScorer.LabelToClass.classColors;

            foreach (var box in filteredBoundingBoxes)
            {
                var x_orig = box.ElementAt(0)[1];
                var y_orig = box.ElementAt(0)[0];
                var width_orig = box.ElementAt(0)[3] - box.ElementAt(0)[1];
                var height_orig = box.ElementAt(0)[2] - box.ElementAt(0)[0];

                var x = (uint) (((double)x_orig * ImageSettings.imageWidth) * ((double)image.Width / ImageSettings.imageWidth));
                var y = (uint) (((double)y_orig * ImageSettings.imageHeight) * ((double)image.Height / ImageSettings.imageHeight));
                var width = (uint) (((double)width_orig * ImageSettings.imageWidth) * ((double)image.Width / ImageSettings.imageWidth));
                var height = (uint) (((double)height_orig * ImageSettings.imageHeight) * ((double)image.Height / ImageSettings.imageHeight));

                string text = $"{labels[(uint)box.ElementAt(2)[0]]} ({(box.ElementAt(1)[0] * 100).ToString("0")}%)";

                using (Graphics thumbnailGraphic = Graphics.FromImage(image))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Define Text Options
                    Font drawFont = new Font("Arial", 12, FontStyle.Bold);
                    SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
                    SolidBrush fontBrush = new SolidBrush(Color.Black);
                    Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                    // Define BoundingBox options
                    var BoxColor = classColors[(uint)box[2][0]];
                    Pen pen = new Pen(BoxColor, 3.2f);
                    SolidBrush colorBrush = new SolidBrush(BoxColor);

                    thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                    thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

                    // Draw bounding box on image
                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height);
                }
            }
            return (image);
        }

        public async Task<IActionResult> Index()
        {
            var Task = await dbContext.ObjectDetectionTasks.ToListAsync();
            return View(Task);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(ObjectDetectionViewModel model)
        {
            Detection detection = new Detection();
            //IEnumerable<float[]> detection = Enumerable.Empty<float[]>();
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                var assetsRelativePath = @"../../../assets";
                string assetsPath = GetAbsolutePath(assetsRelativePath);
                var modelFilePathName = Path.Combine(assetsPath, "saved_model_haha", "efficientdet.onnx");
                //var modelFilePathName = Path.Combine(assetsPath, "saved_model");

                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                string image_filePath = Path.Combine(uploadsFolder, uniqueFileName);

                IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(uploadsFolder, uniqueFileName);
                IDataView imageDataView = _mlContext.Data.LoadFromEnumerable(images);

                var data = _mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

                var pipeline = _mlContext.Transforms.LoadImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                .Append(_mlContext.Transforms.ResizeImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight, resizing: Microsoft.ML.Transforms.Image.ImageResizingEstimator.ResizingKind.Fill))
                .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: TensorFlowModelSettings.inputTensorName, outputAsFloatArray: false, interleavePixelColors: true))
                .Append(_mlContext.Transforms.ApplyOnnxModel(shapeDictionary: new Dictionary<string, int[]>()
                {
                    //{TensorFlowModelSettings.inputTensorName, new[] {1, 640, 640, 3} }
                    {TensorFlowModelSettings.inputTensorName, new[] {1, 512, 512, 3} }
                },
                modelFile: modelFilePathName,
                outputColumnNames: new[] { TensorFlowModelSettings.detection_boxes, TensorFlowModelSettings.detection_classes, TensorFlowModelSettings.detection_scores },
                inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName }));

                //var pipeline = _mlContext.Transforms.LoadImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                //.Append(_mlContext.Transforms.ResizeImages(outputColumnName: TensorFlowModelSettings.inputTensorName, imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight))
                //.Append(_mlContext.Transforms.ExtractPixels(outputColumnName: TensorFlowModelSettings.inputTensorName, outputAsFloatArray: false))
                //.Append(_mlContext.Model.LoadTensorFlowModel(modelFilePathName)
                //                               .ScoreTensorFlowModel(
                //                                      outputColumnNames: new[] { "StatefulPartitionedCall:0" },
                //                                      inputColumnNames: new[] { TensorFlowModelSettings.inputTensorName },
                //                                      addBatchDimensionInput: false));

                var mlModel = pipeline.Fit(data);

                var modelScorer = new FasterRCNNScorer(mlModel);
                // Use model to score data
                detection = modelScorer.Score(imageDataView);

                var FilteredBoxes = new List<List<float[]>>();

                int i = 0;
                foreach (var score in detection.detection_scores.ElementAt(0))
                {
                    var FilteredBox = new List<float[]>();
                    if (score > .5F) //set min score to count detection as positive
                    {
                        var coords = detection.detection_boxes.ElementAt(0).Skip(i * 4).Take(4);
                        float[] box_coords = coords.Cast<float>().ToArray();
                        FilteredBox.Add(box_coords);
                        FilteredBox.Add(new float[] { detection.detection_scores.ElementAt(0).ElementAt(i) });
                        FilteredBox.Add(new float[] { detection.detection_classes.ElementAt(0).ElementAt(i) });

                        FilteredBoxes.Add(FilteredBox);
                    }
                    i = i + 1;
                }

                //string imageFileName = images.ElementAt(i).Label;
                Image processed_image = DrawBoundingBox(image_filePath, FilteredBoxes);
                string uniquesavedimagename = SavingProcessedImage(processed_image);

                ObjectDetectionTask ObjectDetectionTask = new ObjectDetectionTask
                {
                    InputPicture = uniqueFileName,
                    ProcessedImage = uniquesavedimagename
                };

                dbContext.Add(ObjectDetectionTask);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string UploadedFile(ObjectDetectionViewModel model)
        {
            string uniqueFileName = null;

            if (model.InputImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.InputImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.InputImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private string SavingProcessedImage(Image processed_image)
        {
            string uniqueFileName = null;

            if (processed_image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    processed_image.Save(fileStream, ImageFormat.Jpeg);
                }
            }
            return uniqueFileName;
        }
    }
}

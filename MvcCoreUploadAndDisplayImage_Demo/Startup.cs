using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MvcCoreUploadAndDisplayImage_Demo.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;

namespace MvcCoreUploadAndDisplayImage_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSingleton<MLContext>();
            //services.AddSingleton<EstimatorChain<OnnxTransformer>>((ctx) =>
            //{
            //    MLContext mlContext = ctx.GetRequiredService<MLContext>();
            //    //string modelFilePathName = Configuration["/Users/joshualaw/Documents/Monash_U_Work/Codon_Genomics/ObjectDetectionAPI/MLModels"];

            //    var assetsRelativePath = @"../../../assets";
            //    string assetsPath = GetAbsolutePath(assetsRelativePath);
            //    var modelFilePathName = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");
            //    //var imagesFolder = Path.Combine(assetsPath, "images");
            //    //var outputFolder = Path.Combine(assetsPath, "images", "output");

            //    //IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(imagesFolder);
            //    //IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

            //    var pipeline = mlContext.Transforms.LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
            //    .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: 416, imageHeight: 416, inputColumnName: "image"))
            //    .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
            //    .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelFilePathName, outputColumnNames: new[] { "grid" }, inputColumnNames: new[] { "image" }));

            //    //var mlModel = pipeline.Fit(imageDataView);


            //    return (pipeline);
            //});

        //    services.AddSingleton <ITransformer> ((ctx) =>
        //    {
        //        MLContext mlContext = ctx.GetRequiredService<MLContext>();
        //        var assetsRelativePath = @"../../../assets";
        //        string assetsPath = GetAbsolutePath(assetsRelativePath);
        //        var modelFilePathName = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");

        //        ITransformer mlModel;
        //        using (var fileStream = File.OpenRead(modelFilePathName))
        //            mlModel = mlContext.Model.Load(fileStream, out DataViewSchema columns);

        //        return (mlModel);
        //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}

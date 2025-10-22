using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ResizeImages
{
    public class ResizeImages
    {
        [FunctionName("ResizeImage")]
        public void Run(
            [BlobTrigger("articleimages/{name}", Connection = "AzureWebJobsStorage")] Stream image, string name, ILogger log,
            [Blob("articleimages-sm/{name}", FileAccess.Write)] Stream imageSmall, // A type of binding that creates a new blob in the articleimages-sm container
            [Blob("articleimages-md/{name}", FileAccess.Write)] Stream imageMedium) // A type of binding that creates a new blob in the articleimages-md container
        {
            log.LogInformation($"Processing image: {name}");

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy the input stream to a memory stream inorder to be able to read it several times
                    image.CopyTo(memoryStream);

                    // Reset the position of the memory stream to the beginning so it can be read for image processing
                    memoryStream.Position = 0;

                    // Load the image from the memory stream
                    using (Image<Rgba32> input = Image.Load<Rgba32>(memoryStream))
                    {
                        log.LogInformation($"Original size: {input.Width}x{input.Height}");

                        // Resize the image to 640x400 and save it to the imageSmall stream
                        ResizeImageAuto(input, imageSmall, 640, 400, 400, 640, log);
                    }

                    // Same as above but for the medium image
                    memoryStream.Position = 0; 

                    using (Image<Rgba32> input = Image.Load<Rgba32>(memoryStream))
                    {
                        ResizeImageAuto(input, imageMedium, 800, 600, 600, 800, log);
                    }
                }

                log.LogInformation($"Successfully processed {name}");
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing {name}: {ex.Message}");
            }
        }

        // Resize the image to the specified dimensions
        public static void ResizeImageAuto(Image<Rgba32> input, Stream output, int landscapeWidth, int landscapeHeight, int portraitWidth, int portraitHeight, ILogger log)
        {
            int originalWidth = input.Width;
            int originalHeight = input.Height;

            // Check if the image is landscape or portrait
            bool isLandscape = originalWidth >= originalHeight;

            // Determine max width and height based on orientation
            int maxWidth = isLandscape ? landscapeWidth : portraitWidth;
            int maxHeight = isLandscape ? landscapeHeight : portraitHeight;

            // Compute scale factor
            // The smaller of the two scales is used to ensure that the image fits within the specified dimensions
            double scale = Math.Min((double)maxWidth / originalWidth, (double)maxHeight / originalHeight);
            int newWidth = (int)(originalWidth * scale);
            int newHeight = (int)(originalHeight * scale);

            log.LogInformation($"Resizing to {newWidth}x{newHeight}");

            // Resize the image
            input.Mutate(x => x.Resize(newWidth, newHeight));

            // Save the image with 100% quality
            input.Save(output, new JpegEncoder { Quality = 100 });
        }
    }
}




















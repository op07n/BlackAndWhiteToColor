using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithmia;
using BlackAndWhiteToColor.Models;
using Newtonsoft.Json;

namespace BlackAndWhiteToColor.Services
{
    public static class Colorize
    {
        readonly static string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            ? Environment.GetEnvironmentVariable("HOME")
            : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

        private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", ".JPEG" };
        private static Client client = new Client(ConfigurationHelper.Configuration["AlgorithmiaKey"]);

        public static void Start(string directory)
        {
            var images = GetImages(directory);
            var colorizedImages = ProcessImages(images.base64List);
            Console.WriteLine("Images are colerized");
            DownloadFile(colorizedImages, images.names, directory);
        }

        private static Images GetImages(string directory)
        {
            var path = Path.Combine(homePath, directory);
            var files = Directory.GetFiles(path);
            var base64Array = new List<string>();
            var names = new List<string>();
            foreach (var file in files)
            {
                if (ImageExtensions.Contains(Path.GetExtension(file).ToUpperInvariant()))
                {
                    Console.WriteLine($"Image found add location {file}");

                    // image to base64
                    byte[] b = File.ReadAllBytes(file);
                    var base64 = "data:image/png;base64," + Convert.ToBase64String(b);
                    base64Array.Add(base64);
                    Console.WriteLine("Image is converted to Base64");

                    // get name from image 
                    var split = file.Split("/");
                    var name = split.ElementAt(split.Count() - 1);
                    names.Add(name.Split(".").ElementAt(0));
                }
            }

            return new Images()
            {
                base64List = base64Array,
                names = names
            };
        }

        private static List<string> ProcessImages(List<string> images)
        {
            var input = new { image = images };
            var jsonObject = JsonConvert.SerializeObject(input);

            Console.WriteLine("Images are send to Algorithmia and will now be colored. this may take a few minutes.");
            var algorithm = client.algo("deeplearning/ColorfulImageColorization/1.1.13");
            algorithm.setOptions(timeout: 300); // optional
            var response = algorithm.pipeJson<Result>(jsonObject);
            var result = response.result as Result;
            return result.output;
        }



        private static void DownloadFile(List<string> files, List<string> names, string directory)
        {
            var path = Path.Combine(homePath, directory, "Colorization");
            Directory.CreateDirectory(path);
            for (int i = 0; i < files.Count(); i++)
            {
                var file = files.ElementAt(i);
                var name = names.ElementAt(i);

                if (client.file(file).exists())
                {
                    var bytes = client.file(file).getBytes();
                    File.WriteAllBytes(Path.Combine(path, $"{name}.png"), bytes);
                    Console.WriteLine($"the image {name} is downloaded and stored on the location " + Path.Combine(path, $"{name}.png"));
                }
                else
                {
                    Console.WriteLine("The file does not exist");
                }
            }


        }
    }
}

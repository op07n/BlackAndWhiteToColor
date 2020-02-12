using System;
using System.IO;
using BlackAndWhiteToColor.Services;
using Microsoft.Extensions.Configuration;

namespace BlackAndWhiteToColor
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            Colorize.Start("images");
        }
    }
}

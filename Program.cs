using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using dylan_stego.Stego;

namespace dylan_stego
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Encode: ");


            // test logpoint

            byte[] inputData = File.ReadAllBytes("dylan/stego.py");
            Bitmap original = (Bitmap)Bitmap.FromFile("sample_images/original.png");
            Bitmap modified = Stego.Stego.EncodeImage(original, StegoType.DylanLSB, inputData);

            Console.WriteLine("\nDecode: ");

            modified.Save("sample_images/modified.png");

            //byte[] outputData = Stego.Stego.DecodeImage(modified, StegoType.DylanLSB);

            Bitmap dylan_modified = (Bitmap)Bitmap.FromFile("sample_images/modified.png");

            byte[] outputData = Stego.Stego.DecodeImage(dylan_modified, StegoType.DylanLSB);

            File.WriteAllBytes("sample_images/data-decoded", outputData);

            /*string str_data = System.Text.Encoding.Default.GetString(outputData);

            Console.WriteLine("Decoded data as string with Default encoding:");

            Console.WriteLine(str_data);*/
        }
    }
}

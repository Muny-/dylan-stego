using System;
using System.Collections.Generic;
using System.Drawing;

namespace dylan_stego.Stego
{
    public enum StegoType
    {
        DylanLSB = 0
    }

    public class Stego
    {
        public static Bitmap EncodeImage(Bitmap original, StegoType type, byte[] data)
        {
            Bitmap encodedImage = (Bitmap)original.Clone();

            byte[] dataToWrite = new byte[data.Length + 4];

            BitConverter.GetBytes(data.Length).CopyTo(dataToWrite, 0);

            data.CopyTo(dataToWrite, 4);

            int dataToWriteIndex = 0;

            bool is_high_bits = true;

            switch(type)
            {
                case StegoType.DylanLSB:
                    for (int x = 0; x < original.Width; x++)
                    {
                        for (int y = 0; y < original.Height; y++)
                        {
                            if (dataToWriteIndex < dataToWrite.Length)
                            {
                                Color c = encodedImage.GetPixel(x, y);

                                byte bit3 = 0;
                                byte bit2 = 0;
                                byte bit1 = 0;
                                byte bit0 = 0;

                                if (is_high_bits)
                                {
                                    bit3 = (byte)((dataToWrite[dataToWriteIndex] & 0b10000000) >> 7);
                                    bit2 = (byte)((dataToWrite[dataToWriteIndex] & 0b01000000) >> 6);
                                    bit1 = (byte)((dataToWrite[dataToWriteIndex] & 0b00100000) >> 5);
                                    bit0 = (byte)((dataToWrite[dataToWriteIndex] & 0b00010000) >> 4);

                                    is_high_bits = !is_high_bits;
                                }
                                else
                                {
                                    bit3 = (byte)((dataToWrite[dataToWriteIndex] & 0b00001000) >> 3);
                                    bit2 = (byte)((dataToWrite[dataToWriteIndex] & 0b00000100) >> 2);
                                    bit1 = (byte)((dataToWrite[dataToWriteIndex] & 0b00000010) >> 1);
                                    bit0 = (byte)((dataToWrite[dataToWriteIndex] & 0b00000001));

                                    is_high_bits = !is_high_bits;

                                    dataToWriteIndex++;
                                }

                                byte r = 0;
                                byte g = 0;
                                byte b = 0;
                                byte a = 0;

                                byte setMask = 0b00000001;
                                byte resetMask = 0b11111110;

                                /*Console.Write($"{bit3}{bit2}{bit1}{bit0}");

                                if (is_high_bits)
                                    Console.WriteLine();*/
                                //Console.WriteLine($"is_high_bits={!is_high_bits}");

                                r = (byte)(bit3 == 1 ? c.R | setMask : c.R & resetMask);
                                g = (byte)(bit2 == 1 ? c.G | setMask : c.G & resetMask);
                                b = (byte)(bit1 == 1 ? c.B | setMask : c.B & resetMask);
                                a = (byte)(bit0 == 1 ? c.A | setMask : c.A & resetMask);

                                //Console.WriteLine($"old: r={c.R}, g={c.G}, b={c.B}, a={c.A}");
                                //Console.WriteLine($"new: r={r}, g={g}, b={b}, a={a}");

                                Color newColor = Color.FromArgb(a, r, g, b);

                                encodedImage.SetPixel(x, y, newColor);
                            }
                            else
                            {
                                return encodedImage;
                            }
                        }
                    }
                    break;

                default:
                    return original;
            }

            return original;
        }

        public static byte[] DecodeImage(Bitmap image, StegoType type)
        {
            List<byte> dataBytes = new List<byte>();

            byte buff = 0;
            bool is_high_bits = true;

            int data_length = 0;

            switch (type)
            {
                case StegoType.DylanLSB:
                    for (int x = 0; x < image.Width; x++)
                    {
                        for (int y = 0; y < image.Height; y++)
                        {
                            Color c = image.GetPixel(x, y);

                            //Console.WriteLine($"r={c.R}, g={c.G}, b={c.B}, a={c.A}");

                            byte mask = 0b00000001;

                            byte bit3 = (byte)(c.R & mask);
                            byte bit2 = (byte)(c.G & mask);
                            byte bit1 = (byte)(c.B & mask);
                            byte bit0 = (byte)(c.A & mask);

                            /*Console.Write($"{bit3}{bit2}{bit1}{bit0}");

                            if (!is_high_bits)
                                Console.WriteLine();*/

                            if (is_high_bits)
                            {
                                buff = (byte)(bit3 << 7 | bit2 << 6 | bit1 << 5 | bit0 << 4);

                                is_high_bits = false;
                            }
                            else
                            {
                                buff = (byte)(buff | bit3 << 3 | bit2 << 2 | bit1 << 1 | bit0);

                                dataBytes.Add(buff);

                                // we finished reading the length bytes
                                if (data_length == 0 && dataBytes.Count == 4)
                                {
                                    List<byte> len_bytes = dataBytes.GetRange(0, 4);
                                    //len_bytes.Reverse();

                                    data_length = BitConverter.ToInt32(len_bytes.ToArray(), 0);

                                    if (data_length < 0)
                                    {
                                        Console.WriteLine($"Data length [{data_length}] < 0");

                                        return new byte[] { };
                                    }
                                }
                                // we're done reading the data
                                else if (data_length != 0 && (dataBytes.Count - 4) == data_length)
                                {
                                    Console.WriteLine($"Finished reading 4+{data_length} bytes");

                                    return dataBytes.GetRange(4, data_length).ToArray();
                                }

                                is_high_bits = true;
                            }
                        }
                    }

                    break;
                default:
                    return new byte[] { };
            }

            Console.WriteLine("Didn't do anything?");
            return new byte[] { };
        }
    }
}
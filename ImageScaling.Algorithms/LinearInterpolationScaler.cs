using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaling.Algorithms
{
    public class LinearInterpolationScaler : Scaler
    {
        public override byte[] ScaleImage(float scale, byte[] bytes, int originalWidth, int originalHeight)
        {
            // Scale uses the width and height to calculate new dimensions. Not the amount of pixels. 
            var width = (int) Math.Floor(originalWidth * scale);
            var height = (int) Math.Floor(originalHeight * scale);
            var pixelAmount = width * height;

            var result = new byte[pixelAmount * 4];

            // Loop through all rows of new image
            for (var y = 0; y < height; y++)
            {
                // Loop through all cols of new image
                for (var x = 0; x < width; x++)
                {
                    var startIndex = (y * width * 4) + (x * 4);

                    var oy = y / scale;
                    var ox = x / scale;

                    var oyf = oy % 1;
                    var oxf = ox % 1;

                    var oyp = Math.Floor(oy);
                    var oxp = Math.Floor(ox);
                    var oyn = Math.Ceiling(oy);
                    var oxn = Math.Ceiling(ox);

                    var tli = (int)Math.Floor((oyp * originalWidth * 4) + (oxp * 4));
                    var tri = (int)Math.Floor((oyp * originalWidth * 4) + (oxn * 4));
                    var bli = (int)Math.Floor((oyn * originalWidth * 4) + (oxp * 4));
                    var bri = (int)Math.Floor((oyn * originalWidth * 4) + (oxn * 4));

                    // Loop through all colors (RGBA)
                    for (var c = 0; c < 4; c++)
                    {
                        var i = startIndex + c;

                        // Set alpha to 255
                        if (c == 3)
                        {
                            result[i] = 0xff;
                            continue;
                        }

                        // Determine color
                        var tlc = bytes.Length < tli ? bytes[bytes.Length - 1] : bytes[tli + c];
                        var trc = bytes.Length < tri ? bytes[bytes.Length - 1] : bytes[tri + c];
                        var blc = bytes.Length < bli ? bytes[bytes.Length - 1] : bytes[bli + c];
                        var brc = bytes.Length < bri ? bytes[bytes.Length - 1] : bytes[bri + c];

                        var x1Avg = (tlc - trc) * oxf;
                        var x2Avg = (blc - brc) * oxf;
                        var avg = (x1Avg - x2Avg) * oyf;

                        result[i] = (byte) avg;
                    }
                }
            }

            return result;
        }
    }
}

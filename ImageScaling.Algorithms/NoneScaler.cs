using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaling.Algorithms
{
    public class NoneScaler : Scaler
    {
        public override byte[] ScaleImage(float scale, byte[] bytes, int originalWidth, int originalHeight)
        {
            // Scale uses the width and height to calculate new dimensions. Not the amount of pixels. 
            var width = (int)Math.Floor(originalWidth * scale);
            var height = (int)Math.Floor(originalHeight * scale);
            var pixelAmount = width * height;

            var result = new byte[pixelAmount * 4];

            // Loop through all rows of new image
            for (var y = 0; y < height; y++)
            {
                // Loop through all cols of new image
                for (var x = 0; x < width; x++)
                {
                    var startIndex = (y * width * 4) + (x * 4);
                    var oy = ((float)y / scale);
                    var ox = ((float)x / scale);

                    var exactPixelExists = oy % 1 == 0 && ox % 1 == 0;
                    var originalPixelStartIndex = (int)Math.Floor((oy * originalWidth * 4) + (ox * 4));

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

                        // Set to black if pixel does not exists
                        if (!exactPixelExists)
                        {
                            result[i] = 0x00;
                            continue;
                        }

                        // Determine color
                        result[i] = bytes[originalPixelStartIndex + c];
                    }
                }
            }

            return result;
        }
    }
}

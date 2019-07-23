using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaling.Algorithms
{
    public abstract class Scaler
    {
        public abstract byte[] ScaleImage(float scale, byte[] bytes, int originalWidth, int originalHeight);
    }
}

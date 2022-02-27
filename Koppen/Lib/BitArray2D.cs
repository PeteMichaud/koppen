using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Koppen
{
    public class BitArray2D : UnsafeBitArray2D
    {

        public BitArray2D(int dimension1, int dimension2)
            : base(dimension1, dimension2)
        {
            if(dimension1 <= 0) throw new ArgumentOutOfRangeException(nameof(dimension1), dimension1, string.Empty);
            if(dimension2 <= 0) throw new ArgumentOutOfRangeException(nameof(dimension2), dimension2, string.Empty);
        }

        public new bool Get(int x, int y) { 
            CheckBounds(x, y);
            return base.Get(x, y);
        }
        public new bool Set(int x, int y, bool val) { 
            CheckBounds(x, y);
            return base.Set(x, y, val);
        }

        private void CheckBounds(int x, int y)
        {
            if (x < 0 || x >= _dimension1)
            {
                throw new IndexOutOfRangeException();
            }
            if (y < 0 || y >= _dimension2)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}

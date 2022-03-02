using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Koppen
{
    public class UnsafeBitArray2D
    {
        private readonly BitArray _array;
        protected int _dimension1;
        protected int _dimension2;

        public int Dimension1 => _dimension1;
        public int Dimension2 => _dimension2;

        public UnsafeBitArray2D(int dimension1, int dimension2)
        {
            _dimension1 = dimension1;
            _dimension2 = dimension2;
            _array = new BitArray(dimension1 * dimension2);
        }

        public bool Get(int x, int y)
        {
            return _array[y * _dimension1 + x];
        }
        public bool Set(int x, int y, bool val)
        {
            return _array[y * _dimension1 + x] = val;
        }
        public bool this[int x, int y]
        {
            get { return Get(x, y); }
            set { Set(x, y, value); }
        }
    }
}

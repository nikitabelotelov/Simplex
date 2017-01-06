using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehroz;

namespace Simplex
{
    class MatrixTask
    {
        Fraction[,] Matrix;
        int vars;
        int conds;
        public MatrixTask(int vars, int conds)
        {
            Matrix = new Fraction[conds, vars];
        }
        public Fraction this[int i, int j]
        {
            get
            {
                return Matrix[i, j];
            }
            set
            {
                Matrix[i, j] = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehroz;

namespace Simplex
{
    public class MatrixTask
    {
        Fraction[,] Matrix;
        Fraction[] IndepConst;
        Fraction[] Func;
        int vars;
        int conds;

        public int Conds
        {
            get
            {
                return conds;
            }
        }

        public int Vars
        {
            get
            {
                return vars;
            }
        }

        public MatrixTask(int vars, int conds)
        {
            this.vars = vars;
            this.conds = conds;
            Matrix = new Fraction[conds, vars];
            IndepConst = new Fraction[conds];
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

        public void SetFuncCoef(Fraction fr, int i)
        {
            Func[i] = fr;
        }

        public Fraction this[int i]
        {
            get
            {
                return IndepConst[i];
            }
            set
            {
                IndepConst[i] = value;
            }
        }
    }
}

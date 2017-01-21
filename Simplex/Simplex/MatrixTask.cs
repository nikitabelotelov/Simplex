using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehroz;

namespace Simplex
{
    [Serializable]
    public class MatrixTask
    {
        Fraction[,] Matrix;
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
            Func = new Fraction[vars + 1];
            Matrix = new Fraction[conds, vars + 1];
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
        
        public Fraction this[int i]
        {
            get
            {
                return Func[i];
            }
        }

        public void SetFuncCoef(Fraction fr, int i)
        {
            Func[i] = fr;
        }

        public Fraction GetFuncCoef(int i)
        {
            return Func[i];
        }

        public Fraction[, ] GetMatrix()
        {
            return Matrix;
        }
    }
}

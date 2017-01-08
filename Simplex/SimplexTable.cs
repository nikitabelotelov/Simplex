using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehroz;

namespace Simplex
{
    public class SimplexTable
    {
        int[] basis;
        MatrixTask mTask;
        Fraction[,] coefMatrix;
        Fraction[] func;
        List<List<Fraction>> table;

        public SimplexTable(MatrixTask mTask, int[] basis)
        {
            this.basis = basis;

            this.mTask = mTask;
            coefMatrix = mTask.GetMatrix();
            table = new List<List<Fraction>>(basis.Length);

            func = new Fraction[mTask.Vars + 1];

            for(int i = 0; i < mTask.Vars - basis.Length + 1; i++)
            {
                func[i] = new Fraction(0);
            }

            for(int i = 0; i < basis.Length; i++)
            {
                table.Add(new List<Fraction>((mTask.Vars - basis.Length) + 1));
            }

            GenerateTable();
        }

        public int[] Basis
        {
            get
            {
                return basis;
            }
        }

        public Fraction this[int i, int j]
        {
            get
            {
                return table[i][j];
            }
        }

        public Fraction this[int i]
        {
            get
            {
                return func[i];
            }
        }

        public int Vars
        {
            get
            {
                return mTask.Vars;
            }
        }

        public int Conds
        {
            get
            {
                return mTask.Conds;
            }
        }

        private void GenerateTable()
        {
            for (int i = 0; i < basis.Length; i++)
            {

                int k = i;
                while(k < basis.Length && coefMatrix[k, basis[i]] == 0)
                    k++;

                if (k == basis.Length)
                    throw new Exception("Incorrect conditions");

                if (i != k)
                {
                    for (int j = 0; j < mTask.Vars + 1; j++)
                    {
                        Fraction temp = coefMatrix[i, j];
                        coefMatrix[i, j] = coefMatrix[k, j];
                        coefMatrix[k, j] = temp;
                    }
                }

                Fraction divider = coefMatrix[i, basis[i]];
                for (int j = 0; j < mTask.Vars + 1; j++)
                    coefMatrix[i, j] /= divider;

                for (int j = i + 1; j < basis.Length; j++)
                {
                    Fraction multiplier = coefMatrix[j, basis[i]];
                    for(int t = 0; t < mTask.Vars + 1; t++)
                    {
                        coefMatrix[j, t] -= coefMatrix[i, t] * multiplier;
                    }
                }

                for (int j = i - 1; j >= 0; j--)
                {
                    Fraction multiplier = coefMatrix[j, basis[i]];
                    for (int t = 0; t < mTask.Vars + 1; t++)
                    {
                        coefMatrix[j, t] -= coefMatrix[i, t] * multiplier;
                    }
                }
            }

            for (int i = 0; i < mTask.Vars + 1; i++)
            {
                if (!basis.ToList().Contains(i))
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        table[j].Add(coefMatrix[j, i]);
                    }
                }
            }

            for(int i = 0; i < Vars - basis.Length + 1; i++)//Function coefs
            {
                for(int j = 0; j < basis.Length; j++)
                {
                    func[i] += -table[j][i] * mTask[basis[j]];
                }
            }

            for(int i = 0, c = 0; i < Vars + 1; i++)
            {
                if (!basis.Contains(i))
                {
                    func[c++] += mTask[i];
                }
            }
        }
    }
}

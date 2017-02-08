using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehroz;
using System.Windows;

namespace Simplex
{
    public enum SimplexState
    {
        NoSolutionExists,
        Solved,
        NotFinished
    }

    public enum SynthBasisState
    {
        Inconsisted,
        Completed,
        NoSolutionExists,
        NotCompleted
    }


    public class SimplexTable
    {
        int[] basis;
        int[] notBasis;
        MatrixTask mTask;
        Fraction[,] coefMatrix;
        Fraction[] func;
        List<List<Fraction>> table;
        Fraction[,] SyntheticBasisMatrix;
        public bool isSyntheticBasis = false;


        public SimplexTable Copy()
        {
            return new SimplexTable(basis, mTask, coefMatrix, func, table, notBasis);
        }

        public int NonBasisVars
        {
            get
            {
                return mTask.Vars - basis.Length;
            }
        }

        public int BasisVars
        {
            get
            {
                return basis.Length;
            }
        }

        public int Vars
        {
            get
            {
                return mTask.Vars;
            }
        }

        public SimplexTable(MatrixTask mTask, int[] basis)
        {
            this.basis = basis;
            this.mTask = mTask;
            notBasis = new int[NonBasisVars];
            for (int i = 0, c = 0; i < Vars; i++)
            {
                if (!basis.ToList().Contains(i))
                {
                    notBasis[c++] = i;
                }
            }

            coefMatrix = mTask.GetMatrix();
            table = new List<List<Fraction>>(basis.Length);

            func = new Fraction[Vars + 1];

            for (int i = 0; i < NonBasisVars + 1; i++)
            {
                func[i] = new Fraction(0);
            }

            for (int i = 0; i < BasisVars; i++)
            {
                table.Add(new List<Fraction>(NonBasisVars + 1));
            }

            GenerateTable();
        }

        public SimplexTable(MatrixTask m)
        {
            isSyntheticBasis = true;
            this.mTask = m;
            Fraction[] bottRow = new Fraction[mTask.Vars + 1];

            for (int i = 0; i < bottRow.Length; i++)
                bottRow[i] = 0;

            coefMatrix = mTask.GetMatrix();
            makeAllConstPositive();
            SyntheticBasisMatrix = mTask.GetMatrix();
            for (int i = 0; i < bottRow.Length; i++)
            {
                for (int j = 0; j < mTask.Conds; j++)
                {
                    bottRow[i] += mTask[j, i];
                }
            }

            for (int i = 0; i < bottRow.Length; i++)
                bottRow[i] = -bottRow[i];

            //Начало шагов метода искусственного базиса
            SynthBasis(bottRow);


        }

        private void SynthBasis(Fraction[] bottRow)
        {
            int[] synthBasis = new int[mTask.Conds];
            int[] nonBasisVars = new int[mTask.Vars];

            for (int i = 0; i < mTask.Vars; i++)
                nonBasisVars[i] = i;

            for (int i = 0; i < mTask.Conds; i++)
                synthBasis[i] = i + mTask.Vars;
            int col = 0;
            SynthBasisState state;

            while (!SynthBasisCompl(bottRow, nonBasisVars, out state))
            {
                
                for (int i = col; i < mTask.Vars; i++)
                {
                    if (bottRow[i] < 0)//Kostyl'
                    {
                        col = i;
                        break;
                    }
                }

                Fraction refEl = new Fraction(0);//Опорный элемент
                bool refElSet = false;
                int refElIndexI = 0;
                int refElIndexJ = 0;
                Fraction quot = new Fraction(0);
                bool allNeg = true;

                for (int i = 0; i < mTask.Conds; i++)//Находим опорный элемент
                {
                    if (SyntheticBasisMatrix[i, col] >= 0)
                    {
                        allNeg = false;
                        if (!refElSet)
                        {
                            refElSet = true;
                            refElIndexI = i;
                            refElIndexJ = col;
                            refEl = SyntheticBasisMatrix[i, col];
                            quot = SyntheticBasisMatrix[i, mTask.Vars] / SyntheticBasisMatrix[i, col];
                        }
                        else if ((SyntheticBasisMatrix[i, mTask.Vars] / SyntheticBasisMatrix[i, col]) < quot)
                        {
                            refElIndexI = i;
                            refElIndexJ = col;
                            refEl = SyntheticBasisMatrix[i, col];
                            quot = (SyntheticBasisMatrix[i, mTask.Vars] / SyntheticBasisMatrix[i, col]);
                        }
                    }
                }

                if (allNeg == true)
                {
                    state = SynthBasisState.NoSolutionExists;
                }

                for (int i = 0; i < mTask.Vars + 1; i++)//Преобразование опорной строки
                {
                    if (i != col)
                    {
                        SyntheticBasisMatrix[refElIndexI, i] *= (1 / refEl);
                    }
                    else
                    {
                        SyntheticBasisMatrix[refElIndexI, i] = 1 / refEl;
                    }
                }

                for (int i = 0; i < mTask.Conds; i++)//Преобразуем остальные строки
                {
                    if (i != refElIndexI)
                    {
                        for (int j = 0; j < mTask.Vars + 1; j++)
                        {
                            if (j != refElIndexJ)
                            {
                                SyntheticBasisMatrix[i, j] -= SyntheticBasisMatrix[refElIndexI, j] * SyntheticBasisMatrix[i, refElIndexJ];
                            }
                        }
                    }
                }

                for (int i = 0; i < mTask.Vars + 1; i++)//Преобразуем последнюю строку
                {
                    if (i != refElIndexJ)
                    {
                        bottRow[i] -= SyntheticBasisMatrix[refElIndexI, i] * bottRow[refElIndexJ];
                    }
                }

                for (int i = 0; i < mTask.Conds; i++)//Преобразуем столбец
                {
                    if (i != refElIndexI)
                    {
                        SyntheticBasisMatrix[i, refElIndexJ] *= -(1 / refEl);
                    }
                }

                bottRow[refElIndexJ] *= -(1 / refEl);

                int tmp = nonBasisVars[refElIndexJ];//Меняем местами базисную переменную с небазисной
                nonBasisVars[refElIndexJ] = synthBasis[refElIndexI];
                synthBasis[refElIndexI] = tmp;
            }
            if (state == SynthBasisState.Completed)
            {
                basis = synthBasis;

                notBasis = new int[NonBasisVars];
                for (int i = 0, c = 0; i < nonBasisVars.Length; i++)
                {
                    if (nonBasisVars[i] < Vars)
                    {
                        notBasis[c++] = nonBasisVars[i];
                    }
                }

                table = new List<List<Fraction>>(basis.Length);
                for (int i = 0; i < BasisVars; i++)
                    table.Add(new List<Fraction>(NonBasisVars + 1));

                func = new Fraction[Vars + 1];
                for (int i = 0; i < NonBasisVars + 1; i++)
                    func[i] = new Fraction(0);

                AddCoefsToTable(SyntheticBasisMatrix);
            }
            else if (state == SynthBasisState.Inconsisted)
            {
                MessageBox.Show("Система условий несовместна");
            }
            else if (state == SynthBasisState.NoSolutionExists)
            {
                MessageBox.Show("Система не имеет решения");
            }
        }

        private bool SynthBasisCompl(Fraction[] bR, int[] curNonBasis, out SynthBasisState state)
        {
            bool ended = true;
            for (int i = 0; i < bR.Length - 1; i++)
            {
                if (bR[i] < 0)
                {
                    ended = false;
                    break;
                }
            }
            if (ended)
            {
                state = SynthBasisState.Completed;
                return true;
            }
            else
            {
                state = SynthBasisState.NotCompleted;
                return false;
            }

            /* bool allZeroes = true;
             bool allZeroes = true;
             for(int i = 0; i < bR.Length - 1; i++)
             {
                 if (curNonBasis[i] < mTask.Vars)
                 {
                     if (bR[i] != 0)
                         allZeroes = false;
                 }
             }
             if (bR[bR.Length - 1] != 0)
                 allZeroes = false;
             if (allZeroes)
         {
             state = SynthBasisState.Completed;
             return true;
         }
            state = SynthBasisState.NotCompleted;
            return false;*/

        }

        private void makeAllConstPositive()
        {
            for (int i = 0; i < mTask.Conds; i++)
            {
                if (mTask[i, mTask.Vars] < 0)
                {
                    for (int j = 0; j < mTask.Vars + 1; j++)
                    {
                        mTask[i, j] = -mTask[i, j];
                    }
                }

            }
        }

        SimplexTable(int[] basis, MatrixTask mTask, Fraction[,] coefMatrix, Fraction[] func, List<List<Fraction>> table, int[] notBasis)
        {
            this.basis = new int[basis.Length];
            basis.CopyTo(this.basis, 0);

            this.mTask = mTask;
            this.coefMatrix = new Fraction[coefMatrix.GetLength(0), coefMatrix.GetLength(1)];
            for (int i = 0; i < coefMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < coefMatrix.GetLength(1); j++)
                {
                    this.coefMatrix[i, j] = new Fraction(coefMatrix[i, j].ToString());
                }
            }

            this.func = new Fraction[func.Length];
            func.CopyTo(this.func, 0);

            this.table = new List<List<Fraction>>();
            for (int i = 0; i < table.Count; i++)
            {
                this.table.Add(new List<Fraction>());
                for (int j = 0; j < table[i].Count; j++)
                {
                    this.table[i].Add(new Fraction(table[i][j].ToString()));
                }
            }
            this.notBasis = new int[notBasis.Length];
            notBasis.CopyTo(this.notBasis, 0);
        }

        public int[] Basis
        {
            get
            {
                return basis;
            }
        }

        public int[] NotBasis
        {
            get
            {
                return notBasis;
            }
        }

        public Fraction this[int i, int j]
        {
            get
            {
                return table[i][j];
            }
            set
            {
                table[i][j] = value;
            }
        }

        public Fraction this[int i]
        {
            get
            {
                return func[i];
            }
            set
            {
                func[i] = value;
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
            for (int i = 0; i < BasisVars; i++)
            {

                int k = i;
                while(k < basis.Length && coefMatrix[k, basis[i]] == 0)
                    k++;

                if (k == BasisVars)
                    throw new Exception("Incorrect conditions");

                if (i != k)//Меняем местами строки, чтобы получить ровную "лесенку"
                {
                    for (int j = 0; j < mTask.Vars + 1; j++)
                    {
                        Fraction temp = coefMatrix[i, j];
                        coefMatrix[i, j] = coefMatrix[k, j];
                        coefMatrix[k, j] = temp;
                    }
                }

                Fraction divider = coefMatrix[i, basis[i]];//Делим все элементы на коэффициент перед базисной переменной
                for (int j = 0; j < mTask.Vars + 1; j++)
                    coefMatrix[i, j] /= divider;

                for (int j = i + 1; j < BasisVars; j++)//Вычитаем строки (Прямой ход)
                {
                    Fraction multiplier = coefMatrix[j, basis[i]];
                    for(int t = 0; t < mTask.Vars + 1; t++)
                    {
                        coefMatrix[j, t] -= coefMatrix[i, t] * multiplier;
                    }
                }

                for (int j = i - 1; j >= 0; j--)//Вычитаем строки (Обратный ход)
                {
                    Fraction multiplier = coefMatrix[j, basis[i]];
                    for (int t = 0; t < Vars + 1; t++)
                    {
                        coefMatrix[j, t] -= coefMatrix[i, t] * multiplier;
                    }
                }
            }

            AddCoefsToTable(coefMatrix);
        }

        private void AddCoefsToTable(Fraction[,] coefMatrix)
        {

            for (int i = 0; i < Vars + 1; i++)//Записываем коэффициенты в симплекс таблицу
            {
                if (!basis.ToList().Contains(i))
                {
                    for (int j = 0; j < BasisVars; j++)
                    {
                        table[j].Add(coefMatrix[j, i]);
                    }
                }
            }

            for (int i = 0; i < NonBasisVars + 1; i++)//Вычисляем коэффициенты в функции
            {
                for (int j = 0; j < basis.Length; j++)
                {
                    func[i] += -table[j][i] * mTask[basis[j]];
                }
            }

            for (int i = 0, c = 0; i < Vars + 1; i++)
            {
                if (!basis.Contains(i))
                {
                    func[c++] += mTask[i];
                }
            }
        }

        public SimplexTable NextSimplexState(out SimplexState state)
        {
            SimplexTable nextSimplexTable = new SimplexTable(basis, mTask, coefMatrix, func, table, notBasis);

            bool solved = true;
            for(int i = 0; i < nextSimplexTable.NonBasisVars; i++)//Проверяем, закончился ли симплекс
            {
                if(nextSimplexTable[i] < 0)
                {
                    solved = false;
                }
            }
            if (solved)
            {
                state = SimplexState.Solved;
                return null;
            }
            else
            {
                for (int i = 0; i < nextSimplexTable.NonBasisVars; i++)//Ищем полностью отрицательные столбцы
                {
                    bool allNeg = true;
                    for (int j = 0; j < nextSimplexTable.BasisVars; j++)
                    {
                        if (nextSimplexTable[j, i] >= 0)
                        {
                            allNeg = false;
                            break;
                        }
                    }
                    if(allNeg)
                    {
                        state = SimplexState.NoSolutionExists;
                        return null;
                    }
                }

                //Начало преобразований симплекс-таблицы

                Fraction refEl = new Fraction(0);//Опорный элемент
                bool refElSet = false;
                int refElIndexI = 0;
                int refElIndexJ = 0;
                Fraction quot = new Fraction(0);

                int col = -1;
                for (int i = 0; i < mTask.Vars; i++)
                {
                    if (func[i] < 0)
                    {
                        col = i;
                        break;
                    }
                }

                for (int i = 0; i < basis.Length; i++)//Находим опорный элемент
                {
                    if (table[i][col] > 0)
                    {
                        if (!refElSet)
                        {
                            refElSet = true;
                            refElIndexI = i;
                            refElIndexJ = col;
                            refEl = table[i][col];
                            quot = table[i][NonBasisVars] / table[i][col];
                        }
                        else if ((table[i][NonBasisVars] / table[i][col]) < quot)
                        {
                            refElIndexI = i;
                            refElIndexJ = col;
                            refEl = table[i][col];
                        }
                    }
                }

                for (int i = 0; i < NonBasisVars + 1; i++)//Преобразование опорной строки
                {
                    if (i != col)
                    {
                        nextSimplexTable[refElIndexI, i] *= (1 / refEl);
                    }
                    else
                    {
                        nextSimplexTable[refElIndexI, i] = 1 / refEl;
                    }
                }

                for (int i = 0; i < mTask.Conds; i++)//Преобразуем остальные строки
                {
                    if (i != refElIndexI)
                    {
                        for (int j = 0; j < NonBasisVars + 1; j++)
                        {
                            if (j != refElIndexJ)
                            {
                                nextSimplexTable[i, j] -= nextSimplexTable[refElIndexI, j] * nextSimplexTable[i, refElIndexJ];
                            }
                        }
                    }
                }

                for (int i = 0; i < NonBasisVars + 1; i++)//Преобразуем последнюю строку
                {
                    if (i != refElIndexJ)
                    {
                        nextSimplexTable[i] -= nextSimplexTable[refElIndexI, i] * nextSimplexTable[refElIndexJ];
                    }
                }

                for (int i = 0; i < mTask.Conds; i++)//Преобразуем столбец
                {
                    if (i != refElIndexI)
                    {
                        nextSimplexTable[i, refElIndexJ] *= -(1 / refEl);
                    }
                }

                nextSimplexTable[refElIndexJ] *= -(1 / refEl);

                int tmp = nextSimplexTable.notBasis[refElIndexJ];//Меняем местами базисную переменную с небазисной
                nextSimplexTable.notBasis[refElIndexJ] = nextSimplexTable.basis[refElIndexI];
                nextSimplexTable.basis[refElIndexI] = tmp;

            }

            state = SimplexState.NotFinished;
            return nextSimplexTable;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mehroz;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Simplex
{
    /// <summary>
    /// Логика взаимодействия для taskCreationWindow.xaml
    /// </summary>
    /// 
    public partial class TaskCreationWindow : Window
    {
        private int VarNum;
        private int CondNum;
        private const int CellWidth = 30;
        private const int CellHeight = 20;
        MatrixTask matrixTask;
        TextBox[] MatrixTBs;
        TextBox[] IndepConstTBs;
        TextBox[] FunctionTBs;

        public TaskCreationWindow()
        {
            InitializeComponent();
        }

        private bool SetMatrixInfo()
        {
            if (condNumTB.Text == "" || varNumTB.Text == "")
                return false;
            int vars, conds;
            if (!int.TryParse(varNumTB.Text, out vars))
                return false;
            else if (!int.TryParse(condNumTB.Text, out conds))
                return false;
            else if (vars < 1 || vars > 16 || conds < 1 || conds > 16)
                return false;
            else
            {
                VarNum = vars;
                CondNum = conds;
                return true;
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SetMatrixInfo())
                MessageBox.Show("Количество переменных и условий должны быть числами от 1 до 16.");
            else
            {
                matrixTask = new MatrixTask(VarNum, CondNum);
                GenMatrixField();
                GenMatrixFieldTBs();
                GenVarGrid();
                GenFuncGrid();
            }
            
        }
        private void GenMatrixField()
        {
            MatrixField.RowDefinitions.Clear();
            MatrixField.ColumnDefinitions.Clear();
            for (int i = 0; i < CondNum; i++)
            {
                MatrixField.RowDefinitions.Add(new RowDefinition());
                MatrixField.RowDefinitions[i].Height = new GridLength(CellHeight);
            }
            for (int i = 0; i < VarNum + 1; i++)
            {
                MatrixField.ColumnDefinitions.Add(new ColumnDefinition());
                MatrixField.ColumnDefinitions[i].Width = new GridLength(CellWidth);
            }
        }

        private void GenMatrixFieldTBs()
        {
            MatrixField.Children.Clear();
            MatrixTBs = new TextBox[VarNum * CondNum];
            IndepConstTBs = new TextBox[CondNum];
            for (int i = 0; i < MatrixTBs.Length; i++)
            {
                MatrixTBs[i] = new TextBox();
                Grid.SetColumn(MatrixTBs[i], i % VarNum);
                Grid.SetRow(MatrixTBs[i], i / VarNum);
                MatrixTBs[i].Text = 0.ToString();
                MatrixTBs[i].Width = CellWidth;
                MatrixTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                MatrixTBs[i].Height = CellHeight;
                MatrixField.Children.Add(MatrixTBs[i]);
            }
            for (int i = 0; i < CondNum; i++)
            {
                IndepConstTBs[i] = new TextBox();
                IndepConstTBs[i].Width = CellWidth;
                IndepConstTBs[i].Height = CellHeight;
                IndepConstTBs[i].Text = 0.ToString();
                IndepConstTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(IndepConstTBs[i], VarNum);
                Grid.SetRow(IndepConstTBs[i], i);
                MatrixField.Children.Add(IndepConstTBs[i]);
            }
        }

        private void GenVarGrid()
        {
            VarGrid.ColumnDefinitions.Clear();
            VarGrid.Children.Clear();
            for(int i = 0; i < VarNum; i++)
            {
                VarGrid.ColumnDefinitions.Add(new ColumnDefinition());
                VarGrid.ColumnDefinitions[i].Width = new GridLength(CellWidth);
                Label l = new Label();
                l.Content = "x" + i.ToString();
                VarGrid.Children.Add(l);
                Grid.SetColumn(l, i);
            }
            VarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            VarGrid.ColumnDefinitions[VarNum].Width = new GridLength(CellWidth);
            Label la = new Label();
            la.Content = "b";
            VarGrid.Children.Add(la);
            Grid.SetColumn(la, VarNum);
        }

        private void GenFuncGrid()
        {
            FunctionGrid.Children.Clear();
            FunctionGrid.ColumnDefinitions.Clear();
            FunctionTBs = new TextBox[VarNum + 1];
            for(int i = 0; i < VarNum + 1; i++)
            {
                FunctionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                FunctionGrid.ColumnDefinitions[i].Width = new GridLength(CellWidth);
                FunctionTBs[i] = new TextBox();
                FunctionTBs[i].Width = CellWidth;
                FunctionTBs[i].Height = CellHeight;
                FunctionTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                FunctionTBs[i].Text = 0.ToString();
                Grid.SetColumn(FunctionTBs[i], i);
                FunctionGrid.Children.Add(FunctionTBs[i]);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < CondNum; i++)
            {
                for(int j = 0; j < VarNum; j++)
                {
                    matrixTask[i, j] = new Fraction(MatrixTBs[i * VarNum + j].Text);
                }
                matrixTask[i, VarNum] = new Fraction(IndepConstTBs[i].Text);
            }
            for(int i = 0; i < VarNum + 1; i++)
                matrixTask.SetFuncCoef(new Fraction(FunctionTBs[i].Text), i);
            MainWindow.CurMatrixTask = matrixTask;
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream("user.dat",
                FileMode.Create, FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, matrixTask);
            this.Close();
        }
    }

}

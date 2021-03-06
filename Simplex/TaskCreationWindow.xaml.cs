﻿using System.Windows;
using System.Windows.Controls;
using Mehroz;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;

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

        public delegate void TaskSetMethod(MatrixTask m, string FileName);

        public event TaskSetMethod OnTaskEntered;

        public TaskCreationWindow()
        {
            InitializeComponent();
        }

        public TaskCreationWindow(MatrixTask mTask)
        {
            InitializeComponent();
            matrixTask = mTask;
            VarNum = mTask.Vars;
            CondNum = mTask.Conds;
            GenMatrixField();
            GenMatrixFieldTBs();
            GenVarGrid();
            GenFuncGrid();
            FillTbs();
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

        private void FillTbs()
        {
            for (int i = 0; i < CondNum; i++)
            {
                for (int j = 0; j < VarNum; j++)
                {
                    MatrixTBs[i * VarNum + j].Text = matrixTask[i, j].ToString();
                }
                IndepConstTBs[i].Text = matrixTask[i, VarNum].ToString();
            }
            for (int i = 0; i < VarNum + 1; i++)
                FunctionTBs[i].Text = matrixTask[i].ToString();
            condNumTB.Text = CondNum.ToString();
            varNumTB.Text = VarNum.ToString();
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
            for (int i = 0; i < CondNum; i++)
            {
                for (int j = 0; j < VarNum; j++)
                {
                    MatrixTBs[j + i * VarNum] = new TextBox();
                    Grid.SetColumn(MatrixTBs[j + i * VarNum], j % VarNum);
                    Grid.SetRow(MatrixTBs[j + i * VarNum], i);
                    MatrixTBs[j + i * VarNum].Text = 0.ToString();
                    MatrixTBs[j + i * VarNum].Width = CellWidth;
                    MatrixTBs[j + i * VarNum].HorizontalAlignment = HorizontalAlignment.Left;
                    MatrixTBs[j + i * VarNum].Height = CellHeight;
                    MatrixTBs[j + i * VarNum].GotFocus += TaskCreationWindow_GotFocus;
                    MatrixTBs[j + i * VarNum].LostFocus += TaskCreationWindow_LostFocus;
                    MatrixField.Children.Add(MatrixTBs[j + i * VarNum]);
                }

                IndepConstTBs[i] = new TextBox();
                IndepConstTBs[i].Width = CellWidth;
                IndepConstTBs[i].Height = CellHeight;
                IndepConstTBs[i].Text = 0.ToString();
                IndepConstTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                IndepConstTBs[i].GotFocus += TaskCreationWindow_GotFocus;
                IndepConstTBs[i].LostFocus += TaskCreationWindow_LostFocus;
                Grid.SetColumn(IndepConstTBs[i], VarNum);
                Grid.SetRow(IndepConstTBs[i], i);
                MatrixField.Children.Add(IndepConstTBs[i]);
            }
        }

        private void TaskCreationWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "";
        }

        private void GenVarGrid()
        {
            VarGrid.ColumnDefinitions.Clear();
            VarGrid.Children.Clear();
            for (int i = 0; i < VarNum; i++)
            {
                VarGrid.ColumnDefinitions.Add(new ColumnDefinition());
                VarGrid.ColumnDefinitions[i].Width = new GridLength(CellWidth);
                Label l = new Label();
                l.Content = "x" + (i + 1).ToString();
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
            for (int i = 0; i < VarNum + 1; i++)
            {
                FunctionGrid.ColumnDefinitions.Add(new ColumnDefinition());
                FunctionGrid.ColumnDefinitions[i].Width = new GridLength(CellWidth);
                FunctionTBs[i] = new TextBox();
                FunctionTBs[i].Width = CellWidth;
                FunctionTBs[i].Height = CellHeight;
                FunctionTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                FunctionTBs[i].Text = 0.ToString();
                FunctionTBs[i].GotFocus += TaskCreationWindow_GotFocus;
                FunctionTBs[i].LostFocus += TaskCreationWindow_LostFocus;
                Grid.SetColumn(FunctionTBs[i], i);
                FunctionGrid.Children.Add(FunctionTBs[i]);
            }
        }

        private void TaskCreationWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                (sender as TextBox).Text = 0.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CondNum; i++)
            {
                for (int j = 0; j < VarNum; j++)
                {
                    matrixTask[i, j] = new Fraction(MatrixTBs[i * VarNum + j].Text);
                }
                matrixTask[i, VarNum] = new Fraction(IndepConstTBs[i].Text);
            }
            for (int i = 0; i < VarNum + 1; i++)
                matrixTask[i] = new Fraction(FunctionTBs[i].Text);

            string fileName = "";
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Saved tasks(*.smx)|*.smx";
            dialog.CheckFileExists = false;

            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }
            if (fileName == "")
            {
                MessageBox.Show("File not selected");
                return;
            }

            //XmlSerializer serializer = new XmlSerializer(typeof(MatrixTask));
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fStream = new FileStream(fileName,
                FileMode.Create, FileAccess.Write, FileShare.None);
            binFormat.Serialize(fStream, matrixTask);
            fStream.Close();

            OnTaskEntered(matrixTask, dialog.SafeFileName);
            this.Close();
        }
    }
}

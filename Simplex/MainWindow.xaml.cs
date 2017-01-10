using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simplex
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public MatrixTask CurMatrixTask;
        static public SimplexTable CurSimplexTable;
        static public List<SimplexTable> AllSimplexStates;
        static public int pos;
        static public SimplexState state;
        static public string curFileName = "";

        int CellWidth = 50;
        int CellHeight = 30;
        CheckBox[] cbs = null;
        int[] basis;
        

        public MainWindow()
        {
            InitializeComponent();
            CurMatrixTask = null;
            AllSimplexStates = new List<SimplexTable>();
        }

        public void GenSimplexTableView()
        {
            if (CurMatrixTask != null && CurSimplexTable != null)
            {
                genSideVarGrid();
                genTopVarGrid();
                genSimplexMatrix();
            }
        }

        void genSideVarGrid()
        {
            SideVarGrid.Children.Clear();
            SideVarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            SideVarGrid.ColumnDefinitions[0].Width = new GridLength(CellWidth);
            for(int i = 0; i < CurSimplexTable.Basis.Length; i++)
            {
                Label l = new Label();
                l.Width = CellWidth;
                l.Height = CellHeight;
                l.Content = "x" + (CurSimplexTable.Basis[i] + 1).ToString();
                SideVarGrid.RowDefinitions.Add(new RowDefinition());
                SideVarGrid.RowDefinitions[i].Height = new GridLength(CellHeight);
                SideVarGrid.Children.Add(l);
                Grid.SetRow(l, i);
            }

            Label la = new Label();
            la.Width = CellWidth;
            la.Height = CellHeight;
            la.Content = "f";
            SideVarGrid.RowDefinitions.Add(new RowDefinition());
            SideVarGrid.RowDefinitions[CurSimplexTable.Basis.Length].Height = new GridLength(CellHeight);
            SideVarGrid.Children.Add(la);
            Grid.SetRow(la, CurSimplexTable.Basis.Length);

        }

        void genTopVarGrid()
        {
            TopVarGrid.Children.Clear();
            TopVarGrid.RowDefinitions.Add(new RowDefinition());
            TopVarGrid.RowDefinitions[0].Height = new GridLength(CellHeight);

            for (int i = 0, c = 0; i < CurSimplexTable.Vars; i++)
            {
                if (!CurSimplexTable.Basis.Contains(i))
                {
                    Label l = new Label();
                    l.Width = CellWidth;
                    l.Height = CellHeight;
                    l.Content = "x" + (i + 1).ToString();
                    TopVarGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    TopVarGrid.ColumnDefinitions[c].Width = new GridLength(CellWidth);
                    TopVarGrid.Children.Add(l);
                    Grid.SetColumn(l, c);
                    c++;
                }
            }

            Label la = new Label();
            la.Width = CellWidth;
            la.Height = CellHeight;
            la.Content = "b";
            TopVarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            TopVarGrid.ColumnDefinitions[CurSimplexTable.Vars - CurSimplexTable.Basis.Length].Width = new GridLength(CellWidth);
            TopVarGrid.Children.Add(la);
            Grid.SetColumn(la, CurSimplexTable.Vars - CurSimplexTable.Basis.Length);

        }

        void genSimplexMatrix()
        {
            CoefGrid.RowDefinitions.Clear();
            CoefGrid.ColumnDefinitions.Clear();
            for(int i = 0; i < CurSimplexTable.Basis.Length; i++)
            {
                CoefGrid.RowDefinitions.Add(new RowDefinition());
                CoefGrid.RowDefinitions[i].Height = new GridLength(CellHeight);

                for(int j = 0; j < CurSimplexTable.Vars - CurSimplexTable.Basis.Length + 1; j++)
                {
                    CoefGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    CoefGrid.ColumnDefinitions[j].Width = new GridLength(CellWidth);
                    TextBox tb = new TextBox();
                    tb.Focusable = false;
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j);
                    tb.Width = CellWidth;
                    tb.Height = CellHeight;
                    CoefGrid.Children.Add(tb);
                    tb.Text = CurSimplexTable[i, j].ToString();
                }
            }
            CoefGrid.RowDefinitions.Add(new RowDefinition());
            CoefGrid.RowDefinitions[CurSimplexTable.Conds].Height = new GridLength(CellHeight);
            for (int i = 0; i < CurSimplexTable.Vars - CurSimplexTable.Basis.Length + 1; i++)
            {
                TextBox tb = new TextBox();
                tb.Focusable = false;
                Grid.SetRow(tb, CurSimplexTable.Basis.Length);
                Grid.SetColumn(tb, i);
                tb.Width = CellWidth;
                tb.Height = CellHeight;
                CoefGrid.Children.Add(tb);
                tb.Text = CurSimplexTable[i];
            }
        }

        void genBasisGrid()
        {
            BasisGrid.Children.Clear();
            BasisGrid.RowDefinitions.Clear();
            BasisGrid.ColumnDefinitions.Clear();
            BasisGrid.RowDefinitions.Add(new RowDefinition());
            BasisGrid.RowDefinitions[0].Height = new GridLength(25);
            BasisGrid.RowDefinitions.Add(new RowDefinition());
            BasisGrid.RowDefinitions[1].Height = new GridLength(20);
            cbs = new CheckBox[CurMatrixTask.Vars];
            for (int i = 0; i < CurMatrixTask.Vars; i++)
            {
                Label l = new Label();
                l.Width = 30;
                l.Height = 30;
                l.Content = "x" + (i + 1).ToString();
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.VerticalAlignment = VerticalAlignment.Top;
                BasisGrid.ColumnDefinitions.Add(new ColumnDefinition());
                BasisGrid.ColumnDefinitions[i].Width = new GridLength(30);
                BasisGrid.Children.Add(l);
                cbs[i] = new CheckBox();
                Grid.SetColumn(l, i);
                Grid.SetRow(l, 0);
                Grid.SetColumn(cbs[i], i);
                Grid.SetRow(cbs[i], 1);
                BasisGrid.Children.Add(cbs[i]);
                cbs[i].IsChecked = false;
            }
        }

        private void createTaskClicked(object sender, RoutedEventArgs e)
        {
            TaskCreationWindow taskCreationWindow = new TaskCreationWindow();
            taskCreationWindow.Show();
        }

        private bool setBasis()
        {
            int c = 0;
            if (cbs == null)
            {
                MessageBox.Show("Please, set a basis");
                return false;
            }
            for (int i = 0; i < cbs.Length; i++)
            {
                if (cbs[i].IsChecked.Equals(true))
                    basis[c++] = i;
            }
            if (c != CurMatrixTask.Conds)
            {
                MessageBox.Show("Wrong basis");
                return false;
            }
            return true;
        }

        private void StartSimplexButton_Click(object sender, RoutedEventArgs e)
        { 
            if (CurMatrixTask == null)
            {
                MessageBox.Show("Please, set a task");
                return;
            }
            basis = new int[CurMatrixTask.Conds];
            if (!BasisCheckbox.IsChecked.Equals(true))
            {
                if (!setBasis())
                {
                    return;
                }
            }
            NextStepButton.IsEnabled = true;
            PrevStepButton.IsEnabled = true;
            fileNameLabel.Content = curFileName;
            BasisGrid.Children.Clear();
            if (CurMatrixTask != null)
            {
                if (BasisCheckbox.IsChecked.Equals(true))
                    CurSimplexTable = new SimplexTable(CurMatrixTask);
                else
                    CurSimplexTable = new SimplexTable(CurMatrixTask, basis);
                AllSimplexStates.Add(CurSimplexTable.Copy());
                pos = 0;
                state = SimplexState.NotFinished;
                if(StepByStepCB.IsChecked.Equals(false))
                {
                    while(state == SimplexState.NotFinished)
                    {
                        doNextStep();
                    }
                }
            }
            GenSimplexTableView();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            string fileName = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Saved tasks(*.smx)|*.smx";
            if(dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }
            if(fileName == "")
            {
                MessageBox.Show("File not selected");
                return;
            }
            Stream fStream = new FileStream(fileName, FileMode.Open);
            CurMatrixTask = binFormat.Deserialize(fStream) as MatrixTask;
            curFileName = dialog.SafeFileName;
            fileNameLabel.Content = dialog.SafeFileName;
        }

        private void CreateBasisButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurMatrixTask != null)
            {
                SideVarGrid.Children.Clear();
                TopVarGrid.Children.Clear();
                CoefGrid.Children.Clear();
                BasisGrid.Children.Clear();
                genBasisGrid();
            }
            else
            {
                MessageBox.Show("Please, set the task");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CurMatrixTask = null;
            CurSimplexTable = null;
            SideVarGrid.Children.Clear();
            TopVarGrid.Children.Clear();
            CoefGrid.Children.Clear();
            BasisGrid.Children.Clear();
            NextStepButton.IsEnabled = false;
            PrevStepButton.IsEnabled = false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (AllSimplexStates != null)
            {
                if (CurSimplexTable != null)
                {
                    if (pos + 1 != AllSimplexStates.Count)
                    {
                        CurSimplexTable = AllSimplexStates[pos + 1];
                        pos++;
                        GenSimplexTableView();
                        return;
                    }
                }
            }
            doNextStep();
        }

        private void doNextStep()
        {
            if (CurSimplexTable == null)
                return;
            CurSimplexTable = CurSimplexTable.NextSimplexState(out state);
            if (state == SimplexState.Solved)
            {
                CurSimplexTable = AllSimplexStates.Last();
                MessageBox.Show("Simplex finished");
                /*SolutionLabel.Content = "(";
                for (int i = 0; i < CurSimplexTable.Vars; i++)
                {
                    if (CurSimplexTable.Basis.Contains(i))
                    {
                        if (i == CurSimplexTable.Basis.Length - 1)
                        {
                            SolutionLabel.Content += CurSimplexTable[CurSimplexTable.Basis.ToList().IndexOf(i), CurSimplexTable.NonBasisVars];
                        }
                        else
                        {
                            SolutionLabel.Content += CurSimplexTable[CurSimplexTable.Basis.ToList().IndexOf(i), CurSimplexTable.NonBasisVars].ToString() + ", ";
                        }
                    }
                    else
                    {
                        if (i == CurSimplexTable.Basis.Length - 1)
                        {
                            SolutionLabel.Content += 0.ToString();
                        }
                        else
                        {
                            SolutionLabel.Content += 0.ToString() + ", ";
                        }
                    }
                }
                SolutionLabel.Content += ")" + " " + (-CurSimplexTable[CurSimplexTable.NonBasisVars + 1]).ToString();*/
                return;
            }
            else if (state == SimplexState.NotFinished)
            {
                AllSimplexStates.Add(CurSimplexTable.Copy());
                pos++;
                GenSimplexTableView();
            }
            else if (state == SimplexState.NoSolutionExists)
            {
                MessageBox.Show("No solution exists");
            }
        }

        private void PrevStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllSimplexStates != null)
            {
                if(pos == 0)
                {
                    MessageBox.Show("No previous steps");
                    return;
                }
                else
                {
                    CurSimplexTable = AllSimplexStates.ElementAt(pos - 1);
                    pos--;
                    GenSimplexTableView();
                }
            }
        }
    }
}

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
        private const int CellSize = 30;
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
                MatrixTask matrixTask = new MatrixTask(VarNum, CondNum);
                TextBox[] matrixTBs = new TextBox[VarNum * CondNum];
                MatrixField.Children.Clear();
                MatrixField.Margin = new Thickness(10, 76, 0, 0);
                MatrixField.Width = VarNum * CellSize;
                MatrixField.Height = CondNum * CellSize;
                for (int i = 0; i < matrixTBs.Length; i++)
                {
                    matrixTBs[i] = new TextBox();
                    matrixTBs[i].Text = 0.ToString();
                    matrixTBs[i].Width = CellSize;
                    matrixTBs[i].HorizontalAlignment = HorizontalAlignment.Left;
                    //matrixTBs[i].Height = CellSize;
                    matrixTBs[i].VerticalAlignment = VerticalAlignment.Center;
                    matrixTBs[i].Margin = new Thickness((i % VarNum) * CellSize, (i / VarNum) * CellSize, 0, 0);
                    MatrixField.Children.Add(matrixTBs[i]);
                }
            }
        }
        private void GenMatrixField()
        {

        }
    }
}

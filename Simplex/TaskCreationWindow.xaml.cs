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
        private int varNum;
        private int condNum;

        public TaskCreationWindow()
        {
            InitializeComponent();
        }

        private void matrixInfoChanged(object sender, TextChangedEventArgs e)
        {
            if (systemInfo == null)
                return;
            if(!checkMatrixInfo())
                systemInfo.Text = "Количество переменных и условий должно быть числом от 1 до 16.";
            else
                systemInfo.Text = "";
        }

        private bool checkMatrixInfo()
        {
            if (systemInfo == null)
                return true;
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
                return true;

        }

        private void matrixInputGrid_Initialized(object sender, EventArgs e)
        {
            List<TextBox> inputMatrixTBList = new List<TextBox>(64);
            for (int i = 0; i < 256; i++)
            {
                ;
            }
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

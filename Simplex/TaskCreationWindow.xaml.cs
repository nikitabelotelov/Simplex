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
    public partial class TaskCreationWindow : Window
    {
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
            int varNum, condNum;
            if (!int.TryParse(varNumTB.Text, out varNum))
                return false;
            else if (!int.TryParse(condNumTB.Text, out condNum))
                return false;
            else if (varNum < 1 || varNum > 16 || condNum < 1 || condNum > 16)
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
    }
}

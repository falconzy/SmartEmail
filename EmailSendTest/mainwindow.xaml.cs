using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmailSendTest
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        SendEmail SendMail = new SendEmail();
        public Start()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
          
            //SendMail.EmailSending("tjFW8NzS", 2);
			SendMail.EmailSending("hwlM+eGk3i4xH4XlsDmvzw==");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            SendMail.receivedEmail();
        }

		private void btnMethodTest_Click(object sender, RoutedEventArgs e)
		{
			//For test indivdual mehtod;
		}
    }
}

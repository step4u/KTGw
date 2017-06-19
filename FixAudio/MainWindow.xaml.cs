using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using NAudio.Wave;

namespace FixAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private WaveFormat pcmFormat8 = new WaveFormat(8000, 8, 1);

        private void selFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtBox.Text = openFileDialog.FileName;
            }

            FileStream fs = File.OpenRead(openFileDialog.FileName);
            String[] fn = openFileDialog.FileName.Split('\\');

            if (fn.Length < 1)
            {
                txtBox.Text = "File path length error : " + fn.Length.ToString();
                return;
            }

            String onlyfilename = fn[fn.Length-1];
            String[] fn1 = onlyfilename.Split('.');

            if (fn1.Length < 2)
            {
                txtBox.Text = "File name length error : " + fn1.Length.ToString();
                return;
            }

            String filename = fn1[0];
            String ext = fn1[1];
            String newfilename = String.Format("{0}_fixed.{1}", filename, ext);

            WaveFileWriter writer = new WaveFileWriter(newfilename, pcmFormat8);
            
        }
    }
}

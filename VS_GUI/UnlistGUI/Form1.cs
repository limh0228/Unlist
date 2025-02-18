using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
/**
 * Unlist GUI - An application to unsubscribe from those pesky companies who won't leave you alone!
 *
 */
namespace UnlistGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Unsubscribe.Visible = false;
            Pass.UseSystemPasswordChar = true;
        }

        /**
         * Submit credentials and write to file for Python script to use.  Reads results of Python script back into checked list box.
         */
        private void Submit_Click(object sender, EventArgs e)
        {

            File.WriteAllText("src/credentials.txt",User.Text + ":" + Pass.Text);
            FileInfo emails = new FileInfo("src/emails.txt");
            if (IsFileLocked(emails)) {
            }
            int count = 0;
            parseStatus.Step = 1;
            run_cmd("Unsubscribe.py");
            parseStatus.Maximum = File.ReadLines("src/emails.txt").Count();
            using (StreamReader readtext = new StreamReader("src/emails.txt"))
            {
                while (!readtext.EndOfStream)
                {
                    string readMeText = readtext.ReadLine();
                    UnsubList.Items.Insert(count, readMeText);
                    count++;
                    parseStatus.PerformStep();
                }
            }

            Unsubscribe.Visible = true;
        }

        /**
         * Prevents file from being opened simultaneously by Python script and this GUI.
         * 
         */
        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }
        private void UnsubList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /* 
         * Handles writing currently checked items to file for Python script to unsubscribe from.
         */
        private void Unsubscribe_Click(object sender, EventArgs e)
        {
            string checkedItems = string.Empty;
            foreach (object Item in UnsubList.CheckedItems)
            {
                checkedItems += (Item.ToString() + "\n");
            }
            System.IO.File.WriteAllText("src/selected_emails.txt",checkedItems);
        }
        private void run_cmd(string cmd)
        {
            Process p = new Process(); // create process to run the python program
            p.StartInfo.FileName = @"\usr\bin\python3.5"; //Python.exe location
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // ensures you can read stdout
            p.StartInfo.Arguments = (@"D:\cs\Unlist\VS_GUI\UnlistGUI\bin\Debug\src\"+cmd);
            p.Start(); // start the process (the python program)
            StreamReader s = p.StandardOutput;
            String output = s.ReadToEnd();
            Console.WriteLine(output);
            p.WaitForExit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

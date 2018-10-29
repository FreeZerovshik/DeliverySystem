using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace DeliverySystem
{
    public partial class FMain : Form
    {
        public FMain()
        {
            InitializeComponent();
        }

        private void bOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileD = new OpenFileDialog();
            openFileD.Filter = "Mdb Files|*.mdb";
            openFileD.Title = "Select a Mdb File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  
                tFilePath.Text = openFileD.FileName;
            }
        }
    }
}

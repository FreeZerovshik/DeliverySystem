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
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DeliverySystem
{
    public partial class FMain : Form
    {
        static string mdb_file_path;
        static string NL = System.Environment.NewLine;
        dynamic config;

      //  DataTable tData;
        Dictionary<int, string> _myAttachments = new Dictionary<int, string>();

        public FMain()
        {
            InitializeComponent();
        }

        private void bOpenFile_Click(object sender, EventArgs e)
        {
            
            Grid.DataSource = null;
            Grid.Columns.Clear();

            OpenFileDialog openFileD = new OpenFileDialog();
            openFileD.Filter = "Mdb Files|*.mdb";
            openFileD.Title = "Select a Mdb File";

            // Show the Dialog.  
            if (openFileD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mdb_file_path = openFileD.FileName;
                tFilePath.Text = mdb_file_path;
            }

            tLog.AppendText("Open file:" + mdb_file_path + NL);

            Grid.Columns.Add("CLASS", "тип");
            Grid.Columns.Add("SNAME", "кор. имя");
            Grid.Columns.Add("NAME", "наименование");
            Grid.Columns.Add("SECTION", "секциия кода");
            

            DataGridViewImageColumn img = new DataGridViewImageColumn();

            Image image = Image.FromFile("sql_file.png");
            img.Image = image;
                      
            Grid.Columns.Add(img);
            img.HeaderText = "PL+";
            img.Name = "SOURCE";
            img.ImageLayout = DataGridViewImageCellLayout.Zoom;

            Grid.Columns.Add("STATE", "Статус");


            Grid.Columns["CLASS"].Width = 50;
            Grid.Columns["SNAME"].Width = 70;
            Grid.Columns["NAME"].Width = 250;
            Grid.Columns["SECTION"].Width = 50;
            Grid.Columns["SOURCE"].Width = 50;
            Grid.Columns["STATE"].Width = 30;



            // Connection string and SQL query  
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+ mdb_file_path;
            string strSQL = "SELECT m.CLASS_ID, m.SHORT_NAME, m.NAME, s.TYPE, s.TEXT FROM METHODS m, SOURCES_LONG s where m.id = s.method_id order by s.method_id, s.type";
           
            // Create a connection  
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Create a command and set its connection  
                OleDbCommand command = new OleDbCommand(strSQL, connection);
                // Open the connection and execute the select command.  
                try
                {
                    // Open connecton  
                    connection.Open();
                    // Execute command  
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {

                        tLog.AppendText("------------Original data----------------" + NL);
                        while (reader.Read())
                        {
                            tLog.AppendText("Class:"+ reader["CLASS_ID"].ToString() + " SName: " + reader["SHORT_NAME"].ToString() +" | " + reader["NAME"].ToString() + NL);
                            Grid.Rows.Add( reader["CLASS_ID"].ToString(),
                                           reader["SHORT_NAME"].ToString(), 
                                           reader["NAME"].ToString(),
                                           reader["TYPE"].ToString(),
                                           image
                                           );

                            if (_myAttachments.ContainsKey(Grid.Rows.Count - 1))
                                _myAttachments[Grid.Rows.Count - 1] = reader["TEXT"].ToString();
                            else
                                _myAttachments.Add(Grid.Rows.Count - 1, reader["TEXT"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    tLog.AppendText(ex.Message);
                }


                // The connection is automatically closed becasuse of using block.  
                        }
            
        }

        private void DownloadAttachment(DataGridViewCell dgvCell, string p_method_name)
        {
            //string p_method_name = "temp";

            string strData = null;
            string tmp_file = p_method_name + ".sql";


            if (File.Exists(tmp_file))  File.Delete(tmp_file);

            strData = _myAttachments[dgvCell.RowIndex];
            File.WriteAllText(tmp_file, strData.Replace("\n", "\r\n"));

            if (File.Exists(tmp_file))
            {
                try
                {
                    /* http://docs.notepad-plus-plus.org/index.php/Command_Line_Switches
                     * Examples
                        Note for Windows users: Be sure to NOT put quotes around the entire command line when creating a shortcut.
                        This will not work: "C:\Program Files (x86)\Notepad++\notepad++.exe -openSession C:\<my path>\session.xml"
                        but this will: "C:\Program Files (x86)\Notepad++\notepad++.exe" -openSession C:\<my path>\session.xml

                        notepad++ -lxml d   \myproj\proj.vcproj
                        In the above example, the file proj.vcproj will be opened as an xml file, even though its extension .vcproj is not recognized as xml file extension.
                        
                        notepad++ -n150 E \notepad++\PowerEditor\src\Notepad_plus.cpp
                        The above will open the file Notepad_plus.cpp then scroll the view and place the cursor to the line 150.
                     * 
                     */
                    Process.Start("notepad++.exe", tmp_file);
                } catch (Exception ex) {
                    tLog.AppendText("Ошибка: " + ex.Message + NL);
                    tLog.AppendText("!Откроем код с помощью блокнота");
                    Process.Start("notepad.exe", tmp_file);
                }
                
            }


        }


        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("Selected = " + Grid.SelectedRows[1]);

            if (Grid.CurrentCell.OwningColumn.Name  == "SOURCE") {

               DownloadAttachment(Grid.CurrentCell , Grid.CurrentRow.Cells["SNAME"].Value.ToString());
            }
          
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tLog.AppendText(NL);
            tLog.AppendText(Grid.CurrentRow.Cells["SNAME"].Value.ToString());
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            LoadJson("config.json");
        }



        public void LoadJson(string p_file)
        {
            tLog.AppendText("------------ Load Config ------------------");
            using (StreamReader r = new StreamReader(p_file))
            {
                string json = r.ReadToEnd();

              //  tLog.AppendText(json + NL);
                ///List<Config> config = JsonConvert.DeserializeObject<List<Config>>(json);

                config = JsonConvert.DeserializeObject(json);


                tLog.AppendText("------------ parse json------------------" + NL);
                tLog.AppendText("ftp_url: "+ config.ftp.url + NL);
                tLog.AppendText("pattern: " + config.pattern + NL);

            }
        }

    }
} 

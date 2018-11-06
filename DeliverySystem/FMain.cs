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
using System.Text.RegularExpressions;

namespace DeliverySystem
{
    public partial class FMain : Form
    {
        static string mdb_file_path;
        static string NL = System.Environment.NewLine;
        dynamic config;

      //  DataTable tData;
        Dictionary<int, string> _myAttachments = new Dictionary<int, string>();

        SubProgramm _procTree;




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
                          //  tLog.AppendText("Class:"+ reader["CLASS_ID"].ToString() + " SName: " + reader["SHORT_NAME"].ToString() +" | " + reader["NAME"].ToString() + NL);
                            Grid.Rows.Add( reader["CLASS_ID"].ToString(),
                                           reader["SHORT_NAME"].ToString(), 
                                           reader["NAME"].ToString(),
                                           reader["TYPE"].ToString(),
                                           image
                                           );

                            if (_myAttachments.ContainsKey(Grid.Rows.Count - 1))
                                _myAttachments[Grid.Rows.Count - 1] = reader["TEXT"].ToString().Replace("\n", "\r\n");
                            else
                                _myAttachments.Add(Grid.Rows.Count - 1, reader["TEXT"].ToString().Replace("\n", "\r\n"));
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
            //File.WriteAllText(tmp_file, strData.Replace("\n", "\r\n"));
            File.WriteAllText(tmp_file, strData);

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

                get_comment(_myAttachments[Grid.CurrentCell.RowIndex]);
                get_prog(_myAttachments[Grid.CurrentCell.RowIndex]);
            }

           
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tLog.AppendText(NL);
            tLog.AppendText(Grid.CurrentRow.Cells["SNAME"].Value.ToString()+NL);
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            LoadJson("config.json");

        }

        private void get_comment(string p_code)
        {
            Regex regex = new Regex(@LexicalAnalyzer.p_comment, RegexOptions.IgnoreCase);

            var i = 1;
            foreach (var str in p_code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                MatchCollection matches = regex.Matches(str);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        tLog.AppendText("n_str:"+ i +" n_char:" + match.Index + " str:" + match.Value.Trim() + NL);
                }
                i++;
            }
        }

        private string parse(string p_str,string p_patt)
        {
            Regex regex = new Regex(@p_patt, RegexOptions.IgnoreCase);

            string result = null;

             MatchCollection matches = regex.Matches(p_str);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        //tLog.AppendText("n_str:" + i + " n_char:" + match.Index + " str:" + match.Value.Trim() + NL);
                        result = match.Value.Trim();
                }

            return result;
        }
        

        private int cnt_char(string p_str, char p_patt)
        {
           return p_str.Split(p_patt).Length - 1;
        }

        private void get_param(ref string p_code, ref SubProgramm p_subs)
        {
            //p_code = p_code.Substring(p_code.IndexOf('(')+1, p_code.LastIndexOf(')')-1);
            string[] _pars = p_code.Split(',');

            int i = 0;
            foreach (var par in _pars )
            {
                SubProgramm._Params p = new SubProgramm._Params();
                

                string[] _p = par.Trim().Split(new string[] {" ",":=", }, StringSplitOptions.RemoveEmptyEntries);


                p.name = _p[0];

                if (_p.Length == 2) // параметра типа: имя тип
                {                 
                    p.type = _p[1];
                    //if (_p[2].Contains("ref[")) // параметр типа: имя ref тип
                    //{
                    //    p.IsRef = true;
                    //    p.type = _p[2];
                    //}
                } else if (_p.Length > 2) 
                {
                   
                    if (_p.Contains("ref")) // параметр типа: имя ref тип
                    {
                        p.IsRef = true;
                        p.type = _p[2];

                        // ищем знач. по умолчанию
                        if (_p.Contains("default")) p.def_value = _p[4];
                        else if (_p.Length > 3) p.def_value = string.Join(" ", _p.Skip(3));
                        
                    }
                    else
                    {
                        p.type = _p[1];
                        // ищем знач. по умолчанию
                        if (_p.Contains("default")) p.def_value = _p[3];
                        else if (_p.Length > 2) p.def_value = string.Join(" ",_p.Skip(2));
                    }

                }

                


                p_subs.parameters.Add(i, p);
                i++;
            }
        }


        private void get_prog(string p_code)
        {
            Regex regex = new Regex(@LexicalAnalyzer.p_prog, RegexOptions.IgnoreCase);

            string prog_param = null;

            int b_scope_cnt = 0;                                                // счетчик откр. скобок
            int e_scope_cnt = 0;                                                // счетчик закр. скобок


            bool b_next = true;
            //int prog_name_pos;

            int n_str = 1;
            foreach (var str in p_code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {

                MatchCollection matches = regex.Matches(str);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {//1 нашли строку с подпрограммой
                         _procTree= new SubProgramm();
                        _procTree.name = Regex.Replace(match.Value, @"(procedure|function)", "", RegexOptions.IgnoreCase).Trim();    // наим подпрограммы
                        _procTree.type = Regex.Match(str, @"(procedure|function)", RegexOptions.IgnoreCase).Value;
                        _procTree.IsPublic = Regex.IsMatch(str, @"(public)", RegexOptions.IgnoreCase);
                    }

                }

                // найдем параметры подпрораммы
                if (_procTree != null)
                {
                    // разберем строку посимвольно
                    int i = 1;
                    foreach (char c in str)
                    {
                        if (c == '(')   b_scope_cnt++;
                        if (c == ')')   e_scope_cnt++;

                        if (_procTree.type == "function" && str.Contains("return") && b_scope_cnt == 0)
                        {
                            if (str.IndexOf("return") < str.IndexOf('(') || str.IndexOf('(') == 0)   // функия без параметров
                            {
                                b_next = false;
                                break;
                            }
                        } else if (b_scope_cnt > 0 && b_scope_cnt != e_scope_cnt)  // все на одно строке
                        {
                            prog_param += c;
                        } else if (b_scope_cnt > 0 && b_scope_cnt == e_scope_cnt) {
                            prog_param += c;
                            prog_param = Regex.Replace(prog_param.Trim(new Char[] { '(', ')'}),@"(\t)+"," ");

                            //prog_param = Regex.Replace(prog_param.Trim(), @"(\t)+", " ");

                            get_param(ref prog_param, ref _procTree);
                            
                            b_next = false;
                            break;
                        }
                        i++;
                    }

                }

                 // закончили разбор параметров
                 if (b_next == false)
                 {         
                    

                    tLog.AppendText("-----------------------------------------" + NL);
                    tLog.AppendText(_procTree.IsPublic+ " " + _procTree.type+" " +_procTree.name + NL);
                    
                    tLog.AppendText("*****************************************" + NL);
                    foreach (var par in _procTree.parameters)
                    {
                        tLog.AppendText("name=" + par.Value.name + 
                                        " ref=" + par.Value.IsRef + 
                                        " type="+ par.Value.type + 
                                        " def=" + par.Value.def_value + NL);
                    }
                   // tLog.AppendText(prog_param.Replace('\t', ' ') + NL);
                    tLog.AppendText("*****************************************" + NL);
                    
                    //prog_name = null;
                    _procTree = null;
                    prog_param = null;
                    b_scope_cnt = 0;
                    e_scope_cnt = 0;

                    b_next = true;
                 }
                n_str++;
            }
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

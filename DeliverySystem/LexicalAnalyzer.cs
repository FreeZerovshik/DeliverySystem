using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeliverySystem
{
    class LexicalAnalyzer
    {
        // реализация лексического анализа pl+

        // paterns
        public string patt_get_obj ="(function|procedure)[[:space:]]+[[:alnum:]|_+]+";	// получение объекта до(
        public string patt_get_obj_name = "(function|procedure)[[:space:]]+";			//получение имени объекта
        public string patt_get_param_str ="(([^)]*))";							// получение строки параметров


        static string[] one_delim = { ";", ",", "/", "*", "+", "-", ":", "=", ">", "<", "!" , "'", "&", "[","]", "(", ")"};
        static string[] two_delim = { "::", ",", "//", "/*", "*/" };
        static string[] var = { "integer", "pls_integer", "number", "string", "varchar2", "boolean" };
        static string[] r_word = {"procedure", "function", "var", "begin", "end", "false", "true", "null", "while", "loop", "if", "else", "when", "then", "savepoint", "exception"};


        public static string p_comment = "([-]{2}|\\/\\*).+";
        public static string p_prog = ("(procedure|function)([\\s])+([A-z]|[\\d]|[_])+");
        public static string p_public = ("public");



        List<string> data;
        private void IsDigit()
        {
            foreach (string str in data)
            {
                string indent = "     ";
                string left = String.Empty;
                Regex r = new Regex(@"\d");
                Match m = r.Match(str);
                while (m.Success)
                {
                    m = m.NextMatch();
                    left += indent;
                }
            }
        }

        private void IsAlpha()
        {
            foreach (string str in data)
            {
                string indent = "     ";
                string left = String.Empty;
                Regex r = new Regex(@"[a-z]");
                Match m = r.Match(str);
                while (m.Success)
                {
                    m = m.NextMatch();
                    left += indent;
                }
            }

        }
        static Boolean chk_char(string p_char, string p_patt)
        {
            Regex regex = new Regex(@p_patt, RegexOptions.IgnoreCase);

            
            MatchCollection matches = regex.Matches(p_char);
            if (matches.Count > 0)
            {
                ///foreach (Match match in matches)
                //  tLog.AppendText(match.Value.Trim() + NL);
                return true;
            }
            return false;

        }

        static void analize(string p_char)
        {
            if (!chk_char(p_char, "[A-z]|_")) return ;
        }

    }
}

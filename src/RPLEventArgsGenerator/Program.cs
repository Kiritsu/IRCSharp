using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IRCSharp;

namespace RPLEventArgsGenerator
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (!Directory.Exists("./EventArgs"))
            {
                Directory.CreateDirectory("./EventArgs");
            }
            
            var template = File.ReadAllText("./RplTemplate.txt");
            var rawRpl = File.ReadAllLines("./RplCustoms.txt");
            
            var regexes = typeof(RegexConsts).GetFields();
            
            for (var i = 0; i < regexes.Length; i++)
            {
                var rplName = ToConventionReadable(regexes[i].Name);
                var regex = ((Regex) regexes[i].GetValue(null));
                
                var groups = regex.GetGroupNames();
                var properties = new StringBuilder();
                foreach (var group in groups)
                {
                    if (@group == "0") // ?.?
                    {
                        continue;
                    }
                        
                    var groupPropertyName = ToConventionReadable(@group);
                    properties.Append($"        public string ").Append(groupPropertyName).AppendLine(" { get; internal set; }");
                }
                    
                File.WriteAllText($"./EventArgs/{rplName}EventArgs.cs", template.Replace("{0}", rplName).Replace("{1}", properties.ToString()));
            }

            for (var i = 0; i < rawRpl.Length; i++)
            {
                var content = rawRpl[i].Split(' ');
                var rplName = ToConventionReadable(content[0]);
                
                if (content.Length <= 1)
                {
                    continue;
                }
                
                var properties = new StringBuilder();
                foreach (var arg in content.Skip(1))
                {
                    properties.Append($"        public string ").Append(arg).AppendLine(" { get; internal set; }");
                }
                
                File.WriteAllText($"./EventArgs/{rplName}EventArgs.cs", template.Replace("{0}", rplName).Replace("{1}", properties.ToString()));
            }
        }

        private static string ToConventionReadable(string input)
        {
            input = input.ToLower();

            var builder = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                if (i == 0 || (i - 1 > 0 && input[i - 1] == '_'))
                {
                    builder.Append(input[i].ToString().ToUpper());
                }
                else if (input[i] != '_')
                {
                    builder.Append(input[i]);
                }
            }

            return builder.ToString();
        }
    }
}
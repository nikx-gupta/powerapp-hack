// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Utilities.ModelConvertor // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            //string jsonFileName = "input_model.json";

            //JObject objModel = JObject.Parse(File.ReadAllText(jsonFileName));

            //var className = "Person";
            //StringBuilder sb = new StringBuilder();

            //sb.AppendLine($"public class {className} {{");
            //foreach (var property in objModel.Properties())
            //{
            //    if (property.Value.Type != JTokenType.Array && property.Value.Type != JTokenType.Object)
            //    {
            //        NormalizeName(sb, property.Name);
            //        //sb.AppendLine($"public string {property.Name} {{get;set;}}");
            //    }
            //}

            //sb.AppendLine("}");

            //File.WriteAllText($"{className}.cs", sb.ToString());
        }

        static void NormalizeName(StringBuilder sb, string dataVal)
        {
            if (dataVal.StartsWith("@"))
                return;

            var name = dataVal.Replace("_", "");
            if (dataVal.Contains("_"))
            {
                sb.AppendLine($"[JsonProperty(\"{dataVal}\")]");
            }

            sb.AppendLine($"public string {name} {{get;set;}}");
        }
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace QLTSGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitData();
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {   
            string databaseSelected = CBDatabases.Text;
            string tableSelected = CBTables.Text;
            string tablePropertyText = "";
            var templateClass = String.Join(
                Environment.NewLine,
                "using System;",
                "namespace QLTSGenerator",
                "{",
                "   /// <summary>",
                "   /// ",
                "   /// </summary>",
                "   class #TableName",
                "   {",
                "       #Properties",
                "   }",
                "}");
            var templateProperty = String.Join(
                Environment.NewLine,
                "       /// <summary>",
                "       /// #Comment",
                "       /// </summary>",
                "       public #DataType #Name { get; set; }",
                "       ");
            using (Database database = new Database())
            {
                try
                {
                    var reader = database.ExecuteReader($"SELECT c.COLUMN_NAME, c.DATA_TYPE, c.COLUMN_COMMENT FROM information_schema.COLUMNS c WHERE c.TABLE_SCHEMA='{databaseSelected}' AND c.TABLE_NAME='{tableSelected}';");
                    DataTable table = new DataTable();
                    table.Load(reader);
                    var tableProperties = new List<PropertyInfomation>();
                    foreach (DataRow row in table.Rows)
                    {
                        string Name = (string)row["COLUMN_NAME"];
                        string Comment = (string)row["COLUMN_COMMENT"];
                        string DataType = ConvertDataType((string)row["DATA_TYPE"]);
                        if (Name.Length > 0 && DataType.Length > 0)
                        {
                            tableProperties.Add(new PropertyInfomation { Name = Name, DataType = DataType, Comment = Comment });
                        }
                    }

                    foreach (PropertyInfomation property in tableProperties)
                    {
                        string propertyText = templateProperty.Replace("#Comment", property.Comment).Replace("#DataType", property.DataType).Replace("#Name", property.Name);
                        tablePropertyText = string.Join("\r\n", tablePropertyText, propertyText);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            var tableName = tableSelected.Substring(0, 1).ToUpper() + tableSelected.Substring(1, tableSelected.Length - 2);
            TextBoxResult.Text = templateClass.Replace("#TableName", tableName).Replace("#Properties", tablePropertyText);
        }

        void InitData()
        {
            using (Database database = new Database())
            {
                try
                {
                    var reader = database.ExecuteReader("SHOW DATABASES;");
                    DataTable table = new DataTable();
                    table.Load(reader);

                    CBDatabases.ValueMember = "Database";
                    CBDatabases.DataSource = table;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void CBDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            string databaseSelected = CBDatabases.Text;
            using (Database database = new Database())
            {
                try
                {
                    var reader = database.ExecuteReader($"SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_SCHEMA='{databaseSelected}';");
                    DataTable table = new DataTable();
                    table.Load(reader);

                    CBTables.ValueMember = "TABLE_NAME";
                    CBTables.DataSource = table;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private string ConvertDataType(string dataType)
        {
            var Boolean = new List<string> { "bool", "boolean", "bit" };
            var SByte = new List<string> { "tinyint" };
            var Byte = new List<string> { "tinyint unsigned" };
            var Int16 = new List<string> { "smallint", "year" };
            var Int32 = new List<string> { "int", "integer", "smallint unsigned", "mediumint" };
            var Int64 = new List<string> { "bigint", "int unsigned", "integer unsigned", "bit" };
            var Single = new List<string> { "float" };
            var Double = new List<string> { "double", "real" };
            var Decimal = new List<string> { "decimal", "numeric", "dec", "fixed", "bigint unsigned", "float unsigned", "double unsigned", "serial" };
            var DateTime = new List<string> { "date", "timestamp", "datetime" };
            var TimeSpan = new List<string> { "time" };
            var String = new List<string> { "char", "varchar", "tinytext", "text", "mediumtext", "longtext", "set", "enum", "nchar", "national char", "nvarchar", "national varchar", "character varying" };
            var Bytes = new List<string> { "binary", "varbinary", "tinyblob", "blob", "mediumblob", "longblob", "char byte" };
            if (Boolean.Contains(dataType))
            {
                return "boolean";
            }
            if (SByte.Contains(dataType))
            {
                return "SByte";
            }
            if (Byte.Contains(dataType))
            {
                return "Byte";
            }
            if (Int16.Contains(dataType))
            {
                return "Int16";
            }
            if (Int32.Contains(dataType))
            {
                return "int";
            }
            if (Int64.Contains(dataType))
            {
                return "Int64";
            }
            if (Single.Contains(dataType))
            {
                return "float";
            }
            if (Double.Contains(dataType))
            {
                return "double";
            }
            if (Decimal.Contains(dataType))
            {
                return "decimal";
            }
            if (DateTime.Contains(dataType))
            {
                return "DateTime";
            }
            if (TimeSpan.Contains(dataType))
            {
                return "TimeSpan";
            }
            if (String.Contains(dataType))
            {
                return "string";
            }
            if (Bytes.Contains(dataType))
            {
                return "Byte[]";
            }
            return "";
        }
    }
}

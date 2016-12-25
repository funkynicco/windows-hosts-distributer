using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HostsMerger
{
    class Program
    {
        static int Merge(string connectionString, string filename)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[AddHost_MergeTool]";

                        var doc = new XmlDocument();
                        doc.Load(filename);

                        foreach (XmlNode node in doc.SelectNodes("Database/Hosts/Host"))
                        {
                            var name = node.Attributes["Name"].Value;
                            var ip = node.Attributes["IP"].Value;
                            var description = node.Attributes["Description"]?.Value ?? null;
                            var hidden = (node.Attributes["Hidden"]?.Value ?? "false").ToLower() == "true";

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("domain", SqlDbType.VarChar).Value = name;
                            cmd.Parameters.Add("store", SqlDbType.Int).Value = 1;
                            cmd.Parameters.Add("ip", SqlDbType.VarChar).Value = ip;
                            cmd.Parameters.Add("description", SqlDbType.Text).Value = (description != null && description.Length > 0) ? (object)description : DBNull.Value;
                            cmd.Parameters.Add("hidden", SqlDbType.Bit).Value = hidden;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(ex.StackTrace);
                return 1;
            }

            return 0;
        }

        static int AppMain(string[] args)
        {
            string server = null;
            string databaseName = null;
            string filename = null;

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "--server")
                {
                    if (i + 1 >= args.Length)
                        throw new MessageException("--server argument not provided");

                    server = args[++i];
                }
                else if (args[i] == "--database-name")
                {
                    if (i + 1 >= args.Length)
                        throw new MessageException("--database-name argument not provided");

                    databaseName = args[++i];
                }
                else
                    filename = args[i];
            }

            if (string.IsNullOrWhiteSpace(server))
                throw new MessageException("--server argument not provided");
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new MessageException("--database-name argument not provided");
            if (string.IsNullOrWhiteSpace(filename))
                throw new MessageException("Filename not provided.");
            if (!File.Exists(filename))
                throw new MessageException("File was not found: " + filename);

            return Merge($"SERVER={server};Database={databaseName};Trusted_Connection=True", filename);
        }

        static int Main(string[] args)
        {
            try
            {
                return AppMain(args);
            }
            catch (MessageException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }

    class MessageException : Exception
    {
        public MessageException(string message) :
            base(message)
        {
        }
    }
}

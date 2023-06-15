﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSF2SQL
{
    internal class PostgresGenerator : ISqlGenerator
    {
        public string CreateDatabase(string databaseName)
        {
            return "DROP DATABASE IF EXISTS " + databaseName + ";\n" +
                "\n" +
                "CREATE DATABASE " + databaseName + " WITH ENCODING = 'UTF8'  LC_COLLATE = 'en_US.UTF-8'  LC_CTYPE = 'en_US.UTF-8'" + ";\n" +
                "USE " + databaseName + ";\n" +
                //"SET search_path TO " + databaseName +",public;" + ";\n" +
                "\n";
        }

        public string SetVariables()
        {
            //return "/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;\n" +
            //    "/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;\n" +
            //    "/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;\n" +
            //    "/*!40101 SET NAMES utf8 */;\n" +
            //    "/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;\n" +
            //    "/*!40103 SET TIME_ZONE='+00:00' */;\n" +
            //    "/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;\n" +
            //    "/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;\n" +
            //    "/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;\n" +
            //    "/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;\n";
            return "/n";
        }

        public string CreateTable(Table table)
        {
            //Console.WriteLine("CreateTable : " + table.Name);
            try
            {
                switch (table.Name)
                {
                    case "Group":
                        table.Name = "groups";
                        break;
                    case "User":
                        table.Name = "users";
                        break;
                    case "From":
                        table.Name = "froms";
                        break;
                    case "Report":
                        table.Name = "reports";
                        break;
                }

                string query =
                "CREATE TABLE " + table.Name.Replace(" ","") + " ( \n" +
                "id INT NOT NULL,\n";
                foreach (Column column in table.Columns.Values)
                {
                    if (column.Name == "Group")
                    {
                        column.Name = "groups";
                    }
                    else if (column.Name == "User")
                    {
                        column.Name = "users";
                    } 
                    else if (column.Name == "From")
                    {
                        column.Name = "froms";
                    }
                    else if (column.Name == "Report")
                    {
                        column.Name = "reports";
                    }
                    else if (column.Name == "References")
                    {
                        column.Name = "References_s";
                    }
                    
                    query += " " + column.Name.Replace("-","").Replace(" ","") + " " + column.Type + " NULL,\n";
                }
                query += "PRIMARY KEY (id)";
                //if (table.LinkedTable != null)
                //{
                //    query +=
                //        ",\n" +
                //        "KEY " + table.Name + "_idx (" + table.LinkedTable + "id),\n" +
                //        "CONSTRAINT " + table.Name + " FOREIGN KEY (" + table.LinkedTable + "id) REFERENCES " + table.LinkedTable + " (id) ON DELETE CASCADE ON UPDATE CASCADE";
                //}
                query += ");\n";
                return query.Replace("$", "").Replace("datetime", "timestamp") ;
            }
            catch (Exception e)
            {
                //Console.WriteLine("CREATE TABLE");
                //Console.WriteLine(e.ToString());
                //Console.WriteLine(table.ToString());
                return "";
            }

        }

        public string BeginInsertTable(Table table)
        {
            //Console.WriteLine("Begin Insert Table : " + table.Name);
            try
            {
                switch (table.Name)
                {
                    case "Group":
                        table.Name = "groups";
                        break;
                    case "User":
                        table.Name = "users";
                        break;
                    case "From":
                        table.Name = "froms";
                        break;
                    case "Report":
                        table.Name = "reports";
                        break;
                }
                // Convert Column
                
                foreach (Column column in table.Columns.Values)
                {
                    if (column.Name == "Group")
                    {
                        column.Name = "groups";
                    }
                    else if (column.Name == "User")
                    {
                        column.Name = "users";
                    }
                    else if (column.Name == "From")
                    {
                        column.Name = "froms";
                    }
                    else if (column.Name == "Report")
                    {
                        column.Name = "reports";
                    }
                }

                IEnumerable<String> listColumn = table.Columns.Values.Select(x => x.Name.Replace("-","").Replace(" ",""));

                //IEnumerable<String> listColumn = table.Columns.Keys.Select(x => x);
                string query = " INSERT INTO " + table.Name.Replace(" ","") + " (id," + String.Join(",", listColumn) + ") VALUES";
                
                return query.Replace("$", "");
            }
            catch (Exception e)
            {
                //Console.WriteLine("ERROR BEGIN INSERT TABLE");
                //Console.WriteLine(e.ToString());
                return "";
            }

        }

        public List<string> InsertTableRowsToList(Table table)
        {
            //Console.WriteLine("START INSERT TABLE ROW: " + table.Name);
            List<string> rows = new List<string>(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
            {
                try
                {
                    List<string> columnValues = new List<string>(table.Columns.Count);
                    foreach (Column column in table.Columns.Values)
                    {
                        if (column.Values.ContainsKey(i + 1))
                        {
                            if (column.Values[i + 1] != null)
                            {
                                string value = column.Values[i + 1].ToString();
                                switch (column.Type)
                                {
                                    case "decimal(20,10)":
                                        if (value == "")
                                        {
                                            value = "NULL";
                                        }
                                        else if (value == "Infinity")
                                        {
                                            value = "9999999999.9999999999";
                                        }
                                        else
                                        {
                                            double temp;
                                            if (!double.TryParse(value, out temp))
                                            {
                                                value = "NULL";
                                            }
                                        }
                                        break;
                                    case "timestamp":
                                        if (value == "")
                                        {
                                            value = "NULL";
                                        }
                                        else
                                        {
                                            DateTime temp;
                                            if (DateTime.TryParse(value, out temp))
                                            {
                                                if (column.Values[i + 1] is DateTime)
                                                {
                                                    value = ((DateTime)column.Values[i + 1]).ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                else
                                                {
                                                    value = temp.ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                value = "'" + value + "'";
                                            }
                                            else
                                            {
                                                value = "NULL";
                                            }
                                        }
                                        break;
                                    case "datetime":
                                        if (value == "")
                                        {
                                            value = "NULL";
                                        }
                                        else
                                        {
                                            DateTime temp;
                                            if (DateTime.TryParse(value, out temp))
                                            {
                                                if (column.Values[i + 1] is DateTime)
                                                {
                                                    value = ((DateTime)column.Values[i + 1]).ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                else
                                                {
                                                    value = temp.ToString("yyyy-MM-dd HH:mm:ss");
                                                }
                                                value = "'" + value + "'";
                                            }
                                            else
                                            {
                                                value = "NULL";
                                            }
                                        }
                                        break;
                                    default:
                                        value = "'" + value.ToString().Replace("'", "''") + "'";
                                        break;
                                }
                                columnValues.Add(value);
                            }
                            else
                            {
                                columnValues.Add("NULL");
                            }
                        }
                        else
                        {
                            columnValues.Add("NULL");
                        }
                    }
                    rows.Add("(" + (i + 1) + "," + String.Join(",", columnValues) + ")");
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("ERROR CREATE TABLE ROW");
                    Trace.WriteLine(ex.ToString());
                    continue;
                }

            }
            return rows;
        }

        public string InsertTableRowsToString(Table table)
        {
            return string.Join(",", InsertTableRowsToList(table)) + ";";
        }

        public string EndInsertTable(Table table)
        {
            string query = ""; 
                //"/*!40000 ALTER TABLE " + table.Name + " ENABLE KEYS */;\n" +
                //"UNLOCK TABLES;\n";
            return query;
        }

        public string RestoreVariables()
        {
            string query = "\n";
                //"/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;\n" +
                //"/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;\n" +
                //"/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;\n" +
                //"/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;\n" +
                //"/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;\n" +
                //"/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;\n" +
                //"/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;\n" +
                //"/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;\n";
            return query;
        }
    }
}

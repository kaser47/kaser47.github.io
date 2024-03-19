using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace RecentlyAddedShows.Service.Classes
{
    internal static class SqlGenerator
    {
        public static string GenerateSql()
        {
            string connectionString = Consts.Connection; // Replace this with your actual connection string
            string query = "SELECT * FROM [dbo].[ShowTypes]"; // Replace this with your view name
            var types = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

              

                        while (reader.Read())
                        {
                            types.Add(reader["Type"].ToString());
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            var storedProcs = StoredProcGenerator.GetStoredProcedures();

            return string.Format(baseSql(), string.Join(Environment.NewLine, storedProcs), string.Join(Environment.NewLine, types)) ;
        }

        public static string ListStoredProcedures()
        {
            string databaseName = Consts.DatabaseName; // Replace this with your database name

            string listProceduresQuery = $@"
            USE {databaseName};

            SELECT 
                p.name AS 'Stored Procedure Name',
                prm.name AS 'Parameter Name',
                TYPE_NAME(prm.system_type_id) AS 'Parameter Type',
                COUNT(prm.name) AS 'Number of Parameters'
            FROM 
                sys.procedures p
            JOIN 
                sys.parameters prm ON p.object_id = prm.object_id
            WHERE 
                p.type_desc = 'SQL_STORED_PROCEDURE'
            GROUP BY 
                p.name, prm.name, TYPE_NAME(prm.system_type_id)
            ORDER BY 
                p.name;
        ";

            List<string> executeStrings = new List<string>();

            using (SqlConnection connection = new SqlConnection(Consts.Connection))
            {
                using (SqlCommand command = new SqlCommand(listProceduresQuery, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        string currentProcedureName = null;
                        List<string> parameters = new List<string>();

                        while (reader.Read())
                        {
                            string procedureName = reader["Stored Procedure Name"].ToString();
                            string parameterName = reader["Parameter Name"].ToString();
                            string parameterType = reader["Parameter Type"].ToString();
                            int numberOfParameters = Convert.ToInt32(reader["Number of Parameters"]);

                            if (currentProcedureName == null)
                            {
                                currentProcedureName = procedureName;
                            }

                            if (procedureName != currentProcedureName)
                            {
                                // Construct execute string
                                string executeString = $"EXEC {currentProcedureName} ";
                                executeString += string.Join(", ", parameters);
                                executeStrings.Add(executeString);

                                // Reset for the next procedure
                                currentProcedureName = procedureName;
                                parameters.Clear();
                            }

                            parameters.Add($"@{parameterName} = <{parameterType}>"); // Replace <{parameterType}> with actual parameter value

                            if (numberOfParameters == parameters.Count)
                            {
                                // Construct execute string
                                string executeString = $"EXEC {currentProcedureName} ";
                                executeString += string.Join(", ", parameters);
                                executeStrings.Add(executeString);

                                // Reset for the next procedure
                                currentProcedureName = null;
                                parameters.Clear();
                            }
                        }

                        // Add the last procedure if there is one
                        if (parameters.Count > 0)
                        {
                            // Construct execute string
                            string executeString = $"EXEC {currentProcedureName} ";
                            executeString += string.Join(", ", parameters);
                            executeStrings.Add(executeString);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }

                // Print execute strings
                foreach (string executeString in executeStrings)
                {
                    Console.WriteLine(executeString);
                }
                return string.Empty;
            }
        }
        static SqlDbType GetSqlDbType(string typeName)
        {
            // Implement this method to convert type name to SqlDbType
            // For simplicity, assuming all parameters are strings
            return SqlDbType.NVarChar;
        }

        public static string baseSql()
        {
            var baseSql = @"
--SQL TEMPLATE ---

--EXEC dbo.ClearAllLogs;

--EXEC dbo.GetTotalItems;

{0}

-----

--SHOW IN HTML DATE IS NOT NULL
/*
SELECT TOP (100000) [Id]
        ,[Name]
        ,[Type]
        ,[Url]
        ,[Image]
        ,[Created]
        ,[NumberViewing]
        ,[IsUpdated]
        ,[ReleaseDate]
        ,[DeletedDate]
        ,[IsChecked]
        ,[ShowInHtml]
        ,[ShowInHtmlDate]
        ,[SubType]
    FROM [recentlyaddedshows_ras].[dbo].[ShowInHtmlDateISNOTNULL]
    ORDER BY [ShowInHtmlDate] DESC
*/

-----

/*
SHOW TYPES

{1}

*/

";

            return baseSql;
        } 
    }
}

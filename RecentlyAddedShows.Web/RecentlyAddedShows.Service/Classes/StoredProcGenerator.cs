using Microsoft.Data.SqlClient;
using RecentlyAddedShows.Service.Classes;
using System;
using System.Collections.Generic;


public static class StoredProcGenerator
{
    public static List<string> GetStoredProcedures()
    {
        var statements = new List<string>();
        string databaseName = Consts.DatabaseName; // Replace this with your database name

        string query = $@"
            USE {databaseName};

            -- Get stored procedures along with their parameters
SELECT 
    p.name AS 'ProcedureName',
    prm.name AS 'ParameterName',
    TYPE_NAME(prm.system_type_id) AS 'ParameterType'
FROM 
    sys.procedures p
LEFT JOIN 
    sys.parameters prm ON p.object_id = prm.object_id
WHERE 
    p.type_desc = 'SQL_STORED_PROCEDURE' OR prm.object_id IS NULL
ORDER BY 
    p.name, prm.parameter_id;
        ";

        Dictionary<string, List<string>> procedureParameters = new Dictionary<string, List<string>>();

        using (SqlConnection connection = new SqlConnection(Consts.Connection))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string procedureName = reader["ProcedureName"].ToString();
                        string parameterName = reader["ParameterName"].ToString();
                        string parameterType = reader["ParameterType"].ToString();

                        if (!procedureParameters.ContainsKey(procedureName))
                        {
                            procedureParameters[procedureName] = new List<string>();
                        }

                        // Add parameter to the dictionary
                        procedureParameters[procedureName].Add(parameterName);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        // Construct EXEC statements for each stored procedure with parameters
        List<string> executeStatements = new List<string>();
        foreach (var kvp in procedureParameters)
        {
            string procedureName = kvp.Key;
            List<string> parameters = kvp.Value;

            string executeStatement = $"--EXEC {procedureName} ";

            List<string> parameterValues = new List<string>();
            foreach (string parameter in parameters)
            {
                if (parameter != "")
                {
                    parameterValues.Add($"{parameter} = <{parameter}>"); // Replace <{parameter}> with actual parameter value
                }
                }

            if (parameterValues.Count > 0)
            {
                executeStatement += string.Join(", ", parameterValues);
            }
            else
            {
                executeStatement = executeStatement.Substring(0, executeStatement.Length - 1);
            }

                executeStatements.Add(executeStatement + ";" + Environment.NewLine);
        }

        // Print the constructed EXEC statements
        foreach (string statement in executeStatements)
        {
            statements.Add(statement);
        }

        return statements;
    }
}
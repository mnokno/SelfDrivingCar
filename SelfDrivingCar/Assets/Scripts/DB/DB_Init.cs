using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Data;
using Mono.Data.Sqlite;
using System;

public class DB_Init
{
    public static string conn = "URI=file:" + Application.persistentDataPath + "/DB/DeepNet.db";  //Path to database

    public static void CheckTables()
    {
        // Check if expected tables exist and creates missing tables
        CheckNetwork();
        CheckHiddenBiases();
        CheckHiddenWeights();
    }

    public static void CheckNetwork()
    {
        // Open connection to the database
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open();

        // Crates command
        IDbCommand dbcmd = dbconn.CreateCommand();

        // Sets command
        dbcmd.CommandText =
            "CREATE TABLE IF NOT EXISTS \"Network\" (" +
            "\"Network_ID\"	INTEGER NOT NULL UNIQUE," +
            "\"Name\"	TEXT NOT NULL UNIQUE," +
            "\"NetworkTopology\"	TEXT NOT NULL," +
            "\"Generation\"	INTEGER NOT NULL," +
            "\"PopulationSize\"	INTEGER NOT NULL," +
            "\"ParentPopulationSize\"	INTEGER NOT NULL," +
            "\"MutationChance\"	REAL NOT NULL," +
            "\"MutationStrength\"	REAL NOT NULL," +
            "\"ReshuffleMutationChance\"	REAL NOT NULL," +
            "\"ReshuffleMutationStrength\"	REAL NOT NULL," +
            "\"InputToHiddenWeights\"	TEXT NOT NULL," +
            "\"HiddenToOutputWeights\"	TEXT NOT NULL," +
            "\"OutputBiases\"	TEXT NOT NULL," +
            "PRIMARY KEY(\"Network_ID\" AUTOINCREMENT)" +
            ");";

        // Executes the command
        dbcmd.ExecuteScalar();

        // Closes connection to the database
        dbconn.Close();
    }

    public static void CheckHiddenBiases()
    {
        // Open connection to the database
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open();

        // Crates command
        IDbCommand dbcmd = dbconn.CreateCommand();

        // Sets command
        dbcmd.CommandText =
            "CREATE TABLE IF NOT EXISTS \"HiddenBiases\" (" +
            "\"HiddenBiases_ID\"	INTEGER NOT NULL UNIQUE," +
            "\"Network_ID\"	INTEGER NOT NULL," +
            "\"HiddenLayerNumber\"	INTEGER NOT NULL," +
            "\"HiddenBiasesLayer\"	TEXT NOT NULL," +
            "FOREIGN KEY(\"Network_ID\") REFERENCES \"Network\"(\"Network_ID\")," +
            "PRIMARY KEY(\"HiddenBiases_ID\" AUTOINCREMENT)" +
            ");";

        // Executes the command
        dbcmd.ExecuteScalar();

        // Closes connection to the database
        dbconn.Close();
    }

    public static void CheckHiddenWeights()
    {
        // Open connection to the database
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open();

        // Crates command
        IDbCommand dbcmd = dbconn.CreateCommand();

        // Sets command
        dbcmd.CommandText =
            "CREATE TABLE IF NOT EXISTS \"HiddenWeights\" (" +
            "\"HiddenWeights_ID\"	INTEGER NOT NULL UNIQUE," +
            "\"Network_ID\"	INTEGER NOT NULL," +
            "\"HiddenLayerNumber\"	INTEGER NOT NULL," +
            "\"HiddenWeightsLayer\"	TEXT NOT NULL," +
            "FOREIGN KEY(\"Network_ID\") REFERENCES \"Network\"(\"Network_ID\")," +
            "PRIMARY KEY(\"HiddenWeights_ID\" AUTOINCREMENT)" +
            ");";

        // Executes the command
        dbcmd.ExecuteScalar();

        // Closes connection to the database
        dbconn.Close();
    }

    public static bool CheckIfTableExists(string tableName)
    {
        // Open connection to the database
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open();

        // Crates command
        IDbCommand dbcmd = dbconn.CreateCommand();

        // Sets command
        dbcmd.CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";
        object result = dbcmd.ExecuteScalar();
        int resultCount = Convert.ToInt32(result);

        // Executes the command
        dbcmd.ExecuteScalar();

        // Closes connection to the database
        dbconn.Close();

        if (resultCount > 0)
            return true;
        else
            return false;
    }
}

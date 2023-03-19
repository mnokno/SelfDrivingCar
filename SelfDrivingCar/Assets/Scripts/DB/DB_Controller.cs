using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Data;
using Mono.Data.Sqlite;
using System;

public class DB_Controller
{
    
    public static string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/DB/DeepNet.db"; //Path to database
    public static bool SaveDeepNet(DeepNetRecord record)
    {
        try
        {
            IDbConnection dbconn = new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            // Saves network in the Network table
            string Name = record.name;
            string NetworkTopology = String.Join(";", record.networkTopology);
            int Generation = record.generation;
            int PopulationSize = record.populationSize;
            int ParentPopulationSize = record.parentPopulationSize;
            float MutationStrength = record.mutationStrength;
            float MutationChance = record.mutationChance;
            float ReshuffleMutationStrength = record.reshuffleMutationStrength;
            float ReshuffleMutationChance = record.reshuffleMutationChance;
            string InputToHiddenWeights;
            string HiddenToOutputWeights;
            string OutputBiases = String.Join(";", record.oBiases);

            List<string> parts = new List<string>();
            foreach (double[] array in record.ihWeights)
            {
                parts.Add(String.Join(";", array));
            }
            InputToHiddenWeights = String.Join(":", parts);

            parts.Clear();
            foreach (double[] array in record.hoWeights)
            {
                parts.Add(String.Join(";", array));
            }
            HiddenToOutputWeights = String.Join(":", parts);

            dbcmd.CommandText = "INSERT INTO \"main\".\"Network\"" +
                "(\"Name\", \"NetworkTopology\", \"Generation\", \"PopulationSize\", \"ParentPopulationSize\", \"MutationStrength\", \"MutationChance\", \"ReshuffleMutationStrength\", \"ReshuffleMutationChance\", \"InputToHiddenWeights\", \"HiddenToOutputWeights\", \"OutputBiases\")" +
                $"VALUES(\"{Name}\", \"{NetworkTopology}\", \"{Generation}\", \"{PopulationSize}\", \"{ParentPopulationSize}\", {MutationStrength}, {MutationChance}, {ReshuffleMutationStrength}, {ReshuffleMutationChance}, \"{InputToHiddenWeights}\", \"{HiddenToOutputWeights}\", \"{OutputBiases}\");";

            dbcmd.ExecuteNonQuery(); // Executes command

            // Finds the PK of a newly saved network
            dbcmd.CommandText = $"SELECT Network_ID FROM Network WHERE name='{record.name}' LIMIT 1;";
            IDataReader reader = dbcmd.ExecuteReader();
            int Network_ID = 0;
            while (reader.Read())
            {
                Network_ID = reader.GetInt32(0);
            }
            reader.Close();

            // Saves biases for all hidden layers
            for (int i = 0; i < record.hBiases.Length; i++)
            {
                dbcmd.CommandText = $"INSERT INTO 'main'.'HiddenBiases'" +
                    "('Network_ID', 'HiddenLayerNumber', 'HiddenBiasesLayer')" +
                    $"VALUES({Network_ID}, {i}, '{String.Join(";", record.hBiases[i])}');";
                dbcmd.ExecuteNonQuery();
            }

            // Saves weights for all hidden layers
            for (int i = 0; i < record.hhWeights.Length; i++)
            {
                parts.Clear();
                foreach (double[] array in record.hhWeights[i])
                {
                    parts.Add(String.Join(";", array));
                }
                string HiddenWeightsLayer = String.Join(":", parts);

                dbcmd.CommandText = $"INSERT INTO 'main'.'HiddenWeights'" +
                    "('Network_ID', 'HiddenLayerNumber', 'HiddenWeightsLayer')" +
                    $"VALUES({Network_ID}, {i}, '{HiddenWeightsLayer}');";
                dbcmd.ExecuteNonQuery();
            }

            dbcmd.Dispose(); // Closes connection
            dbconn.Close(); // Disposes command

            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public static void DeletDeepNet(string deepNetName)
    {
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        // Finds the Network_ID
        dbcmd.CommandText = $"SELECT Network_ID FROM Network WHERE name='{deepNetName}' LIMIT 1;";
        IDataReader reader = dbcmd.ExecuteReader();
        int Network_ID = 0;
        while (reader.Read())
        {
            Network_ID = reader.GetInt32(0);
        }
        reader.Close();

        dbcmd.CommandText = $"DELETE FROM HiddenBiases WHERE Network_ID = {Network_ID};" +
                            $"DELETE FROM HiddenWeights WHERE Network_ID = {Network_ID};" +
                            $"DELETE FROM Network WHERE Network_ID = {Network_ID};";
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose(); // Closes connection
        dbconn.Close(); // Disposes command
    }

    public static bool DoesContainName(string name)
    {
        IDbConnection dbConnection = new SqliteConnection(conn);
        dbConnection.Open(); //Open connection to the database

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT EXISTS(SELECT 1 FROM Network WHERE name=\'{name}\');"; // Create command
    
        int count = Convert.ToInt32(dbCommand.ExecuteScalar()); // Executes command

        dbConnection.Close(); // Closes connection
        dbCommand.Dispose(); // Disposes command

        return count != 0; // Checks if a record with given name exists
    }

    public static DeepNetRecord GetDeepNet(string name)
    {
        // Open connection to the database
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open(); 
        
        // Crates command
        IDbCommand dbcmd = dbconn.CreateCommand();

        // Create DeepNetRecord
        DeepNetRecord deepNetRecord = new DeepNetRecord();

        // Finds the Network_ID
        dbcmd.CommandText = $"SELECT Network_ID FROM Network WHERE name='{name}' LIMIT 1;";
        IDataReader reader = dbcmd.ExecuteReader();
        int Network_ID = 0;
        while (reader.Read())
        {
            Network_ID = reader.GetInt32(0);
        }
        reader.Close();

        // Reads from the Network table
        dbcmd.CommandText = $"SELECT * FROM Network WHERE Network_ID='{Network_ID}';";
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            deepNetRecord.name = reader.GetString(1);
            deepNetRecord.networkTopology = Array.ConvertAll(reader.GetString(2).Split(';'), int.Parse);
            deepNetRecord.generation = reader.GetInt32(3);
            deepNetRecord.populationSize = reader.GetInt32(4);
            deepNetRecord.parentPopulationSize = reader.GetInt32(5);
            deepNetRecord.mutationChance = reader.GetFloat(6);
            deepNetRecord.mutationStrength = reader.GetFloat(7);
            deepNetRecord.reshuffleMutationChance = reader.GetFloat(8);
            deepNetRecord.reshuffleMutationStrength = reader.GetFloat(9);
            deepNetRecord.ihWeights = GetArrayFromString(reader.GetString(10));
            deepNetRecord.hoWeights = GetArrayFromString(reader.GetString(11));
            deepNetRecord.oBiases = Array.ConvertAll(reader.GetString(12).Split(';'), double.Parse);
        }
        reader.Close();

        // Reads biases of hidden layers from HiddenBiases table
        Dictionary<int, double[]> hBiasesDict = new Dictionary<int, double[]>();

        dbcmd.CommandText = $"SELECT HiddenLayerNumber, HiddenBiasesLayer FROM HiddenBiases WHERE Network_ID='{Network_ID}';";
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            hBiasesDict.Add(reader.GetInt32(0), Array.ConvertAll(reader.GetString(1).Split(';'), double.Parse));
        }
        reader.Close();

        deepNetRecord.hBiases = new double[hBiasesDict.Count][];
        for(int i = 0; i < hBiasesDict.Count; i++)
        {
             hBiasesDict.TryGetValue(i, out deepNetRecord.hBiases[i]);
        }
        

        // Reads weights of hidden layers from HiddenWeight table
        Dictionary<int, double[][]> hWeightsDict = new Dictionary<int, double[][]>();

        dbcmd.CommandText = $"SELECT HiddenLayerNumber, HiddenWeightsLayer FROM HiddenWeights WHERE Network_ID='{Network_ID}';";
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            hWeightsDict.Add(reader.GetInt32(0), GetArrayFromString(reader.GetString(1)));
        }
        reader.Close();

        deepNetRecord.hhWeights = new double[hWeightsDict.Count][][];
        for (int i = 0; i < hWeightsDict.Count; i++)
        {
            hWeightsDict.TryGetValue(i, out deepNetRecord.hhWeights[i]);
        }
        

        dbcmd.Dispose(); // Closes connection
        dbconn.Close(); // Disposes command

        return deepNetRecord;
    }

    public static string[] GetAllNetworkNames()
    {
        // Create array to store names
        List<string> names = new List<string>();

        IDbConnection dbConnection = new SqliteConnection(conn);
        dbConnection.Open(); //Open connection to the database

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"SELECT Name FROM Network"; // Create command
        IDataReader dataReader = dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            names.Add(dataReader.GetString(0));
        }

        dataReader.Close(); 
        dbCommand.Dispose(); // Closes connection
        dbConnection.Close(); // Disposes command

        // Returns list of names
        return names.ToArray();
    }

    private static double[][] GetArrayFromString(string arrayAsString)
    {
        string[] parts = arrayAsString.Split(':');
        double[][] array = new double[parts.Length][];
        for(int i = 0; i < parts.Length; i++)
        {
            array[i] = Array.ConvertAll(parts[i].Split(';'), double.Parse);
        }
        return array;
    }

    public static void DumpDeepNetRecord(DeepNetRecord record)
    {
        string dump = "";

        dump += $"Name: {record.name} \n";
        dump += $"Network topology: {String.Join("-", record.networkTopology)} \n";
        dump += $"Generation: {record.generation} \n";
        dump += $"Population Size: {record.populationSize} \n";
        dump += $"Parent population size: {record.parentPopulationSize} \n";

        dump += $"Mutation chance: {record.mutationChance} \n";
        dump += $"Mutation strength: {record.mutationStrength} \n";
        dump += $"Reshuffle mutation chance: {record.reshuffleMutationChance} \n";
        dump += $"Reshuffle mutation strength: {record.reshuffleMutationStrength} \n";

        dump += "\n";
        for (int i = 0; i < record.ihWeights.Length; i++)
        {
            dump += $"Input to hidden-{i} weights: {String.Join("-", record.ihWeights[i])} \n";
        }

        for (int i = 0; i < record.hhWeights.Length; i++)
        {
            dump += "\n";
            for (int j = 0; j < record.hhWeights[i].Length; j++)
            {
                dump += $"Hidden-{i} to hidden-{j} weights: {String.Join("-", record.hhWeights[i][j])} \n";
            }
        }

        dump += "\n";
        for (int i = 0; i < record.hoWeights.Length; i++)
        {
            dump += $"Hidden to output-{i} weights: {String.Join("-", record.hoWeights[i])} \n";
        }

        dump += "\n";
        for (int i = 0; i < record.hBiases.Length; i++)
        {
            dump += $"Hidden-{i} biases: {String.Join("-", record.hBiases[i])} \n";
        }

        dump += "\n";
        dump += $"Outputs biases: {String.Join("-", record.oBiases)} \n";

        Debug.Log(dump);
    }

    public struct DeepNetRecord
    {
        public string name;
        public int[] networkTopology;
        public int generation;
        public int populationSize;
        public int parentPopulationSize; 

        public float mutationChance;
        public float mutationStrength;
        public float reshuffleMutationChance;
        public float reshuffleMutationStrength;

        public double[][] ihWeights;  // input-1st hidden
        public double[][][] hhWeights; // hidden-hidden
        public double[][] hoWeights;  // last hidden-output

        public double[][] hBiases;  // hidden node biases
        public double[] oBiases;  // output node biases
    }
}

// Service for handling database interactions \\
// For majority of the creation of this service, I used Microsoft documentation, and relevant links can be found above each section \\
// https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\

// USe model for unpackaging database data (MealPlanModel) \\
using MealMe.Models;
// The microsoft NPM package for sqlite \\
using Microsoft.Data.Sqlite;

namespace MealMe.Services;

public class DatabaseService
{
    // Creating string which will contain the database directory \\
    private readonly string connectDirectory;
    // Used for storing fetched database entries \\
    public MealPlanModel mealPlanModel;

    // Constructor \\
    public DatabaseService()
    {
        // https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?view=net-maui-9.0&tabs=windows \\
        // folderPath stores the path to the apps appdata directory for storing data which will persist on app close \\
        var folderPath = FileSystem.Current.AppDataDirectory;
        // Combines the directory to the appdata with the database name. This will also mean it creates if the database does not exist here :) \\
        connectDirectory = Path.Combine(folderPath, "database.db");

        // Calls method for initialising database in constructor \\
        InitialiseDB();
    }

    // Method which Initialised the database connection, creates the table if it doesn't exist) \\
    private void InitialiseDB()
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Connects to the sqlite database using the directory of the database \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            // Open the connection to db \\
            connection.Open();
            // Initialise the command for execution \\
            var command = connection.CreateCommand();

            // Add SQL to the command \\
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS mealplans (
                    planID INTEGER PRIMARY KEY AUTOINCREMENT,
                    date DATE NOT NULL UNIQUE,
                    Breakfast VARCHAR(64) DEFAULT 'None',
                    BreakfastID INTEGER DEFAULT 0,
                    Lunch VARCHAR(64) DEFAULT 'None',
                    LunchID INTEGER DEFAULT 0,
                    Dinner VARCHAR(64) DEFAULT 'None',
                    DinnerID INTEGER DEFAULT 0,
                    Snack VARCHAR(64) DEFAULT 'None',
                    SnackID INTEGER DEFAULT 0
                );
            ";
            // Execute the command \\
            command.ExecuteNonQuery();
        }
    }

    // Method for checking if a meal plan exists at the date, and if it does, direct to update operation, if it doesn't direct to create operation \\
    public async Task<bool> DirectItem(string type, string date, string name, int id)
    {
        // True if it exists, False if it doesn't
        bool exists = await GetDate(date);
        // Holds status of operation \\
        bool status;
        // If the date exists in db \\
        if (exists)
        {
            // Await the update of the date entry, stores true if success \\
            status = await UpdateItem(type, date, name, id);
        } else
        {
            // Await the creation of the date entry, stores true if success \\
            status = await AddItem(type, date, name, id);
        }
        // Return success status \\
        return status;
    }

    // C - rud \\
    // Method for creating a database entry for the meal plans, takes the type of meal, the date of the meal, the name of the meal and the id of the meal \\
    public async Task<bool> AddItem(string type, string date, string name, int id)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Co0nnects to the database, closes connection after completion \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            // Try catch handles database errors \\
            try
            {
                // Open the connection to db asynchronously \\
                await connection.OpenAsync();
                // Initialise the command for execution \\
                var command = connection.CreateCommand();
                // Add SQL to command \\
                string SQL = $"INSERT INTO mealplans (date, {type}, {type}ID) VALUES ($date, $name, $id);";
                command.CommandText = SQL;
                // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/parameters \\
                // Add the values to the SQL for execution (parameterised) \\
                command.Parameters.AddWithValue("$date", date);
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$id", id);
                // Execute Command, stores rows affected \\
                int affected = await command.ExecuteNonQueryAsync();
                // If rows affected is 1 return true \\
                if (affected != 0)
                {
                    return true;
                }
                // If rows affected is 0 return false \\
                else if (affected == 0)
                {
                    return false;
                }
                // Catch any other results \\
                else
                {
                    return false;
                }
            } catch
            {
                // Catch database error return false. \\
                return false;
            }
        }
    }

    // cr - U - d \\
    // Method for updating a database entry \\
    public async Task<bool> UpdateItem(string type, string date, string name, int id)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Connects to the database, closes connection after completion \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            try
            {
                // Open the connection to db \\
                await connection.OpenAsync();
                // Initialise command for execution \\
                var command = connection.CreateCommand();
                // Add SQL to command \\
                string SQL = $"UPDATE mealplans SET {type} = $name, {type}ID = $id WHERE date = $date;";
                command.CommandText = SQL;
                // Add SQL command parameters \\
                command.Parameters.AddWithValue("$date", date);
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$id", id);
                // Execute command \\
                int affected = await command.ExecuteNonQueryAsync();
                // Returns true if affected rows, false if not \\
                if (affected != 0)
                {
                    return true;
                }
                else if (affected == 0)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // Catch database error and return false \\
                return false;
            }
        }
    }

    // c - R - ud \\
    // Method for getting a date entry for a specified data. Returns true if exists \\
    public async Task<bool> GetDate(string date)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Connects to the database, closes connection after completion \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            // Open the connection to db \\
            await connection.OpenAsync();
            // Initialise command for execution \\
            var command = connection.CreateCommand();
            // Add SQL to command \\
            string SQL = $"SELECT * FROM mealplans WHERE date = $date";
            command.CommandText = SQL;
            // Add SQL command parameters \\
            command.Parameters.AddWithValue("$date", date);
            // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
            // Starts reader and closes after all reading result complete \\
            using (var reader = await command.ExecuteReaderAsync())
            {
                // IF no rows to read return false \\
                if (!reader.HasRows)
                {
                    return false;
                } 
                // If there are rows to read, meaning an entry at that date exists \\
                else
                {
                    return true;
                }
            }
            
        }
    }

    // c - R - ud \\
    // Method for getting a date entry and return it as a MEalPlanModel \\
    public async Task<MealPlanModel> GetDateReturn(string date)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Connects to the database, closes connection after completion \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            // Open the connection to db \\
            await connection.OpenAsync();
            // Initialise command for execution \\
            var command = connection.CreateCommand();
            // Add SQL to command \\
            string SQL = $"SELECT * FROM mealplans WHERE date = $date";
            command.CommandText = SQL;
            // Add SQL command parameters \\
            command.Parameters.AddWithValue("$date", date);
            // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
            // Starts reader and closes after all reading result complete \\
            using (var reader = await command.ExecuteReaderAsync())
            {
                // If there are no rows to read \\
                if (!reader.HasRows)
                {
                    // Create a Meal Plan Model with none and 0 \\
                    MealPlanModel meal = new MealPlanModel
                    {
                        Date = date,
                        Breakfast = "None",
                        BreakfastID = 0,
                        Lunch = "None",
                        LunchID = 0,
                        Dinner = "None",
                        DinnerID = 0,
                        Snack = "None",
                        SnackID = 0
                    };
                    // return the mealplan model \\
                    return meal;
                }
                else
                {
                    // Read firsyt entry (should only be one since date is unique \\
                    reader.Read();
                    // Create meal plan model with the entry values from database. Default values set as well if certain values dont have any value \\
                    MealPlanModel meal = new MealPlanModel
                    {
                        Date = reader.GetString(1),
                        Breakfast = reader.IsDBNull(2) ? "None" : reader.GetString(2),
                        BreakfastID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        Lunch = reader.IsDBNull(4) ? "None" : reader.GetString(4),
                        LunchID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        Dinner = reader.IsDBNull(6) ? "None" : reader.GetString(6),
                        DinnerID = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                        Snack = reader.IsDBNull(8) ? "None" : reader.GetString(8),
                        SnackID = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                    };
                    // Return the meal plan model \\
                    return meal;
                }
            }

        }
    }

    // cru - D \\
    // Method for removing a database entry at a specified date \\
    public async Task<bool> Remove(string date)
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=net-cli \\
        // Connects to the database, closes connection after completion \\
        using (var connection = new SqliteConnection($"Data Source={connectDirectory}"))
        {
            // Open the connection to db \\
            await connection.OpenAsync();
            // Initialise command for execution \\
            var command = connection.CreateCommand();
            // Add SQL to command \\
            string SQL = $"DELETE FROM mealplans WHERE date = $date";
            command.CommandText = SQL;
            // Add SQL command parameters \\
            command.Parameters.AddWithValue("$date", date);
            // Await the execution \\
            int affected = await command.ExecuteNonQueryAsync();
            // Returns false if no rows deleted, true if rows deleted \\
            if (affected == 0)
            {
                return false;
            }
            else if (affected == 1)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
    }
}

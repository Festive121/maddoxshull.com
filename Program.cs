using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<MySqlConnection>(_ =>
    new MySqlConnection(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", context =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
    return context.Response.SendFileAsync(filePath);
});

app.MapMethods("/login", new[] { "GET", "POST" }, LoginHandler);

app.MapMethods("/signup", new[] { "GET", "POST" }, async (context) =>
{
    if (context.Request.Method == "GET")
    {
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/signup.cshtml"));
    }
    else if (context.Request.Method == "POST")
    {
        string? username = context.Request.Form["username"];
        string? email = context.Request.Form["email"];
        string? password = context.Request.Form["password"];

        Console.WriteLine($"username: {username}");
        Console.WriteLine($"email: {email}");
        Console.WriteLine($"password: {password}");

        await using var connection = context.RequestServices.GetService<MySqlConnection>();
        await connection.OpenAsync();

        // Check if the username or email already exists
        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM `users` WHERE username = @username OR email = @email";
        checkCommand.Parameters.AddWithValue("@username", username);
        checkCommand.Parameters.AddWithValue("@email", email);
        long existingUserCount = (long)await checkCommand.ExecuteScalarAsync();

        if (existingUserCount > 0)
        {
            await context.Response.WriteAsync("Error: User already exists.");
            return;
        }

        // Insert the new user into the database
        await using var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = "INSERT INTO `users` (username, email, pass) VALUES (@username, @email, @password)";
        insertCommand.Parameters.AddWithValue("@username", username);
        insertCommand.Parameters.AddWithValue("@email", email);
        insertCommand.Parameters.AddWithValue("@password", password);
        int rowsAffected = await insertCommand.ExecuteNonQueryAsync();

        if (rowsAffected > 0)
        {
            // await context.Response.WriteAsync("User added successfully.");
            context.Response.Redirect("/login");
        }
        else
        {
            await context.Response.WriteAsync("Failed to add user.");
        }
    }
    else
    {
        context.Response.StatusCode = 405;
    }
});

async Task LoginHandler(HttpContext context)
{
    if (context.Request.Method == "GET")
    {
        // Handle GET request, show the login form
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/login.cshtml"));
    }
    else if (context.Request.Method == "POST")
    {
        // Handle POST request, validate login credentials and redirect
        string username = context.Request.Form["username"];
        string password = context.Request.Form["password"];

        Console.WriteLine($"username: {username}");
        Console.WriteLine($"password: {password}");

        bool credentialsMatch = await ValidateCredentialsAsync(context, username, password);

        if (credentialsMatch)
        {
            await context.Response.WriteAsync("good yes");
            // context.Response.Redirect("/dashboard");
        }
        else
        {
            await context.Response.WriteAsync("bad no");
            // context.Response.Redirect("/login?error=1"); 
        }
    }
    else
    {
        context.Response.StatusCode = 405; // Method Not Allowed
    }
}

async Task<bool> ValidateCredentialsAsync(HttpContext context, string username, string password)
{
    using (var connection = context.RequestServices.GetService<MySqlConnection>())
    {
        await connection.OpenAsync();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username AND pass = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            var result = await command.ExecuteScalarAsync();
            int count = Convert.ToInt32(result);

            return count > 0;
        }
    }
}



app.Run();
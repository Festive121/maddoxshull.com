using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.IO;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<MySqlConnection>(_ =>
    new MySqlConnection(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseRouting();

app.UseStaticFiles();

app.MapGet("/", context =>
{
    if (context.Request.Cookies.TryGetValue("username", out _)) {
        // If the username cookie exists, redirect the user to the /home page
        context.Response.Redirect("/home");
        return Task.CompletedTask;
    } else {
        // If the username cookie doesn't exist, return the default index.html file
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
        return context.Response.SendFileAsync(filePath);
    }
});

app.MapGet("/home", async (context) => 
{
    if (context.Request.Cookies.TryGetValue("username", out _)) {
        string? username = context.Request.Cookies["username"];

        await using var connection = context.RequestServices.GetService<MySqlConnection>();

        if (connection != null) {
            await connection.OpenAsync(); // open the connection
            await using var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            int userCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            if (userCount > 0) {
                await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/home.html"));
            } else {
                context.Response.Redirect("/login");
            }
        } else {
            Console.WriteLine($"[{DateTime.Now}] Connection from [HOME] is null");
        }
    } else {
        context.Response.Redirect("/login");
    }
});

app.MapMethods("/login", new[] { "GET", "POST" }, LoginHandler);

app.MapMethods("/signup", new[] { "GET", "POST" }, async (context) =>
{
    if (context.Request.Method == "GET") {
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/signup.html"));
    } else if (context.Request.Method == "POST") {
        string? username = context.Request.Form["username"];
        string? email = context.Request.Form["email"];
        string? password = context.Request.Form["password"];

        await using var connection = context.RequestServices.GetService<MySqlConnection>();

        if (connection != null) {
            await connection.OpenAsync();

            // Check if the username or email already exists
            await using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "SELECT COUNT(*) FROM `users` WHERE username = @username OR email = @email";
            checkCommand.Parameters.AddWithValue("@username", username);
            checkCommand.Parameters.AddWithValue("@email", email);

            object? result = await checkCommand.ExecuteScalarAsync();
            long existingUserCount = result != null ? (long)result : 0;

            if (existingUserCount > 0) {
                await context.Response.WriteAsync("Error: User or email already exists.");
                Console.WriteLine($"[{DateTime.Now}]: User or email already exists. ({username})");
                return;
            }

            Console.WriteLine($"{DateTime.Now}: SIGNUP--username: {username}");
            Console.WriteLine($"{DateTime.Now}: SIGNUP--email: {email}");
            Console.WriteLine($"{DateTime.Now}: SIGNUP--password: {password}");

            // Insert the new user into the database
            await using var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO `users` (username, email, pass) VALUES (@username, @email, @password)";
            insertCommand.Parameters.AddWithValue("@username", username);
            insertCommand.Parameters.AddWithValue("@email", email);
            insertCommand.Parameters.AddWithValue("@password", password);
            int rowsAffected = await insertCommand.ExecuteNonQueryAsync();

            // Add default cards to user's deck

            string query = @"
                INSERT INTO user_cards (user_id, card_id)
                VALUES 
                (@userId, 1), 
                (@userId, 2), 
                (@userId, 3), 
                (@userId, 4), 
                (@userId, 5), 
                (@userId, 6), 
                (@userId, 7)
            ";

            if (context.Request.Cookies["SECRET"] == "true") {
                query += ",(@userId, 8);";
            } else {
                query += ";";
            }

            long userId = insertCommand.LastInsertedId;

            await using var assignCardsCommand = connection.CreateCommand();
            assignCardsCommand.CommandText = query;
            assignCardsCommand.Parameters.AddWithValue("@userId", userId);

            await assignCardsCommand.ExecuteNonQueryAsync();

            context.Response.Cookies.Delete("SECRET");
        if (rowsAffected > 0) {
            context.Response.Redirect("/login");
        } else {
            await context.Response.WriteAsync("Failed to add user.");
        }
        } else {
            Console.WriteLine($"[{DateTime.Now}] Connection from [SIGNUP] is null");
        }
    } else {
        context.Response.StatusCode = 405;
    }
});

app.MapGet("/newsletter", async (context) =>
{
    if (context.Request.Cookies.TryGetValue("username", out _)) {
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/newsletter.html"));
    } else {
        context.Response.Redirect("/login");
    }
});

app.MapPost("/Newsletter/SubscribeToNewsletter", async (context) =>
{
    string? email = context.Request.Form["email"];
    string? username = context.Request.Cookies["username"];

    await using var connection = context.RequestServices.GetService<MySqlConnection>();

    if (connection != null)
    {
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM newsletter WHERE email = @email";
            command.Parameters.AddWithValue("@email", email);

            int existingEmailCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            if (existingEmailCount > 0)
            {
                context.Response.StatusCode = 400; // Bad Request
                context.Response.Redirect("/newsletter?error=2");
                return;
            }

            // Email does not exist, insert the new subscription
            command.CommandText = "INSERT INTO newsletter (email, username) VALUES (@email, @username)";
            command.Parameters.AddWithValue("@username", username);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                context.Response.StatusCode = 200;
                context.Response.Redirect("/home?subscribed=true");
                Console.WriteLine($"[{DateTime.Now}] {username} subscribed to the newsletter.");
            }
            else
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Subscription failed.");
            }
        }
    }
    else
    {
        Console.WriteLine($"[{DateTime.Now}] Connection from [NEWSLETTER] is null");
    }
});

app.MapMethods("/editdeck", new[] { "GET", "POST" }, async (context) =>
{
    if (context.Request.Method == "GET") 
    {
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/edit_deck.html"));
    } else if (context.Request.Method == "POST") {
        // post
    } else {
        context.Response.StatusCode = 405;
    }
});

async Task LoginHandler(HttpContext context)
{
    if (context.Request.Method == "GET") 
    {
        // Handle GET request, show the login form
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/login.html"));
    } else if (context.Request.Method == "POST") {
        // Handle POST request, validate login credentials and redirect
        string? username = context.Request.Form["username"];
        string? password = context.Request.Form["password"];

        if (username != null && password != null) {
            Console.WriteLine($"{DateTime.Now}: LOGIN--username: {username}");
            Console.WriteLine($"{DateTime.Now}: LOGIN--password: {password}");

            bool credentialsMatch = await ValidateCredentialsAsync(context, username, password);

            if (credentialsMatch) {
                // await context.Response.WriteAsync("login accepted");
                context.Response.Cookies.Append("username", username); // Store the username in a cookie
                context.Response.Redirect("/home");
            } else {
                context.Response.Redirect("/login?error=1");
            }
        } else {
            Console.WriteLine($"[{DateTime.Now}] Username or password is null");
        }
    } else {
        context.Response.StatusCode = 405; // Method Not Allowed
    }
}

app.MapGet("/reset-password", async (content) => {
    await content.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/pw_reset.html"));
});

async Task<bool> ValidateCredentialsAsync(HttpContext context, string username, string password)
{
    await using var connection = context.RequestServices.GetService<MySqlConnection>();

    if (connection != null) {
        await connection.OpenAsync();
        using (var command = connection.CreateCommand()) {
            command.CommandText = "SELECT COUNT(*) FROM users WHERE username = @username AND pass = @password";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            var result = await command.ExecuteScalarAsync();
            int count = Convert.ToInt32(result);

            return count > 0;
        }
    } else {
        Console.WriteLine($"[{DateTime.Now}] Connection from [VALIDATE] is null");
    }

    return false;
}

app.MapMethods("/secret", new[] { "GET", "POST" }, async (context) => 
{
    if (context.Request.Method == "GET") {
        await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/secret.html"));
    } else if (context.Request.Method == "POST") {
        
    }
    else
    {
        context.Response.StatusCode = 405;
    }
});

app.MapMethods("/{hash}", new[] { "GET" }, async (context) =>
{
    var hash = context.Request.RouteValues["hash"]?.ToString();
    var hashes = await File.ReadAllLinesAsync("sha-secret");
    var hashSet = new HashSet<string>(hashes);

    if (hash != null) {
        if (hashSet.Contains(hash))
        {
            context.Response.Cookies.Append("SECRET", "true");
            context.Response.Redirect("/");
        }
    } else {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Invalid hash");
    }
});

app.MapControllers();

app.Run();

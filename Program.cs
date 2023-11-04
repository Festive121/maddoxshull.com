using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ||
// ****pages
// ||

// app.Use(async (context, next) =>
// {
//     var allowedPaths = new List<string> { "/", path, "/another-allowed-path", "/yet-another-allowed-path" };
//     if (!allowedPaths.Contains(context.Request.Path.Value, StringComparer.OrdinalIgnoreCase))
//     {
//         context.Response.Redirect("/");
//     }
//     else
//     {
//         await next();
//     }
// });


app.MapGet("/", async (context) => 
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/index.html"));
});

app.MapGet("/updates", async (context) =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/updates.html"));
});

app.MapGet("/secret", async (context) => 
{
    Console.WriteLine("routing to secret page");
    await context.Response.WriteAsync(await File.ReadAllTextAsync("wwwroot/html/secret.html"));
});

//            ||
// end pages****
//            ||

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

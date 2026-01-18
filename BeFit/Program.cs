using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ensure database is created and roles are initialized
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        // Check and add missing UserId columns if needed
        var connection = context.Database.GetDbConnection();
        connection.Open();
        try
        {
            using var command = connection.CreateCommand();
            
            // Check TrainingSessions table
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='TrainingSessions'";
            var trainingSessionsExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            
            if (trainingSessionsExists)
            {
                command.CommandText = "PRAGMA table_info(TrainingSessions)";
                using var reader = command.ExecuteReader();
                var hasUserId = false;
                while (reader.Read())
                {
                    if (reader.GetString(1) == "UserId")
                    {
                        hasUserId = true;
                        break;
                    }
                }
                reader.Close();
                
                if (!hasUserId)
                {
                    command.CommandText = "ALTER TABLE TrainingSessions ADD COLUMN UserId TEXT NOT NULL DEFAULT ''";
                    command.ExecuteNonQuery();
                }
            }
            
            // Check TrainingEntries table
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='TrainingEntries'";
            var trainingEntriesExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            
            if (trainingEntriesExists)
            {
                command.CommandText = "PRAGMA table_info(TrainingEntries)";
                using var reader = command.ExecuteReader();
                var hasUserId = false;
                var hasWeightKg = false;
                var hasWeight = false;
                var hasSets = false;
                var hasRepetitions = false;
                
                while (reader.Read())
                {
                    var columnName = reader.GetString(1);
                    if (columnName == "UserId")
                    {
                        hasUserId = true;
                    }
                    else if (columnName == "WeightKg")
                    {
                        hasWeightKg = true;
                    }
                    else if (columnName == "Weight")
                    {
                        hasWeight = true;
                    }
                    else if (columnName == "Sets")
                    {
                        hasSets = true;
                    }
                    else if (columnName == "Repetitions")
                    {
                        hasRepetitions = true;
                    }
                }
                reader.Close();
                
                // Ensure WeightKg column exists (legacy column name)
                if (!hasWeightKg && !hasWeight)
                {
                    command.CommandText = "ALTER TABLE TrainingEntries ADD COLUMN WeightKg REAL NOT NULL DEFAULT 0.0";
                    command.ExecuteNonQuery();
                }
                // If Weight exists but WeightKg doesn't, we keep WeightKg as the column name via mapping
                
                if (!hasUserId)
                {
                    command.CommandText = "ALTER TABLE TrainingEntries ADD COLUMN UserId TEXT NOT NULL DEFAULT ''";
                    command.ExecuteNonQuery();
                }
                
                if (!hasSets)
                {
                    command.CommandText = "ALTER TABLE TrainingEntries ADD COLUMN Sets INTEGER NOT NULL DEFAULT 1";
                    command.ExecuteNonQuery();
                }
                
                if (!hasRepetitions)
                {
                    command.CommandText = "ALTER TABLE TrainingEntries ADD COLUMN Repetitions INTEGER NOT NULL DEFAULT 1";
                    command.ExecuteNonQuery();
                }
            }
        }
        finally
        {
            connection.Close();
        }

        // Initialize roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var adminRoleName = "Administrator";
        
        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRoleName));
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB or initializing roles.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

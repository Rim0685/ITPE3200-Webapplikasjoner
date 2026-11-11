// Program.cs configures MVC, database access and Identity authentication.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quizapp.DAL;
using Quizapp.Models;

var builder = WebApplication.CreateBuilder(args);

// Registers MVC with controllers and views.
builder.Services.AddControllersWithViews();

// Registers the database context with an SQLite connection.
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("QuizDbContext")
        ?? throw new InvalidOperationException(
            "Connection string 'QuizDbContext' was not found.")));

// Registers Identity with the application's custom user model.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Allows users to sign in without email confirmation.
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<QuizDbContext>();


// Configures routes used when authentication or access is required.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";

});

// Registers the quiz repository for dependency injection.
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Registers the study note repository for dependency injection.
builder.Services.AddScoped<IStudyNoteRepository, StudyNoteRepository>();

// Registers the flashcard repository for dependency injection.
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();

// Registers the flashcard deck repository for dependency injection.
builder.Services.AddScoped<IFlashcardDeckRepository, FlashcardDeckRepository>();
var app = builder.Build();

// Shows detailed errors during development.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Makes CSS, JavaScript and images in wwwroot available.
app.UseStaticFiles();

// Matches requests to controller routes.
app.UseRouting();

// Identifies the signed-in user from the authentication cookie.
app.UseAuthentication();

// Checks whether the user may access protected actions.
app.UseAuthorization();

// Applies existing migrations and adds example data.
DBInit.Seed(app);

// Uses the default route: Controller/Action/Id.
app.MapDefaultControllerRoute();

app.Run();
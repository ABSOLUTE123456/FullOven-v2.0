using Microsoft.EntityFrameworkCore;
using polnuyaPetch.Data;
using polnuyaPetch.Models;
using polnuyaPetch.Config;

var builder = WebApplication.CreateBuilder(args);

var configService = new ConfigService();
var appConfig = configService.LoadOrCreateDefault();
builder.Services.AddSingleton(configService);
builder.Services.AddSingleton(appConfig);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=menu.db"));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    if (!context.MenuItems.Any())
    {
        context.MenuItems.AddRange(
            new MenuItem { Name = "Борщ", Category = "Супы", Price = 500, ImagePath = "image 2.png", Description = "Традиционный свекольный суп." },
            new MenuItem { Name = "Блинчики", Category = "Выпечка", Price = 450, ImagePath = "image 3.png", Description = "Нежные блинчики с ягодами." },
            new MenuItem { Name = "Шашлык", Category = "Мясо", Price = 700, ImagePath = "image 4.png", Description = "Маринованный шашлык на гриле." },
            new MenuItem { Name = "Оливье", Category = "Салаты", Price = 400, ImagePath = "image 5.png", Description = "Классический салат с мясом." },
            new MenuItem { Name = "Пельмени", Category = "Мясо", Price = 350, ImagePath = "image 6.png", Description = "Сибирские peльмени ручной работы." },
            new MenuItem { Name = "Бефстроганов", Category = "Мясо", Price = 600, ImagePath = "image 7.png", Description = "Говядина в сливочном соусе." },
            new MenuItem { Name = "Селедка под шубой", Category = "Салаты", Price = 450, ImagePath = "image 8.png", Description = "Слоеный салат с овощами." },
            new MenuItem { Name = "Брусничный морс", Category = "Напитки", Price = 200, ImagePath = "image 9.png", Description = "Натуральный морс из диких ягод." }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

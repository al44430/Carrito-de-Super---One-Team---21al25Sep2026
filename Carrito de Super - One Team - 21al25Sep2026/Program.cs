var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 1. AGREGA ESTA LÍNEA (Normalmente abajo de builder.Services.AddRazorPages())
// Registra el servicio de sesiones en el contenedor de dependencias
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Tiempo de expiración del carrito
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Permite que funcione sin aceptar políticas de cookies complejas
});

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

app.UseRouting();

app.UseAuthorization();

// 2. AGREGA ESTA LÍNEA (¡MUY IMPORTANTE EL ORDEN!)
// Debe ir obligatoriamente DESPUÉS de UseRouting y ANTES de MapRazorPages
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()          // app.MapRazorPages();
   .WithStaticAssets();

app.Run();
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Carrito_de_Super___One_Team___21al25Sep2026.Pages
{
    // ==========================================
    // DEFINICIÓN DE MODELOS DE DATOS
    // ==========================================

    // Representa un producto del catálogo
    public class Producto
    {
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; } = string.Empty; 
    }

    // Representa un elemento dentro del carrito de compras
    public class ItemCarrito
    {
        public Producto Producto { get; set; } = new Producto();
        public int Cantidad { get; set; }
        public decimal Subtotal => Producto.Precio * Cantidad;
    }

    // ==========================================
    // CONFIGURACIÓN ESTILÍSTICA PARAMETRIZADA
    // ==========================================
    public class ConfiguracionEstilo
    {
        public string TituloTienda { get; set; } = "OneTeam Express";
        public string SloganTienda { get; set; } = "Precios siempre bajos";
        public string ColorPrimario { get; set; } = "#1E3A8A";   // Azul oscuro del encabezado
        public string ColorBotones { get; set; } = "#0EA5E9";    // Azul brillante de los botones
        public string ColorTextoBotones { get; set; } = "#FFFFFF";
        public string ColorFondo { get; set; } = "#F8FAFC";      // Fondo gris claro de la app
        public string ColorTarjeta { get; set; } = "#FFFFFF";    // Fondo de los productos
        public string FuenteFamilia { get; set; } = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
    }

    // ==========================================
    // MODELO DE LA PÁGINA (CONTROLADOR/LÓGICA)
    // ==========================================
    public class IndexModel : PageModel
    {
        // Propiedades expuestas a la vista (Index.cshtml)
        public List<Producto> CatalogoProductos { get; set; } = new List<Producto>();
        public List<ItemCarrito> Carrito { get; set; } = new List<ItemCarrito>();
        public ConfiguracionEstilo Estilos { get; set; } = new ConfiguracionEstilo();

        public decimal TotalAPagar => Carrito.Sum(item => item.Subtotal);
        public int TotalArticulos => Carrito.Sum(item => item.Cantidad);

        // Clave utilizada para almacenar el carrito en la sesión del servidor
        private const string SessionKeyCarrito = "CarritoCompras";

        public void OnGet()
        {
            // Inicializa los componentes en la carga inicial de la página
            CargarConfiguracion();
            CargarCarritoDesdeSesion();
        }

        // AGREGAR PRODUCTO DESDE EL PANEL IZQUIERDO
        public IActionResult OnPostAgregarAlCarrito(string sku)
        {
            CargarConfiguracion(); // Recarga el catálogo necesario para buscar el producto
            CargarCarritoDesdeSesion();

            // Busca el producto en el catálogo parametrizado por su SKU
            var producto = CatalogoProductos.FirstOrDefault(p => p.Sku == sku);

            if (producto != null)
            {
                // Verifica si el producto ya existe en el carrito
                var itemExistente = Carrito.FirstOrDefault(i => i.Producto.Sku == sku);
                if (itemExistente != null)
                {
                    itemExistente.Cantidad++;
                }
                else
                {
                    Carrito.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
                }

                // Guarda el estado actualizado en la sesión en formato JSON
                // HttpContext.Session.SetString(SessionKeyCarrito, JsonSerializer.Serialize(Carrito));
                GuardarCarritoEnSesion();
            }

            // Redirige a la misma página (OnGet) para evitar reenvíos de formulario duplicados
            return RedirectToPage();
        }

        // BOTÓN "+" PARA INCREMENTAR DE UNO EN UNO
        public IActionResult OnPostIncrementar(string sku)
        {
            CargarCarritoDesdeSesion();
            var item = Carrito.FirstOrDefault(i => i.Producto.Sku == sku);
            if (item != null)
            {
                item.Cantidad++;
                GuardarCarritoEnSesion();
            }
            return RedirectToPage();
        }

        // BOTÓN "-" PARA RESTAR DE UNO EN UNO
        public IActionResult OnPostDecrementar(string sku)
        {
            CargarCarritoDesdeSesion();
            var item = Carrito.FirstOrDefault(i => i.Producto.Sku == sku);
            if (item != null)
            {
                item.Cantidad--;
                // Si la cantidad llega a 0, eliminamos el renglón automáticamente
                if (item.Cantidad <= 0)
                {
                    Carrito.Remove(item);
                }
                GuardarCarritoEnSesion();
            }
            return RedirectToPage();
        }

        // BOTÓN "ELIMINAR" PARA BORRAR TODO EL RENGLÓN
        public IActionResult OnPostEliminar(string sku)
        {
            CargarCarritoDesdeSesion();
            var item = Carrito.FirstOrDefault(i => i.Producto.Sku == sku);
            if (item != null)
            {
                Carrito.Remove(item);
                GuardarCarritoEnSesion();
            }
            return RedirectToPage();
        }






        // ==========================================
        // MÉTODOS DE CONTROL INTERNO (CÓDIGO DURO)
        // ==========================================

        private void CargarConfiguracion()
        {
            // Definición de estilos visuales modificables rápidamente
            Estilos = new ConfiguracionEstilo
            {
                TituloTienda = "OneTeam Express",
                SloganTienda = "Precios siempre bajos",
                ColorPrimario = "#223d5b",     // Azul marino oscuro idéntico a la imagen
                ColorBotones = "#007bff",      // Azul brillante para añadir al carrito
                ColorTextoBotones = "#FFFFFF",
                ColorFondo = "#f4f6f9",
                ColorTarjeta = "#FFFFFF",
                FuenteFamilia = "Arial, sans-serif"
            };

            // CATALOGO DE ARTÍCULOS PARAMETRIZADO
            // Agrega o remueve elementos aquí y la interfaz se adaptará automáticamente
            CatalogoProductos = new List<Producto>
            {
                new Producto { Sku = "7500761400", Nombre = "Coca-Cola Original 600 ml",        Precio = 21.00m, ImagenUrl = "/images/7500761400.jpg" },
                new Producto { Sku = "0417890019", Nombre = "Sopa Maruchan de Camarón 64g",     Precio = 19.00m, ImagenUrl = "/images/0417890019.jpg" },
                new Producto { Sku = "1011101456", Nombre = "Sabritas Sal Original",            Precio = 27.00m, ImagenUrl = "/images/1011101456.jpg" },
                new Producto { Sku = "1011167612", Nombre = "Doritos Nacho 75g",                Precio = 26.00m, ImagenUrl = "/images/1011167612.jpg" },
                new Producto { Sku = "2111624529", Nombre = "Café Americano Mediano 360 ml",    Precio = 24.00m, ImagenUrl = "/images/2111624529.jpg" },
                new Producto { Sku = "8104100422", Nombre = "Agua Purificada Bonafont 1 Litro", Precio = 16.50m, ImagenUrl = "/images/8104100422.jpg" }
            };
        }

        private void CargarCarritoDesdeSesion()
        {
            var carritoJson = HttpContext.Session.GetString(SessionKeyCarrito);
            Carrito = !string.IsNullOrEmpty(carritoJson)
                ? JsonSerializer.Deserialize<List<ItemCarrito>>(carritoJson) ?? new List<ItemCarrito>()
                : new List<ItemCarrito>();
        }

        private void GuardarCarritoEnSesion()
        {
            HttpContext.Session.SetString(SessionKeyCarrito, JsonSerializer.Serialize(Carrito));
        }
    }
}

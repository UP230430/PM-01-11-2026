using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IProductoRepository repo, ILogger<ProductosController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // Respuesta uniforme para errores de base de datos
    private ObjectResult ErrorBd(MySqlException ex)
    {
        _logger.LogError(ex, "Error de MySQL");
        return StatusCode(503, new { mensaje = "No se pudo comunicar con la base de datos.", detalle = ex.Message });
    }

    // Respuesta uniforme para cualquier otro error
    private ObjectResult ErrorGeneral(Exception ex)
    {
        _logger.LogError(ex, "Error inesperado");
        return StatusCode(500, new { mensaje = "Ocurrió un error inesperado.", detalle = ex.Message });
    }

    [HttpGet]
    public ActionResult<List<Producto>> ObtenerTodos()
    {
        try { return Ok(_repo.ObtenerTodos()); }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerPorId(int id)
    {
        try
        {
            var producto = _repo.ObtenerPorId(id);
            return producto is null ? NotFound($"No existe el producto {id}") : Ok(producto);
        }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }

    [HttpGet("buscar")]
    public ActionResult<List<Producto>> Buscar([FromQuery] string termino)
    {
        try { return Ok(_repo.Buscar(termino)); }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }

    [HttpPost]
    public ActionResult<Producto> Insertar([FromBody] Producto producto)
    {
        try
        {
            producto.Id = _repo.Insertar(producto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, [FromBody] Producto producto)
    {
        try
        {
            producto.Id = id;
            return _repo.Actualizar(producto)
                ? NoContent()
                : NotFound($"No existe el producto {id}");
        }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            return _repo.Eliminar(id)
                ? NoContent()
                : NotFound($"No existe el producto {id}");
        }
        catch (MySqlException ex) { return ErrorBd(ex); }
        catch (Exception ex) { return ErrorGeneral(ex); }
    }
}
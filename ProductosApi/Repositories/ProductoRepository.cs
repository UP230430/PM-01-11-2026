using MySqlConnector;
using ProductosApi.Data;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly ConexionDb _db;

    public ProductoRepository(ConexionDb db) => _db = db;

    private const string Columnas =
        "id, nombre, descripcion, precio, stock, fecha_creacion";

    private static Producto Mapear(MySqlDataReader r) => new Producto
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Descripcion = r.IsDBNull(2) ? null : r.GetString(2),
        Precio = r.GetDecimal(3),
        Stock = r.GetInt32(4),
        FechaCreacion = r.GetDateTime(5)
    };

    public List<Producto> ObtenerTodos()
    {
        var lista = new List<Producto>();
        using var cn = _db.CrearConexion();
        cn.Open();
        using var cmd = new MySqlCommand($"SELECT {Columnas} FROM productos", cn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var cn = _db.CrearConexion();
        cn.Open();
        using var cmd = new MySqlCommand(
            $"SELECT {Columnas} FROM productos WHERE id = @Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public int Insertar(Producto p)
    {
        using var cn = _db.CrearConexion();
        cn.Open();
        const string sql = @"
            INSERT INTO productos (nombre, descripcion, precio, stock)
            VALUES (@Nombre, @Descripcion, @Precio, @Stock);
            SELECT LAST_INSERT_ID();";
        using var cmd = new MySqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@Descripcion", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Precio", p.Precio);
        cmd.Parameters.AddWithValue("@Stock", p.Stock);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool Actualizar(Producto p)
    {
        using var cn = _db.CrearConexion();
        cn.Open();
        const string sql = @"
            UPDATE productos
            SET nombre = @Nombre, descripcion = @Descripcion,
                precio = @Precio, stock = @Stock
            WHERE id = @Id";
        using var cmd = new MySqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", p.Id);
        cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@Descripcion", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Precio", p.Precio);
        cmd.Parameters.AddWithValue("@Stock", p.Stock);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = _db.CrearConexion();
        cn.Open();
        using var cmd = new MySqlCommand("DELETE FROM productos WHERE id = @Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public List<Producto> Buscar(string termino)
    {
        var lista = new List<Producto>();
        using var cn = _db.CrearConexion();
        cn.Open();
        var sql = $@"
            SELECT {Columnas} FROM productos
            WHERE nombre LIKE @T OR descripcion LIKE @T";
        using var cmd = new MySqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@T", $"%{termino}%");
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }
}
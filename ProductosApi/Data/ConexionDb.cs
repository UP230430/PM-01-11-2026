using MySqlConnector;

namespace ProductosApi.Data;

public class ConexionDb
{
    private readonly string _cadena;

    public ConexionDb(IConfiguration config)
    {
        _cadena = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta la cadena de conexión.");
    }

    public MySqlConnection CrearConexion() => new MySqlConnection(_cadena);
}
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BackendGeems.Domain;
using BackendGeems.Application;

namespace BackendGeems.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmpleadoRepo _EmpleadoRepo;
        private readonly BorradoDeEmpleados _borradoDeEmpleados;

        public AuthController(IConfiguration configuration,IEmpleadoRepo EmpleadoRepo,BorradoDeEmpleados borradoDeEmpleados)
        {
            _configuration = configuration;
            _borradoDeEmpleados = borradoDeEmpleados;
            _EmpleadoRepo = EmpleadoRepo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario request)
        {
            Console.WriteLine($"Identificador recibido: {request.Username}");
            Console.WriteLine($"Contraseña: {request.Contrasena}");

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Contrasena))
            {
                return BadRequest(new { message = "Debe completar todos los campos" });
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT * FROM Usuario
            WHERE Username = @identificador OR CorreoPersona = @identificador
        ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@identificador", request.Username);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    string contrasenaDB = reader["Contrasena"].ToString();

                    if (request.Contrasena == contrasenaDB)
                    {
                        if(_borradoDeEmpleados.UsuarioActivo(Convert.ToString(reader["CedulaPersona"])))
                        {
                             return Ok(new
                        {
                            message = "Inicio de sesión exitoso",
                            usuario = new
                            {
                                id = reader["Id"].ToString(),
                                tipo = reader["Tipo"].ToString(),
                                cedulaPersona = Convert.ToInt32(reader["CedulaPersona"]),
                                nombreUsuario = reader["Username"].ToString(),
                                contrasena = reader["Contrasena"].ToString()
                            }
                        });
                        }
                        else
                        {
                            return Unauthorized(new { message = "Usuario o contraseña incorrecta" });
                        }
                       
                    }
                    else
                    {
                        return Unauthorized(new { message = "Usuario o contraseña incorrecta" });
                    }
                }
                else
                {
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
                }
            }
        }

    }
}

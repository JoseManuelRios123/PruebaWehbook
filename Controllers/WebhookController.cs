using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PruebaWebhook.Models;
using System.Data.SqlTypes;

namespace PruebaWebhook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly string cadenaSQL;

        public WebhookController(IConfiguration configuration)
        {
            cadenaSQL = configuration.GetConnectionString("CadenaSQL");
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<WebhookModel> lista = new List<WebhookModel>();

            try
            {
                Logger.Log("Solicitud GET a Lista: Iniciando la recuperación de datos.");

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarData", conexion);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Logger.Log($"Notificación recibida - " +
                                $"ID: {Convert.ToInt32(rd["Id"])}, " +
                                $"RequestId: {Convert.ToInt32(rd["RequestId"])}, " +
                                $"Status: {rd["Status"]}, " +
                                $"Message: {Convert.ToString(rd["Message"])}");

                            lista.Add(new WebhookModel()
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                RequestId = Convert.ToInt32(rd["RequestId"]),
                                Status = rd["Status"].ToString(),
                                Message = Convert.ToString(rd["Message"]),
                                Date = Convert.ToDateTime(rd["Date"]),
                                Document = rd["Document"].ToString(),
                                DocumentType = rd["DocumentType"].ToString(),
                                Name = rd["Name"].ToString(),
                                Surname = rd["Surname"].ToString(),
                                Mobile = rd["Mobile"].ToString(),
                                Email = rd["Email"].ToString(),
                                Total = Convert.ToInt32(rd["Total"]),
                                IssuerName = rd["IssuerName"].ToString(),
                                PaymentMethodName = rd["PaymentMethodName"].ToString(),
                            });
                        }
                    }

                    Logger.Log("Solicitud GET a Lista: Datos recuperados con éxito.");
                }

                Logger.Log("Solicitud GET a Lista: Respuesta enviada correctamente.");

                return StatusCode(StatusCodes.Status200OK, new { response = lista });
            }
            catch (Exception ex)
            {
                Logger.Log($"Error en la solicitud GET a Lista: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }


        [HttpGet]
        [Route("Obtener/{RequestId:int}")]
        public IActionResult Obtener(int RequestId)
        {
            List<WebhookModel> lista = new List<WebhookModel>();
            WebhookModel webhook = new WebhookModel();

            try
            {
                Logger.Log($"Solicitud GET a Obtener/{RequestId}: Iniciando la recuperación de datos por ID.");

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarData", conexion);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new WebhookModel()
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                RequestId = Convert.ToInt32(rd["RequestId"]),
                                Status = rd["Status"].ToString(),
                                Message = Convert.ToString(rd["Message"]),
                                Date = Convert.ToDateTime(rd["Date"]),
                                Document = rd["Document"].ToString(),
                                DocumentType = rd["DocumentType"].ToString(),
                                Name = rd["Name"].ToString(),
                                Surname = rd["Surname"].ToString(),
                                Mobile = rd["Mobile"].ToString(),
                                Email = rd["Email"].ToString(),
                                Total = Convert.ToInt32(rd["Total"]),
                                IssuerName = rd["IssuerName"].ToString(),
                                PaymentMethodName = rd["PaymentMethodName"].ToString(),
                            });
                        }
                    }

                    Logger.Log($"Solicitud GET a Obtener/{RequestId}: Datos recuperados con éxito.");
                }

                webhook = lista.Where(item => item.RequestId == RequestId).FirstOrDefault();

                Logger.Log($"Solicitud GET a Obtener/{RequestId}: Respuesta enviada correctamente.");

                return StatusCode(StatusCodes.Status200OK, new { response = webhook });
            }
            catch (Exception ex)
            {
                Logger.Log($"Error en la solicitud GET a Obtener/{RequestId}: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }


        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] WebhookModel objeto)
        {

            try
            {
                Logger.Log($"Solicitud POST a Guardar: Guardando las notificaciones recientes");

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("InsertarWebhook", conexion);
                    cmd.Parameters.AddWithValue("RequestId", objeto.RequestId);
                    cmd.Parameters.AddWithValue("Date", objeto.Date);
                    cmd.Parameters.AddWithValue("Document", objeto.Document);
                    cmd.Parameters.AddWithValue("DocumentType", objeto.DocumentType);
                    cmd.Parameters.AddWithValue("Name", objeto.Name);
                    cmd.Parameters.AddWithValue("Surname", objeto.Surname);
                    cmd.Parameters.AddWithValue("Mobile", objeto.Mobile);
                    cmd.Parameters.AddWithValue("Email", objeto.Email);
                    cmd.Parameters.AddWithValue("Status", objeto.Status);
                    cmd.Parameters.AddWithValue("Total", objeto.Total);
                    cmd.Parameters.AddWithValue("Message", objeto.Message);
                    cmd.Parameters.AddWithValue("IssuerName", objeto.IssuerName);
                    cmd.Parameters.AddWithValue("PaymentMethodName", objeto.PaymentMethodName);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                    Logger.Log($"Solicitud POST a Guardar: Notificación recibida con éxito.");

                }

                Logger.Log($"Solicitud POST a Guardar: Respuesta enviada correctamente.");

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Notificación recibida con éxito" });
            }
            catch (Exception ex)
            {
                Logger.Log($"Error en la solicitud POST a Guardar: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });

            }
        }
    }
}

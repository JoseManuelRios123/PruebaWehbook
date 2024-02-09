using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prueba2Webhook.Models;

namespace Prueba2Webhook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly string cadenaSQL;
        private readonly servicioWebhookContext _context;

        public WebhookController(IConfiguration configuration, servicioWebhookContext context)
        {
            cadenaSQL = configuration.GetConnectionString("CadenaSQL");
            _context = context;
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            try
            {
                Logger.Log("Solicitud GET a Lista: Iniciando la recuperación de datos.");

                List<WebhookListModel> lista = new List<WebhookListModel>();

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ListarWebhookModel", conexion);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(BuildWebhookModel(rd));
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

        private WebhookListModel BuildWebhookModel(SqlDataReader rd)
        {
            return new WebhookListModel()
            {
                WebhookModelId = Convert.ToInt32(rd["WebhookModelId"]),
                RequestId = Convert.ToInt32(rd["RequestId"]),
                Status = new StatusDetails()
                {
                    Status = rd["StatusValue"].ToString(),
                    Reason = rd["Reason"].ToString(),
                    Message = Convert.ToString(rd["Message"]),
                    Date = Convert.ToDateTime(rd["Date"])
                },
                Request = new Request
                {
                    Payer = new Payer()
                    {
                        Document = rd["Document"].ToString(),
                        DocumentType = rd["DocumentType"].ToString(),
                        Name = rd["Name"].ToString(),
                        Surname = rd["Surname"].ToString(),
                        Mobile = rd["Mobile"].ToString(),
                        Email = rd["Email"].ToString()
                    },
                },
                Payment = new List<PaymentDetails>()
                {
                    new PaymentDetails()
                    {
                        IssuerName = rd["IssuerName"].ToString(),
                        PaymentMethodName = rd["PaymentMethodName"].ToString(),
                        Amount = new AmountDetails
                        {
                            To = new ToDetails
                            {
                                Total = Convert.ToInt32(rd["Total"]),
                            }
                        }
                    }
                }
            };
        }

        [HttpGet]
        [Route("Obtener/{RequestId:int}")]
        public IActionResult Obtener(int RequestId)
        {
            try
            {
                Logger.Log($"Solicitud GET a Obtener/{RequestId}: Iniciando la recuperación de datos por ID.");

                WebhookListModel webhook = GetWebhookById(RequestId);

                if (webhook != null)
                {
                    Logger.Log($"Solicitud GET a Obtener/{RequestId}: Datos recuperados con éxito.");
                    return StatusCode(StatusCodes.Status200OK, new { response = webhook });
                }
                else
                {
                    Logger.Log($"Solicitud GET a Obtener/{RequestId}: No se encontró la notificación.");
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Notificación no encontrada" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error en la solicitud GET a Obtener/{RequestId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        private WebhookListModel GetWebhookById(int requestId)
        {
            WebhookListModel webhook = null;

            using (var conexion = new SqlConnection(cadenaSQL))
            {
                conexion.Open();
                var cmd = new SqlCommand("ListarWebhookModel", conexion);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (Convert.ToInt32(rd["RequestId"]) == requestId)
                        {
                            webhook = BuildWebhookModel(rd);
                            break;
                        }
                    }
                }
            }

            return webhook;
        }


        [HttpPost]
        [Route("Guardar")]
        public IActionResult ReceiveWebhook([FromBody] WebhookListModel webhookData)
        {
            try
            {
                InsertarEnDB(webhookData);

                Logger.Log("Solicitud POST a Guardar: Respuesta enviada correctamente.");
                return StatusCode(StatusCodes.Status200OK, new { response = webhookData });
            }
            catch (Exception ex)
            {
                Logger.Log($"Error en la solicitud POST a Guardar: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        private void InsertarEnDB(WebhookListModel webhookData)
        {
            try
            {
                Logger.Log("Iniciando inserción de datos en la base de datos.");

                var request = webhookData.Request;

                var status = new SqlParameter("@StatusValue", webhookData.Status?.Status);
                var reason = new SqlParameter("@Reason", webhookData.Status?.Reason);
                var message = new SqlParameter("@Message", webhookData.Status?.Message);
                var date = new SqlParameter("@Date", webhookData.Status?.Date ?? DateTime.Now);

                var payer = request?.Payer;

                var document = new SqlParameter("@Document", payer?.Document);
                var documentType = new SqlParameter("@DocumentType", payer?.DocumentType);
                var name = new SqlParameter("@Name", payer?.Name);
                var surname = new SqlParameter("@Surname", payer?.Surname);
                var email = new SqlParameter("@Email", payer?.Email);
                var mobile = new SqlParameter("@Mobile", payer?.Mobile);

                var payment = webhookData.Payment?.FirstOrDefault();

                var issuerName = new SqlParameter("@IssuerName", payment?.IssuerName);
                var paymentMethodName = new SqlParameter("@PaymentMethodName", payment?.PaymentMethodName);
                var total = new SqlParameter("@Total", payment?.Amount?.To?.Total ?? 0);

                _context.Database.ExecuteSqlRaw("InsertarWebhookModel @StatusValue, @Reason, @Message, @Date, " +
                    "@Document, @DocumentType, @Name, @Surname, @Email, @Mobile, " +
                    "@IssuerName, @PaymentMethodName, @Total",
                    status, reason, message, date, document, documentType, name, surname, email, mobile,
                    issuerName, paymentMethodName, total);

                Logger.Log("Datos insertados correctamente en la base de datos.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error al insertar los datos: {ex.Message}");
                throw;
            }
        }
    }
}

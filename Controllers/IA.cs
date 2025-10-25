using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using Newtonsoft.Json;
using PROYECTOMOVIE.Recursos.IA;

namespace PROYECTOMOVIE.Controllers
{
    public class IA : Controller
    {
        public static string _EndPoint = "https://api.openai.com/";
        public static string _URI = "v1/chat/completions";
        public static string _APIKey = "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";
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string pSolicitud)
        {

                var strRespuesta = "";
                //Consumir API
                var oCliente = new RestClient(_EndPoint);
                var oSolicitud = new RestRequest(_URI, Method.Post);
                oSolicitud.AddHeader("Content-Type", "application/json");
                oSolicitud.AddHeader("Authorization", "Bearer" + _APIKey);

                //cREAR CUERPO
                var oCuerpo = new Request()
                {
                    model = "gpt-5",
                    messages = new List<Message>()
                    {
                        new Message()
                        {
                            role = "developer",
                            content = pSolicitud
                        }
                    }
                };

                var jsonString = JsonConvert.SerializeObject(oCuerpo);

                oSolicitud.AddJsonBody(jsonString);

                var oRespuesta = oCliente.Post<Response>(oSolicitud);
                strRespuesta = oRespuesta.choices[0].message.content;

                ViewBag.Respuesta = oRespuesta;

                return View();
        }
    }
}

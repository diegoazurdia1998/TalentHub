using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TalentHub.Models;
using Lab1Consola.Models;
using Lab1Consola.Services;
using Lab1Consola.Utils;
using System.Text.Json;

namespace TalentHub.Controllers
{
    public class HomeController : Controller
    {
        public static ApplicantService services;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult CargarArchivo(String filePath)
        {
            services = new ApplicantService(filePath);
            if (services.isNotEmpty())
            {
                ViewBag.errores = services.errores;
                return View("CargarArchivo");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        
        public IActionResult Visualizer(bool DPISearch, String search)
        {
            if (services != null && services.isNotEmpty())
            {
                services.extraerSolicitantes();
                CompressingOperations operation = new CompressingOperations();
                if (search == null || search == String.Empty)
                {
                    List<Applicant> Compressed = services.extraerSolicitantes(), Decompressed = new List<Applicant>();
                    Applicant applicant = new Applicant();
                    foreach (var appl in Compressed)
                    {
                        applicant = appl.Clone();
                        applicant.companies = operation.DecompressDPICompany(appl);
                        Decompressed.Add(applicant);
                    }

                    ViewData["solicitantes"] = Decompressed;
                }
                else
                {
                    List<Applicant> Answer, Decompressed = new List<Applicant>();
                    if (DPISearch)
                    {
                        Answer = services.buscarDPI(search);
                        if (Answer.Count > 0)
                        {
                            Applicant applicant;
                            foreach (var appl in Answer)
                            {
                                applicant = appl.Clone();
                                applicant.companies = operation.DecompressDPICompany(appl);
                                Decompressed.Add(applicant);
                            }
                            ViewData["solicitantes"] = Decompressed;
                        }
                    }
                    else
                    {
                        Answer = services.buscarNombre(search);
                        if (Answer.Count > 0)
                        {
                            Applicant applicant;
                            foreach (var appl in Answer)
                            {
                                applicant = appl.Clone();
                                applicant.companies = operation.DecompressDPICompany(appl);
                                Decompressed.Add(applicant);
                            }
                            ViewData["solicitantes"] = Decompressed;
                        }
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public IActionResult Bitacoras(string name)
        {
            if (services != null && services.isNotEmpty())
            {
                services.ExtraerBitacoraNombre(name);
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public IActionResult VisualizerLetters(string applicant, bool letters)
        {
            Applicant tempApplicant = services.buscarDPI(applicant).ToArray()[0].Clone();
            ViewBag.applicant = tempApplicant;
            ViewBag.letters = letters;
            if(!letters) ViewBag.conversations = services.ValidateConversations(tempApplicant);
            
            return View();
        }
        public IActionResult VisualizerDetails(string applicant)
        {

            ViewBag.applicant = services.buscarDPI(applicant).ToArray()[0].Clone();
            
            return View();
        }
    }
}

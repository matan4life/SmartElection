using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartElectionFrontend.Models;

namespace SmartElectionFrontend.Controllers
{
    public class ElectionController : Controller
    {
        public static int ID { get; set; }
        private readonly Client client = new Client("http://localhost:56010", new System.Net.Http.HttpClient());
        public async Task<IActionResult> Index()
        {
            return View(await client.GetElectionsAsync());
        }

        public IActionResult Create()
        {
            return View(new Election());
        }
        [HttpPost]
        public async Task<IActionResult> PostCreate(Election model)
        {
            model.UserId = HomeController.UserId;
            await client.PostElectionAsync(model.Start, model.End, model.NeedsCertificate, model.NeedsFingerprint, model.CalculationType, model.UserId);
            return RedirectToAction("Index");
        }

        public IActionResult AddAlternative(int id)
        {
            ID = id;
            return View(new Alternative());
        }

        [HttpPost]
        public async Task<IActionResult> PostAlternative(Alternative model)
        {
            model.ElectionId = ID;
            try
            {
                await client.PostAlternativeAsync(model.Name, model.Info, model.ElectionId);
                return RedirectToAction("Index");
            }
            catch (SwaggerException)
            {
                return RedirectToAction("AddAlternative");
            }
        }

        public async Task<IActionResult> Vote(int id)
        {
            ID = id;
            var model = new AlternativeViewModel();
            model.Alternatives = (await client.GetAlternativesAsync(id)).ToList().Select(x=>new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = x.Name,
                Text = x.Name
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PostVote(string alternative)
        {
            int candidateId = (await client.GetAlternativesAsync(ID)).Where(x => x.Name == alternative).FirstOrDefault().Id.Value;
            try
            {
                await client.PostVoteAsync(HomeController.UserId, ID, 1, candidateId);
                return RedirectToAction("Index");
            }
            catch (SwaggerException)
            {
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Results(int id)
        {
            ID = id;
            return View(await client.GetAlternativeAsync(2));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
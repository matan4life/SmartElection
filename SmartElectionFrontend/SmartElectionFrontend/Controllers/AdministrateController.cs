using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartElectionFrontend.Controllers
{
    public class AdministrateController : Controller
    {
        private readonly Client client = new Client("http://localhost:56010", new System.Net.Http.HttpClient());
        public static string ID { get; set; } = "";
        public async Task<IActionResult> Index()
        {
            return View(await client.GetUsersAsync());
        }

        public IActionResult Create()
        {
            return View(new RegisterViewModel());
        }

        public async Task<IActionResult> Delete(string id)
        {
            ID = id;
            return View(await client.GetUserAsync(id));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await client.GetUserAsync(id);
            return View(new EditViewModel()
            {
                Id = id,
                Country = user.Country,
                Email = user.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> PostCreate(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                await client.PostUserAsync(model);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        public async Task<IActionResult> PutEdit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await client.PutUserAsync(model.Id, model);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> PostDelete()
        {
            await client.DeleteUserAsync(ID);
            return RedirectToAction("Index");
        }
    }
}
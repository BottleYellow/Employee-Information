using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EIS.WebApp.Controllers
{
    public class PeopleController : Controller
    {
     
        public readonly IEISService service;
        static string imageBase64Data;
        public PeopleController(IEISService service)
        {
            this.service = service;
            
        }
        public IActionResult Index()
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.GetAsync("api/employee").Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Person> data = JsonConvert.DeserializeObject<List<Person>>(stringData);
            return View(data);
        }

        //Get : People by id
        public IActionResult Details(int id)
        {
            TempData["Id"]=HttpContext.Session.GetInt32("Id");
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.GetAsync("api/employee/" + id + "").Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            imageBase64Data = Convert.ToBase64String(data.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImageData = imageDataURL;
            ViewBag.Name = data.FirstName + " " + data.LastName;
            return View("Profile",data);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("IdCard,PanCard,AadharCard,Image,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,Designation,Id")]Person person, IList<IFormFile> file)
        {
            person.CreatedDate = DateTime.Now.Date;
            IFormFile uploadedImage = file.FirstOrDefault();
            if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
            {
                MemoryStream ms = new MemoryStream();
                uploadedImage.OpenReadStream().CopyTo(ms);

                Image image = Image.FromStream(ms);
                person.Image = ms.ToArray();

                if (ModelState.IsValid)
                {
                    HttpClient client = service.GetService();
                    string stringData = JsonConvert.SerializeObject(person);
                    var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync("api/employee/Create", contentData).Result;
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: People/Edit/5
        public IActionResult Edit(int id)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.GetAsync("api/employee/" + id + "").Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            imageBase64Data = Convert.ToBase64String(data.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            //HttpContext.Session.SetString("image", imageDataURL);
            ViewBag.ImageData = imageDataURL;
            return View(data);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IdCard,PanCard,AadharCard,Image,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,Designation,Id,CreatedDate,UpdatedDate,IsActive,RowVersion")] Person person)
        {

            if (id != person.Id)
            {
                return NotFound();
            }

            byte[] imageBytes = Convert.FromBase64String(imageBase64Data);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            person.Image = ms.ToArray();
            person.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                try
                {
                    HttpClient client = service.GetService();
                    HttpResponseMessage response = client.PutAsJsonAsync("api/employee/Edit/" + id + "", person).Result;
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    var entry = ex.Entries.Single();
                    var clientValues = (Person)entry.Entity;
                    var databaseValues = (Person)entry.GetDatabaseValues().ToObject();

                    if (databaseValues.FirstName != clientValues.FirstName)
                        ModelState.AddModelError("First Name", "Current value: "
                            + databaseValues.FirstName);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.");
                    person.RowVersion = databaseValues.RowVersion;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(person);
        }

        // GET: People/Delete/5
        public IActionResult Delete(int id)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.GetAsync("api/employee/" + id + "").Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            imageBase64Data = Convert.ToBase64String(data.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImageData = imageDataURL;
            return View(data);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.DeleteAsync("api/employee/Delete/" + id + "").Result;
            return RedirectToAction(nameof(Index));
        }
    }
}
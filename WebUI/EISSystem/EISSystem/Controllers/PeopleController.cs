using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
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
        #region Declarations
        public readonly IEISService<Person> service;
        public readonly IEISService<Permanent> perService;
        public readonly IEISService<Current> currentService;
        public readonly IEISService<Emergency> emergencyService;
        static string imageBase64Data;
        #endregion

        #region Controller
        public PeopleController(IEISService<Person> service, IEISService<Permanent> perService, IEISService<Current> currentService, IEISService<Emergency> emergencyService)
        {
            this.service = service;
            this.perService = perService;
            this.currentService = currentService;
            this.emergencyService = emergencyService;
        }
        #endregion

        #region Employee
        public IActionResult Index()
        {
            HttpResponseMessage response = service.GetResponse("api/employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Person> data = JsonConvert.DeserializeObject<List<Person>>(stringData);
            Response.StatusCode = (int)response.StatusCode;

            return View(data);
        }

        public IActionResult Profile(int id)
        {
            HttpResponseMessage response = service.GetResponse("api/employee/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            string permanent = perService.GetResponse("api/PermanentAddress/" + id + "").Content.ReadAsStringAsync().Result;
            string current = currentService.GetResponse("api/CurrentAddress/" + id + "").Content.ReadAsStringAsync().Result;
            string emergency = emergencyService.GetResponse("api/EmergencyAddress/" + id + "").Content.ReadAsStringAsync().Result;
            string other = perService.GetResponse("api/OtherAddress/" + id + "").Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            data.PermanentAddress = JsonConvert.DeserializeObject<Permanent>(permanent);
            data.CurrentAddress = JsonConvert.DeserializeObject<Current>(current);
            data.EmergencyAddress= JsonConvert.DeserializeObject<List<Emergency>>(emergency);
            data.OtherAddress = JsonConvert.DeserializeObject<List<Other>>(other);
            if (data != null)
            {
                imageBase64Data = Convert.ToBase64String(data.Image);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;
                ViewBag.Name = data.FirstName + " " + data.LastName;
            }
            if (data.PermanentAddress == null)
                data.PermanentAddress = new Permanent() { PersonId = id };
            var cur = new Current() { PersonId = id };
            if (data.CurrentAddress == null)
                data.CurrentAddress = cur;
            Response.StatusCode = (int)response.StatusCode;
            return View("Profile", data);
        }

        public IActionResult Create()
        {
            return View();
        }
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
                    HttpResponseMessage response = service.PostResponse("api/employee", person);
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(person);
        }
        public IActionResult Edit(int id)
        {
            string stringData = service.GetResponse("api/employee/" + id + "").Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            imageBase64Data = Convert.ToBase64String(data.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImageData = imageDataURL;
            return View(data);
        }

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
                    HttpResponseMessage response = service.PutResponse("api/employee/" + id + "", person);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.DeleteAsync("api/employee/Delete/" + id + "").Result;
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Permanent Address

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePermanentAddress(int id, [Bind("Address,City,State,Country,PinCode,PhoneNumber")]Permanent permanent)
        {

            permanent.PersonId = id;
            permanent.CreatedDate = DateTime.Now.Date;
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = perService.PostResponse("api/PermanentAddress", permanent);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPermanentAddress(int id, Permanent permanent)
        {
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = perService.PutResponse("api/PermanentAddress", permanent);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = id });
        }
        public IActionResult DeletePermanentAddress(int id)
        {
            HttpResponseMessage response = perService.DeleteResponse("api/PermanentAddress/" + id + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Current Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCurrentAddress(int id, [Bind("Address,City,State,Country,PinCode,PhoneNumber")]Current current)
        {

            current.PersonId = id;
            current.CreatedDate = DateTime.Now.Date;
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = currentService.PostResponse("api/CurrentAddress", current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrentAddress(int id, Current current)
        {
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = currentService.PutResponse("api/CurrentAddress", current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = id });
        }
        public IActionResult DeleteCurrentAddress(int id)
        {
            HttpResponseMessage response = currentService.DeleteResponse("api/CurrentAddress/" + id + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Emergency Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmergencyAddress(int pid, Emergency emergency)
        {

            emergency.PersonId = pid;
            emergency.CreatedDate = DateTime.Now.Date;
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = emergencyService.PostResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [HttpGet]
        public IActionResult EditEmergencyAddress(int id)
        {
            HttpResponseMessage response = emergencyService.GetResponse("api/EmergencyAddress/Get/"+id+"");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            string emergency = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Emergency>(emergency);
            return PartialView("EditEmergencyAddress", data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmergencyAddress(int id, Emergency emergency)
        {
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = emergencyService.PutResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Profile", "People", new { id = id });
        }
        public IActionResult DeleteEmergencyAddress(int id)
        {
            HttpResponseMessage response = emergencyService.DeleteResponse("api/EmergencyAddress/" + id + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
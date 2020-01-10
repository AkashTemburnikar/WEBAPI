using Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Controllers
{
	public class EmployeeController : Controller
	{
		// GET: Employee
		public ActionResult Index()
		{
			IEnumerable<mvcEmployeeModel> empList;
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:64028/api/");
				var responseTask = client.GetAsync("Employee");
				responseTask.Wait();
				var result = responseTask.Result;
				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsAsync<IEnumerable<mvcEmployeeModel>>();
					readTask.Wait();
					empList = readTask.Result;
				}
				else
				{
					empList = Enumerable.Empty<mvcEmployeeModel>();
					ModelState.AddModelError(string.Empty, "Server error try after some time.");
				}
			}
			return View(empList);
		}

		 public ActionResult AddOrEdit(int id = 0)
        {
			
            if (id == 0)
                return View(new mvcEmployeeModel());
            else
            {
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri("http://localhost:64028/api/");
					var response = client.GetAsync("Employee/" + id.ToString());
					response.Wait();
					var result = response.Result;
					if (result.IsSuccessStatusCode)
					{
						var readTask = result.Content.ReadAsAsync<mvcEmployeeModel>();
						return View(readTask.Result);
					}
					else
					{
						return View();
					}
				}
            }
        }

		[HttpPost]
		public ActionResult AddOrEdit(mvcEmployeeModel emp)
		{
			if (emp.EmployeeID == 0)
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri("http://localhost:64028/api/");
					var postTask = client.PostAsJsonAsync("Employee", emp);
					postTask.Wait();
					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						TempData["SuccessMessage"] = "Saved Successfully";
					}
					else
					{
						TempData["SuccessMessage"] = "Save Failed";
					}
				}
			}
			else
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri("http://localhost:64028/api/");
					var postTask = client.PutAsJsonAsync("Employee/" + emp.EmployeeID, emp);
					postTask.Wait();
					var result = postTask.Result;
					if (result.IsSuccessStatusCode)
					{
						TempData["SuccessMessage"] = "Updated Successfully";
					}
					else
					{
						TempData["SuccessMessage"] = "Update Failed";
					}
				}
			}
			return RedirectToAction("Index");
		}

		public ActionResult Delete(int id)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("http://localhost:64028/api/");
				var postTask = client.DeleteAsync("Employee/" + id.ToString());
				postTask.Wait();
				var result = postTask.Result;
				if (result.IsSuccessStatusCode)
				{
					TempData["SuccessMessage"] = "Deleted Successfully";
				}
				else
				{
					TempData["SuccessMessage"] = "Delete Failed";
				}
				TempData["SuccessMessage"] = "Deleted Successfully";
				return RedirectToAction("Index");
			}
		}

	}
}
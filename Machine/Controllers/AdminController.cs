using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Machine.Abstract;
using Machine.Concrete;
using Machine.Models;

namespace Machine.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private IProductRepository repository;

        public AdminController(IProductRepository repo)
        {
            repository = repo;
        }
        public ViewResult Index()
        {
            return View();
        }
        public ViewResult Drink()
        {
            return View(repository.Drinks);
        }
        public ViewResult Edit(int productId)
        {
            Drink drink = repository.Drinks
            .FirstOrDefault(p => p.ProductID == productId);
            return View(drink);
        }
        [HttpPost]
        public ActionResult Edit(Drink drink, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    drink.ImageMimeType = image.ContentType;
                    drink.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(drink.ImageData, 0, image.ContentLength);
                }
                repository.SaveProduct(drink);
                TempData["message"] = string.Format("{0} has been saved", drink.Name);
                return RedirectToAction("Drink");
            }
            else
            {
                // there is something wrong with the data values
                return View(drink);
            }
        }
        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Drink deletedProduct = repository.DeleteDrink(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
            }
            return RedirectToAction("Drink");
        }
        public ViewResult Create()
        {
            return View("Edit", new Drink());
        }
        public ViewResult Coin()
        {
            return View(repository.Coins);
        }
        
        public ViewResult EditCoin(int CoinID)
        {
            Coin coin = repository.Coins
            .FirstOrDefault(p => p.CoinID == CoinID);
            return View(coin);
        }
        [HttpPost]
        public ActionResult EditCoin(Coin coin)
        {
            if (ModelState.IsValid)
            {
                repository.SaveCoin(coin);
                TempData["message"] = string.Format("{0} has been saved", coin.SNameCoin);
                return RedirectToAction("Coin");
            }
            else
            {
                // there is something wrong with the data values
                return View(coin);
            }
        }
    }
}
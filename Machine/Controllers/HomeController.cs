using Machine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Machine.Concrete;
using Machine.Abstract;

namespace Machine.Controllers
{
    public class HomeController : Controller
    {

        private IProductRepository repository;
        public HomeController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }
        //private void PlusCoin(string ValueCoin)
        //{
        //    HttpContext.Application.Lock();
        //    int count = 0;

        //    if (HttpContext.Application[ValueCoin] != null)
        //        count = (int)HttpContext.Application[ValueCoin];

        //    count++;
        //    HttpContext.Application[ValueCoin] = count;


        //    HttpContext.Application.UnLock();
        //}
        //private void DefaultGlobalVariable()
        //{
        //    if ((int)(HttpContext.Application["1 руб."]) > 0) SaveCoinInBase("One", "1 руб.", 0);
        //    if ((int)(HttpContext.Application["2 руб."]) > 0) SaveCoinInBase("Two", "2 руб.", 0);
        //    if ((int)(HttpContext.Application["5 руб."]) > 0) SaveCoinInBase("Five", "5 руб.", 0);
        //    if ((int)(HttpContext.Application["10 руб."]) > 0) SaveCoinInBase("Ten", "10 руб.", 0);
        //    HttpContext.Application["1 руб."] = 0;
        //    HttpContext.Application["2 руб."] = 0;
        //    HttpContext.Application["5 руб."] = 0;
        //    HttpContext.Application["10 руб."] = 0;
        //}
        public void DefaultViewBag()
        {
            ViewBag.BDontOne = false;
            ViewBag.BDontTwo = false;
            ViewBag.BDontFive = false;
            ViewBag.BDontTen = false;
            foreach (var c in repository.Coins)
            {
                if ((c.SNameCoin == "One") & (c.BDontCoin)) ViewBag.BDontOne = true;

                if ((c.SNameCoin == "Two") & (c.BDontCoin)) ViewBag.BDontTwo = true;

                if ((c.SNameCoin == "Five") & (c.BDontCoin)) ViewBag.BDontFive = true;

                if ((c.SNameCoin == "Ten") & (c.BDontCoin)) ViewBag.BDontTen = true;
            }
        }
        private Dictionary<int, int> CalculateChange(int Money)
        {
            Dictionary<int, int> Dic = new Dictionary<int, int>();
            int[] FaceValues = { 10, 5, 2, 1 };
            foreach (int item in FaceValues)
            {
                if (Money / item == 0) continue;
                Dic.Add(item, Money / item);
                Money %= item;
                if (Money == 0) break;
            }
            return Dic;
        }
        //private string StringToCoin(string StringNameButton)
        //{
        //    string text = "";
        //    switch (StringNameButton)
        //    {
        //        case "1 руб.":text="One";
        //            break;
        //        case "2 руб.":
        //            text = "Two";
        //            break;
        //        case "5 руб.":
        //            text = "Five";
        //            break;
        //        case "10 руб.":
        //            text = "Ten";
        //            break;
        //    }
        //    return (text);
        //}
        private void SaveCoinInBase(string NameNumberCoin)
        {
            Coin coin = repository.Coins.FirstOrDefault(d => d.SNameNumberCoin == NameNumberCoin);
            coin.iCountCoin ++;
            repository.SaveCoin(coin);
        }
        private void SaveCoinInBase(string NameCoin,int ValueCountCoin)
        {
            Coin coin = repository.Coins.FirstOrDefault(d => d.SNameCoin == NameCoin);
            coin.iCountCoin += ValueCountCoin;
            repository.SaveCoin(coin);
        }
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.SumMoney = int.Parse("0");
            ViewBag.RestOfMoney = 0;
            DefaultViewBag();
            if ( Request.QueryString["id"] == "secret")
                return Redirect(Url.Action("Index", "Admin")); 
            else return View(repository.Drinks);
        }
        [HttpPost]
        public ActionResult Index(string SumMoney, string clickonbutton,string clickbuttoncoin,string buttonrestofmoney)
        {
            ViewBag.Title = buttonrestofmoney;
            //(HttpContext.Application["LicenseFile"] as string);
            int iSumInController = 0;
            int.TryParse(SumMoney, out iSumInController);
            //Sum += k;
            ViewBag.RestOfMoney = 0;
            ViewBag.SumMoney = int.Parse(SumMoney);
            DefaultViewBag();
            if (!string.IsNullOrEmpty(clickbuttoncoin)) SaveCoinInBase(clickbuttoncoin.Split(' ')[0]);
            if (!string.IsNullOrEmpty(clickonbutton))
            {
                Drink Drink = repository.Drinks.FirstOrDefault(d => d.Name == clickonbutton);
                Drink.iCount--;
                repository.SaveProduct(Drink);
                clickonbutton = "";
                clickbuttoncoin = "";
                ViewBag.RestOfMoney = iSumInController - (int)Drink.Price;
                
                ViewBag.SumMoney = ViewBag.RestOfMoney;
            }
            if (!string.IsNullOrEmpty(buttonrestofmoney))
            {
                foreach (KeyValuePair<int, int> item in CalculateChange((int.Parse(buttonrestofmoney.Split(' ')[0]))))
                {
                    switch (item.Key)
                    {
                        case 1:
                            SaveCoinInBase("One", -item.Value);
                            break;
                        case 2:
                            SaveCoinInBase("Two", -item.Value);
                            break;
                        case 5:
                            SaveCoinInBase("Five", -item.Value);
                            break;
                        case 10:
                            SaveCoinInBase("Ten", -item.Value);
                            break;
                    }
                }
                ViewBag.SumMoney = 0;
            }
            
            return View(repository.Drinks);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View(repository.Drinks.FirstOrDefault(d => d.ProductID == 1));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public FileContentResult GetImage(int productId)
        {
            Drink prod = repository.Drinks.FirstOrDefault(p => p.ProductID == productId);
            if (prod != null)
            {
                return File(prod.ImageData, prod.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}
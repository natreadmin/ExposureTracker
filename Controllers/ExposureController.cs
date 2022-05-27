using Microsoft.AspNetCore.Mvc;
using ExposureTracker.Data;
using ExposureTracker.Models;

namespace ExposureTracker.Controllers
{
    public class ExposureController :Controller
    {
        private readonly AppDbContext _db;
        IEnumerable<Insured> objInsuredList { get; set; }


        public ExposureController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Search(string pKey)
        {
            //if(!string.IsNullOrEmpty(pKey))
            //{
            //    //objInsuredList = (from x in _db.dbLifeData where x.FirstName.ToUpper().Contains(pKey.ToUpper()) || (x.LastName.ToUpper().Contains(pKey.ToUpper()) || (x.CedingCompany.ToUpper().Contains(pKey.ToUpper()))) select x).ToList();
            //     objInsuredList = (from x in _db.dbLifeData where x.PolicyNumber.ToUpper().Contains(pKey.ToUpper()) || (x.LastName.ToUpper().Contains(pKey.ToUpper()) || (x.CedingCompany.ToUpper().Contains(pKey.ToUpper()))) select x).ToList();
            //}
            //else
            //{
            //    objInsuredList = _db.dbLifeData;
            //}
            return View(/*objInsuredList*/);
        }
    }
}

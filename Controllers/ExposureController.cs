using Microsoft.AspNetCore.Mvc;
using ExposureTracker.Data;
using ExposureTracker.Models;
using System.Net.Http.Headers;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;

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
        public IActionResult Index(string searchKey)
        {
            if(!string.IsNullOrEmpty(searchKey))
            {
                objInsuredList = (from x in _db.dbLifeData where x.FirstName.ToUpper().Contains(searchKey.ToUpper()) || (x.LastName.ToUpper().Contains(searchKey.ToUpper()) || (x.CedingCompany.ToUpper().Contains(searchKey.ToUpper()))) select x).ToList();
                //     objInsuredList = (from x in _db.dbLifeData where x.PolicyNumber.ToUpper().Contains(pKey.ToUpper()) || (x.LastName.ToUpper().Contains(pKey.ToUpper()) || (x.CedingCompany.ToUpper().Contains(pKey.ToUpper()))) select x).ToList();
            }
            else
            {
                objInsuredList = _db.dbLifeData;
            }
            return View(objInsuredList);
        }


        public IActionResult Upload()
        {
            ViewBag.Message = "Upload File";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcelFile(IFormFile ExcelFile)
        {
            try
            {
                ViewBag.Message = "";

                if(ExcelFile != null)
                {

                    var list = new List<Insured>();

                    using(var stream = new MemoryStream())
                    {
                        await ExcelFile.CopyToAsync(stream);
                        using(var package = new ExcelPackage(stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets [0];
                            var rowcount = worksheet.Dimension.Rows;
                            for(int row = 2; row <= rowcount; row++)
                            {

                                list.Add(new Insured
                                {

                                    Identifier = worksheet.Cells [row, 1].Value.ToString().ToLower().Trim(),
                                    PolicyNumber = worksheet.Cells [row, 2].Value.ToString().Trim(),
                                    FirstName = worksheet.Cells [row, 3].Value.ToString().Trim(),
                                    MiddleName = worksheet.Cells [row, 4].Value.ToString().Trim(),
                                    LastName = worksheet.Cells [row, 5].Value.ToString().Trim(),
                                    FullNameDOB = worksheet.Cells [row, 6].Value.ToString().Trim(),
                                    Gender = worksheet.Cells [row, 7].Value.ToString().Trim(),
                                    ClientID = worksheet.Cells [row, 8].Value.ToString().Trim(),
                                    DateofBirth = Convert.ToDateTime(worksheet.Cells [row, 9].Value).ToString("MM-dd-yyyy"),
                                    CedingCompany = worksheet.Cells [row, 10].Value.ToString().Trim(),
                                    CedantCode = worksheet.Cells [row, 11].Value.ToString().Trim(),
                                    TypeOfBusiness = worksheet.Cells [row, 12].Value.ToString().Trim(),
                                    Filename = worksheet.Cells [row, 13].Value.ToString().Trim(),
                                    Certificate = worksheet.Cells [row, 14].Value.ToString().Trim(),
                                    Plan = worksheet.Cells [row, 15].Value.ToString().Trim(),
                                    Currency = worksheet.Cells [row, 16].Value.ToString().Trim(),
                                    BenefitType = worksheet.Cells [row, 17].Value.ToString().Trim(),
                                    PlanEffectiveDate = Convert.ToDateTime(worksheet.Cells [row, 18].Value).ToString("MM-dd-yyyy"),
                                    SumAssured = Convert.ToDecimal(worksheet.Cells [row, 19].Value),
                                    ReinsuredNetAmountAtRisk = Convert.ToDecimal(worksheet.Cells [row, 20].Value),
                                    ReinsuredNetAmountAtRiskPlan = Convert.ToDecimal(worksheet.Cells [row, 21].Value),
                                    ReinsuredNetAmountAtRiskRiders = Convert.ToDecimal(worksheet.Cells [row, 22].Value),
                                    BordereauxYear = Convert.ToInt32(worksheet.Cells [row, 23].Value),
                                    SubstandardRatingPlan = worksheet.Cells [row, 24].Value.ToString().Trim(),
                                    SubstandardRatingRiders = worksheet.Cells [row, 25].Value.ToString().Trim(),
                                    RetrocededNarPlan = Convert.ToInt32(worksheet.Cells [row, 26].Value),
                                    RetrocededNarRider = Convert.ToInt32(worksheet.Cells [row, 27].Value),
                                    Status = worksheet.Cells [row, 28].Value.ToString().Trim(),
                                }); ;

                            }
                        }
                        foreach(var item in list)
                        {
                            if(ModelState.IsValid)
                            {
                                //check if record exist in the database
                                var query = from obj in _db.dbLifeData
                                            where obj.Identifier == item.Identifier.ToLower() && obj.PolicyNumber == item.PolicyNumber
                                            select obj;

                                
                                if(query.Count() > 0)  //with record
                                {
                                    var query2 = from obj in _db.dbLifeData
                                                 where obj.Identifier == item.Identifier.ToLower() && obj.PolicyNumber == item.PolicyNumber && obj.BordereauxYear >= item.BordereauxYear 
                                                 select obj; 

                                    if(query2.Count() > 0) //bordereau year in raw file is greater than bordereau year in database 
                                    {

                                        _db.dbLifeData.UpdateRange(item);
                                        _db.SaveChanges();
                                    }
                                    
                                }
                                else //if no record
                                {
                                    _db.dbLifeData.Add(item);
                                    _db.SaveChanges();
                                }
                                

                            }

                        }

                    }
                    //if the code reach here means everthing goes fine and excel data is imported into database
                    ViewBag.Message = "Data uploaded successfully ";
                    return View("Upload");
                }
                else
                {
                    ViewBag.Message = "Upload Failed";
                    return View("Upload");
                }

            }
            catch(Exception ex)
            {
                ViewBag.Message = "Upload Failed";
                return View("Upload");
            }


        }


        public IActionResult Details(string Id)
        {
            objInsuredList = (from obj in _db.dbLifeData
            where obj.Identifier.Contains(Id) select obj).ToList();

            string strIdentifier = string.Empty;
            string strFName = string.Empty;
            int intPolicyNo = 0;
            intPolicyNo = objInsuredList.Count();
            decimal dclTotalNarBasic = 0;
            decimal dclTotalReinsuredNarBasic = 0;
            decimal dclTotalReinsuredNarAH = 0;
            decimal dclTotalReinsuredNarCI = 0;
            decimal dclTotalReinsuredNarTR = 0;
            decimal dclTotalReinsuredNarA01 = 0;
            decimal dclTotalReinsuredNarC11 = 0;
           
            foreach (var item in objInsuredList)
            {
                strIdentifier = item.Identifier;
                strFName = item.FullNameDOB;
                dclTotalNarBasic += item.SumAssured;
                
            }
            TempData ["Identifier"] = strIdentifier;
           
            ViewBag.FullName = strFName;
            ViewBag.TotalPolicy = intPolicyNo;
            ViewBag.TotalNarBasic = dclTotalNarBasic;
            ViewBag.TotalRNarBasic = dclTotalReinsuredNarBasic;
            ViewBag.TotalRNarAH = dclTotalReinsuredNarAH;
            ViewBag.TotalRNarCI = dclTotalReinsuredNarCI;
            ViewBag.TotalRNarTR = dclTotalReinsuredNarTR;
            ViewBag.TotalRNarA01 = dclTotalReinsuredNarA01;
            ViewBag.TotalRNarC11 = dclTotalReinsuredNarC11;

            return View("ViewDetails");
        }


        public IActionResult ViewDetails()
        {
            
            return View();
        }


        public IActionResult ViewPolicies(string Identifier)
        {
            objInsuredList = (from obj in _db.dbLifeData
                              where obj.Identifier.Contains(Identifier)
                              select obj).ToList();
            return View();
        }
      
        public IActionResult Edit(string identifier)
        {
            return PartialView("Edit");
        }

    }


}

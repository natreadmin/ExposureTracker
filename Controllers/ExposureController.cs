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
                objInsuredList = (from x in _db.dbLifeData where x.firstname.ToUpper().Contains(searchKey.ToUpper()) || (x.lastname.ToUpper().Contains(searchKey.ToUpper()) || (x.policyno.Contains(searchKey.Trim()) ||(x.cedingcompany.ToUpper().Contains(searchKey.ToUpper())))) select x).ToList();
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
                            ExcelWorksheet worksheet = package.Workbook.Worksheets ["Sheet1"];
                            var rowcount = worksheet.Dimension.Rows;
                            for(int row = 2; row <= rowcount; row++)
                            {
                                
                                list.Add(new Insured
                                {
                                    identifier = worksheet.Cells [row, 1].Value.ToString().ToLower().Trim(),
                                    policyno = worksheet.Cells [row, 2].Value.ToString().Trim(),
                                    firstname = worksheet.Cells [row, 3].Value.ToString().Trim(),
                                    middlename = worksheet.Cells [row, 4].Value.ToString().Trim(),
                                    lastname = worksheet.Cells [row, 5].Value.ToString().Trim(),
                                    fullName = worksheet.Cells [row, 6].Value.ToString().Trim(),
                                    gender = worksheet.Cells [row, 7].Value.ToString().Trim(),
                                    clientid = worksheet.Cells [row, 8].Value.ToString().Trim(),
                                    dateofbirth = Convert.ToDateTime(worksheet.Cells [row, 9].Value).ToString("MM-dd-yyyy"),
                                    cedingcompany = worksheet.Cells [row, 10].Value.ToString().Trim(),
                                    cedantcode = worksheet.Cells [row, 11].Value.ToString().Trim(),
                                    typeofbusiness = worksheet.Cells [row, 12].Value.ToString().Trim(),
                                    bordereauxfilename = worksheet.Cells [row, 13].Value.ToString().Trim(),
                                    bordereauxyear = Convert.ToInt32(worksheet.Cells [row, 14].Value),
                                    certificate = worksheet.Cells [row, 15].Value.ToString().Trim(),
                                    plan = worksheet.Cells [row, 16].Value.ToString().Trim(),
                                    benefittype = worksheet.Cells [row, 17].Value.ToString().Trim(),
                                    currency = worksheet.Cells [row, 18].Value.ToString().Trim(),
                                    planeffectivedate = Convert.ToDateTime(worksheet.Cells [row, 19].Value).ToString("MM-dd-yyyy"),
                                    sumassured = Convert.ToDecimal(worksheet.Cells [row, 20].Value),
                                    reinsurednetamountatrisk = Convert.ToDecimal(worksheet.Cells [row, 21].Value),
                                    mortalityrating = worksheet.Cells [row, 22].Value.ToString(),
                                    status = worksheet.Cells [row, 23].Value.ToString(),
                                    
                                }); 

                            }
                        }


                        list.ForEach (x =>{
                            var query = _db.dbLifeData.FirstOrDefault(y => y.identifier == x.identifier && y.policyno == x.policyno && y.plan == x.plan);
                           
                            if (query != null)
                            {
                                if (query.bordereauxyear < x.bordereauxyear)
                                {
                                    query.identifier = x.identifier;
                                    query.policyno = x.policyno;
                                    query.firstname = x.firstname;
                                    query.middlename = x.middlename;
                                    query.lastname = x.lastname;
                                    query.bordereauxyear = x.bordereauxyear;
                                    _db.Entry(query).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                                
                            }
                            else
                            {
                                _db.AddRange(list);
                                _db.SaveChanges();
                            }
                        });
                        
                        


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


        public IActionResult ViewAccumulation(string Id)
        {
            objInsuredList = (from obj in _db.dbLifeData
            where obj.identifier.Contains(Id) select obj).ToList();

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
                strIdentifier = item.identifier;
                strFName = item.fullName;
                dclTotalNarBasic += item.sumassured;
                
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
                              where obj.identifier.Contains(Identifier)
                              select obj).ToList();


            string strFullName = string.Empty;
            string strIdentifier = string.Empty;
         
            foreach(var item in objInsuredList)
            {
                strIdentifier = item.identifier;
                strFullName = item.fullName;
                if (strFullName != string.Empty)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            TempData ["Id"] = strIdentifier;
            ViewBag.FullName = strFullName;


            return View(objInsuredList);
        }
      
        public IActionResult EditSession(int Id)
        {

            var objInsured = _db.dbLifeData.Find(Id);
            string strDOB = Convert.ToDateTime(objInsured.dateofbirth).ToString("MM/dd/yyyy"); //DateofBirth
            string strPED = Convert.ToDateTime(objInsured.planeffectivedate).ToString("MM/dd/yyyy"); //PlanEffectiveDate
            ViewBag.DOB = strDOB;
            ViewBag.PED = strPED;
            return PartialView("_partialViewEdit",objInsured);
        }

    }


}

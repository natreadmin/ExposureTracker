using Microsoft.AspNetCore.Mvc;
using ExposureTracker.Data;
using ExposureTracker.Models;
using System.Net.Http.Headers;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using System.ComponentModel;

namespace ExposureTracker.Controllers
{
    public class ExposureController :Controller
    {
        private readonly AppDbContext _db;
        IEnumerable<Insured> objInsuredList { get; set; }
        IEnumerable<TranslationTables> objTransTableList { get; set; }


        public ExposureController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string searchKey)
        {
            if(!string.IsNullOrEmpty(searchKey))
            {
                objInsuredList = (from x in _db.dbLifeData where x.firstname.ToUpper().Contains(searchKey.ToUpper()) || (x.lastname.ToUpper().Contains(searchKey.ToUpper()) || (x.policyno.Contains(searchKey.Trim()) || (x.cedingcompany.ToUpper().Contains(searchKey.ToUpper())))) select x).ToList();
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
            ViewBag.Message = "Upload a data to database";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile DBTemplate, string selectedDB)
        {
            try
            {
                ViewBag.Message = "";

                if(DBTemplate != null)
                {
                    if(selectedDB == "SICS")
                    {
                        var list = new List<Insured>();
                        using(var stream = new MemoryStream())
                        {
                            await DBTemplate.CopyToAsync(stream);
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
                                        dateofbirth = Convert.ToDateTime(worksheet.Cells [row, 9].Value).ToString("MM/dd/yyyy"),
                                        cedingcompany = worksheet.Cells [row, 10].Value.ToString().Trim(),
                                        cedantcode = worksheet.Cells [row, 11].Value.ToString().Trim(),
                                        typeofbusiness = worksheet.Cells [row, 12].Value.ToString().Trim(),
                                        bordereauxfilename = worksheet.Cells [row, 13].Value.ToString().Trim(),
                                        bordereauxyear = Convert.ToInt32(worksheet.Cells [row, 14].Value),
                                        certificate = worksheet.Cells [row, 15].Value.ToString().Trim(),
                                        plan = worksheet.Cells [row, 16].Value.ToString().Trim(),
                                        benefittype = worksheet.Cells [row, 17].Value.ToString().Trim().ToUpper(),
                                        currency = worksheet.Cells [row, 18].Value.ToString().Trim(),
                                        planeffectivedate = Convert.ToDateTime(worksheet.Cells [row, 19].Value).ToString("MM/dd/yyyy"),
                                        sumassured = Convert.ToDecimal(worksheet.Cells [row, 20].Value),
                                        reinsurednetamountatrisk = Convert.ToDecimal(worksheet.Cells [row, 21].Value),
                                        mortalityrating = worksheet.Cells [row, 22].Value.ToString(),
                                        status = worksheet.Cells [row, 23].Value.ToString(),
                                    });

                                }
                            }

                            list.ForEach(x =>
                            {
                                var query = _db.dbLifeData.FirstOrDefault(y => y.identifier == x.identifier && y.policyno == x.policyno && y.plan == x.plan);

                                if(query != null)
                                {
                                    if(query.bordereauxyear < x.bordereauxyear)
                                    {
                                        query.policyno = x.policyno;
                                        query.firstname = x.firstname;
                                        query.middlename = x.middlename;
                                        query.lastname = x.lastname;
                                        query.fullName = x.fullName;
                                        query.gender = x.gender;
                                        query.clientid = x.clientid;
                                        query.dateofbirth = x.dateofbirth;
                                        query.cedingcompany = x.cedingcompany;
                                        query.cedantcode = x.cedantcode;
                                        query.typeofbusiness = x.typeofbusiness;
                                        query.bordereauxfilename = x.bordereauxfilename;
                                        query.bordereauxyear = x.bordereauxyear;
                                        query.certificate = x.certificate;
                                        query.plan = x.plan;
                                        query.benefittype = x.benefittype;
                                        query.currency = x.currency;
                                        query.planeffectivedate = x.planeffectivedate;
                                        query.sumassured  = x.sumassured;
                                        query.reinsurednetamountatrisk = x.reinsurednetamountatrisk;
                                        query.mortalityrating = x.mortalityrating;
                                        query.status = x.status;
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
                    }
                    //if the code reach here means everthing goes fine and excel data is imported into database

                    else if(selectedDB == "TranslationTable")
                    {

                        var list = new List<TranslationTables>();
                        using(var stream = new MemoryStream())
                        {
                            await DBTemplate.CopyToAsync(stream);
                            using(var package = new ExcelPackage(stream))
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets ["Sheet1"];
                                var rowcount = worksheet.Dimension.Rows;
                                for(int row = 2; row <= rowcount; row++)
                                {

                                    list.Add(new TranslationTables
                                    {
                                        ceding_company = worksheet.Cells [row, 1].Value.ToString().ToLower().Trim(),
                                        plan_code = worksheet.Cells [row, 2].Value.ToString().Trim(),
                                        benefit_cover = worksheet.Cells [row, 3].Value.ToString().Trim(),
                                        insured_prod = worksheet.Cells [row, 4].Value.ToString().Trim(),
                                        prod_description = worksheet.Cells [row, 5].Value.ToString().Trim(),
                                        base_rider = worksheet.Cells [row, 6].Value.ToString().Trim()
                                    });

                                }
                            }

                            list.ForEach(x =>
                            {
                                var query = _db.dbTranslationTable.FirstOrDefault(y => y.plan_code == x.plan_code && y.ceding_company == x.ceding_company);

                                if(query != null)
                                {
                                    if(query.plan_code == x.plan_code && query.ceding_company == x.ceding_company)
                                    {
                                        query.ceding_company = x.ceding_company;
                                        query.plan_code = x.plan_code;
                                        query.benefit_cover = x.benefit_cover;
                                        query.insured_prod = x.insured_prod;
                                        query.prod_description = x.prod_description;
                                        query.base_rider = x.base_rider;
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
                    }
                    else
                    {
                        ViewBag.Message = "No database was selected";
                        return View("Upload");
                    }
                    ViewBag.Message = "Data successfully upload to database!";
                    return View("Upload");
                }

                else
                {
                    ViewBag.Message = "Select a file to upload";
                    return View("Upload");
                }

            }
            catch(Exception ex)
            {
                ViewBag.Message = "Upload Failed";
                return View("Upload");
            }

        }


        public IActionResult ViewAccumulation(string Identifier)
            {

            var results = _db.dbLifeData.Where(y => y.identifier == Identifier);
            var ridersList = new List<Insured>();
            string strIdentifier = string.Empty;    
            string strFName = string.Empty;
            string strDob = string.Empty;
            int intPolicyNo = 0;
            intPolicyNo = results.Count();
            decimal dclBasicTotalSumReinsured = 0;
            decimal dclBasicReinsuredAmount = 0;
            decimal dclRiderReinsuredAmount = 0;
            string strPolicyNo = string.Empty; 

            foreach(var item in results)
            {
                Console.WriteLine(item.benefittype.ToString());
                if(item.benefittype.Trim().Contains("RIDER")) //For Update Tommorrow
                {
                    strIdentifier = item.identifier;
                    strPolicyNo = item.policyno;
                    strFName = item.fullName;
                    strDob = item.dateofbirth.Replace("-", "/");
                    ridersList.Add(item);
                    objInsuredList = ridersList;
                }
                else
                {
                    strIdentifier = item.identifier;
                    strPolicyNo = item.policyno;
                    strFName = item.fullName;
                    strDob = item.dateofbirth.Replace("-", "/");
                    dclRiderReinsuredAmount += item.reinsurednetamountatrisk;
                }
            }
            ViewBag.Policy = strPolicyNo;
            ViewBag.Identifier = strIdentifier;
            ViewBag.FullName = strFName;
            ViewBag.TotalPolicy = intPolicyNo;
            ViewBag.DateofBirth = strDob;
            ViewBag.TotalBasicSumReinsured = dclBasicTotalSumReinsured;
            ViewBag.TotalBasicReinsuredAmount = dclBasicReinsuredAmount;
            ViewBag.TotalRiderNetAmount = dclRiderReinsuredAmount;

            return View("ViewDetails", objInsuredList);
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
                if(strFullName != string.Empty)
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
            return PartialView("_partialViewEdit", objInsured);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Insured objInsuredList)
        {

            _db.dbLifeData.Update(objInsuredList);
            _db.SaveChanges();
            return RedirectToAction("Index");



        }

    }


}

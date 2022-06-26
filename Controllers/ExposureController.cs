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
            }
            else
            {
                objInsuredList = _db.dbLifeData;
            }
            return View(objInsuredList);
        }

        public IActionResult Upload()
        {
            ViewBag.Message = "UPLOAD DATA TO DATABASE";
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
                                        firstname = Convert.ToString(worksheet.Cells [row, 3].Value).Trim(),
                                        middlename = Convert.ToString(worksheet.Cells [row, 4].Value).Trim(),
                                        lastname = Convert.ToString(worksheet.Cells [row, 5].Value).Trim(),
                                        fullName = Convert.ToString(worksheet.Cells [row, 6].Value).Trim(),
                                        gender = Convert.ToString(worksheet.Cells [row, 7].Value).Trim(),
                                        clientid = Convert.ToString(worksheet.Cells [row, 8].Value).Trim(),
                                        dateofbirth = Convert.ToDateTime(worksheet.Cells [row, 9].Value).ToString("MM/dd/yyyy"),
                                        cedingcompany = Convert.ToString(worksheet.Cells [row, 10].Value).Trim(),
                                        cedantcode = Convert.ToString(worksheet.Cells [row, 11].Value).Trim(),
                                        typeofbusiness = Convert.ToString(worksheet.Cells [row, 12].Value).Trim(),
                                        bordereauxfilename = Convert.ToString(worksheet.Cells [row, 13].Value).Trim(),
                                        bordereauxyear = Convert.ToInt32(worksheet.Cells [row, 14].Value),
                                        certificate = Convert.ToString(worksheet.Cells [row, 15].Value).Trim(),
                                        plan = Convert.ToString(worksheet.Cells [row, 16].Value).Trim().ToUpper(),
                                        benefittype = Convert.ToString(worksheet.Cells [row, 17].Value).Trim().ToUpper(),
                                        baserider = Convert.ToString(worksheet.Cells [row, 18].Value).Trim().ToUpper(),
                                        currency = Convert.ToString(worksheet.Cells [row, 19].Value).Trim(),
                                        planeffectivedate = Convert.ToDateTime(worksheet.Cells [row, 20].Value).ToString("MM/dd/yyyy"),
                                        sumassured = Convert.ToDecimal(worksheet.Cells [row, 21].Value),
                                        reinsurednetamountatrisk = Convert.ToDecimal(worksheet.Cells [row, 22].Value),
                                        mortalityrating = Convert.ToString(worksheet.Cells [row, 23].Value),
                                        status = Convert.ToString(worksheet.Cells [row, 24].Value),
                                        dateuploaded = Convert.ToDateTime(worksheet.Cells [row, 25].Value).ToString("MM/dd/yyyy"),
                                    });

                                }
                            }
                          
                            list.ForEach(x =>
                            {
                                string strTransInsuranceProd = string.Empty;
                                objInsuredList = (from obj in _db.dbLifeData
                                                  where (obj.identifier == x.identifier) && (obj.plan == x.plan) && (obj.policyno == x.policyno)
                                                  select obj).ToList();//get all existing record
                                var query = _db.dbLifeData.FirstOrDefault(y => y.identifier == x.identifier && y.policyno == x.policyno && y.plan == x.plan); //check current row in list if it exists
                                var queryTransTable = _db.dbTranslationTable.FirstOrDefault(y => y.plan_code == x.plan); //get the benefit type based on the plan code as reference
                                if(query != null)
                                {
                                    
                                    if(objInsuredList.Count() > 0)
                                    {
                                        if(query.bordereauxyear < x.bordereauxyear && query.identifier == x.identifier && query.policyno == x.policyno && query.cedingcompany == x.cedingcompany && query.plan == x.plan)
                                        {
                                            
                                            if(!string.IsNullOrEmpty(query.benefittype))
                                            {
                                                query.identifier = x.identifier;
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
                                                query.baserider = fn_getBaseRider(queryTransTable.insured_prod);
                                                query.currency = x.currency;
                                                query.planeffectivedate = x.planeffectivedate;
                                                query.sumassured = x.sumassured;
                                                query.reinsurednetamountatrisk = x.reinsurednetamountatrisk;
                                                query.mortalityrating = x.mortalityrating;
                                                query.status = x.status;
                                                _db.Entry(query).State = EntityState.Modified;
                                            }
                                            else //benefit type column is empty in file upload
                                            {
                                                query.identifier = x.identifier;
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
                                                query.benefittype = queryTransTable.prod_description;// add prod description
                                                query.baserider = fn_getBaseRider(queryTransTable.insured_prod);
                                                query.currency = x.currency;
                                                query.planeffectivedate = x.planeffectivedate;
                                                query.sumassured = x.sumassured;
                                                query.reinsurednetamountatrisk = x.reinsurednetamountatrisk;
                                                query.mortalityrating = x.mortalityrating;
                                                query.status = x.status;
                                                _db.Entry(query).State = EntityState.Modified;
                                            }
                                           
                                        }// if bordereau year is less than the existing year in the database do nothhing
                                        #region exclude this logic for now
                                        //else if(x.benefittype == string.Empty || x.cedantcode == string.Empty)
                                        //{
                                        //    query.identifier = x.identifier;
                                        //    query.policyno = x.policyno;
                                        //    query.firstname = x.firstname;
                                        //    query.middlename = x.middlename;
                                        //    query.lastname = x.lastname;
                                        //    query.fullName = x.fullName;
                                        //    query.gender = x.gender;
                                        //    query.clientid = x.clientid;
                                        //    query.dateofbirth = x.dateofbirth;
                                        //    query.cedingcompany = x.cedingcompany;
                                        //    query.cedantcode = queryTransTable.cedant_code;
                                        //    query.typeofbusiness = x.typeofbusiness;
                                        //    query.bordereauxfilename = x.bordereauxfilename;
                                        //    query.bordereauxyear = x.bordereauxyear;
                                        //    query.certificate = x.certificate;
                                        //    query.plan = x.plan;
                                        //    query.benefittype = queryTransTable.prod_description;// add prod description
                                        //    query.baserider = fn_getBaseRider(queryTransTable.insured_prod);
                                        //    query.currency = x.currency;
                                        //    query.planeffectivedate = x.planeffectivedate;
                                        //    query.sumassured = x.sumassured;
                                        //    query.reinsurednetamountatrisk = x.reinsurednetamountatrisk;
                                        //    query.mortalityrating = x.mortalityrating;
                                        //    query.status = x.status;
                                        //    _db.Entry(query).State = EntityState.Modified;
                                        //}
                                        #endregion

                                    }

                                }
                                else //current row in excel dont have record yet
                                {
                                    //var queryTransTable = _db.dbTranslationTable.FirstOrDefault(y => y.plan_code == x.plan); //get the benefit type based on the plan code as reference
                                    var list_ = new List<Insured>();
                             
                                   
                            
                                    if (query == null)
                                        {
                                            if(x.benefittype == string.Empty ||x.cedantcode == string.Empty)
                                            {
                                                x.benefittype = queryTransTable.prod_description;
                                                x.cedantcode = queryTransTable.cedant_code;
                                                x.baserider = fn_getBaseRider(queryTransTable.insured_prod);
                                                list_.Add(x);
                                            }
                                           
                                        }
                                    _db.AddRange(list_);
                                }
                            });
                        }
                        _db.SaveChanges();
                    }
        
                    //if the code reach here means everthing goes fine and excel data is imported into database

                    else if(selectedDB == "TRANSLATION TABLE")
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
                                        ceding_company = Convert.ToString(worksheet.Cells [row, 1].Value).Trim().ToUpper(),
                                        cedant_code = Convert.ToString(worksheet.Cells [row, 2].Value).Trim().ToUpper(),
                                        plan_code = Convert.ToString(worksheet.Cells [row, 3].Value).Trim().ToUpper(),
                                        benefit_cover = Convert.ToString(worksheet.Cells [row, 4].Value).Trim().ToUpper(),
                                        insured_prod = Convert.ToString(worksheet.Cells [row, 5].Value).Trim().ToUpper(),
                                        prod_description = Convert.ToString(worksheet.Cells [row, 6].Value).Trim().ToUpper(),
                                        base_rider = Convert.ToString(worksheet.Cells [row, 7].Value).Trim().ToUpper()
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
                                    }
                                }
                                else
                                {
                                    if(string.IsNullOrEmpty(x.base_rider))
                                    {
                                        x.base_rider = fn_getBaseRider(x.insured_prod);
                                    }
                                    _db.AddRange(list);
                                }
                            });
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "SELECT A DATABASE";
                        return View("Upload");
                    }
                    ViewBag.Message = "Upload to " + selectedDB.ToUpper() + " database completed!";
                    return View("Upload");
                }

                else
                {
                    ViewBag.Message = "UPLOAD A FILE AND SELECT A DATABASE";
                    return View("Upload");
                }

            }
            catch(Exception ex)
            {
                ViewBag.Message = "UPLOAD FAILED";
                return View("Upload");
            }

        }


        public string fn_getBaseRider(string valueInsuranceProd)
        {
            string [] InsuranceProd = { "VARIABLELIFE-RE", "TRADITIONAL LIFE", "TERMLIFE-GRP", "CREDITLIFE"};

            foreach (var item in InsuranceProd)
            {
                if (item.ToUpper() == valueInsuranceProd.ToUpper())
                {
                    return "BASE";
                    break;
                  
                }
                else
                {
                    continue;
                    
                }
            }
            return "RIDER";
          
        }

        public IActionResult ViewAccumulation(string Identifier)
        {
            var list = new List<Insured>();
            var results = _db.dbLifeData.Where(y => y.identifier == Identifier);
            list = results.ToList();
            string strFullName = string.Empty;
            string strDob = string.Empty;
            int intPolicyNo = 0;
            intPolicyNo = list.Count();
            decimal dclBasicTotalSumReinsured = 0;
            decimal dclBasicTotalReinsuredAmount = 0;
            decimal dclRiderTotalReinsuredAmount = 0;

            foreach(var item in list)
            {
                strFullName = item.fullName;
                strDob = item.dateofbirth;
                if (item.baserider == "BASE")
                {
                    dclBasicTotalSumReinsured += item.sumassured;
                    dclBasicTotalReinsuredAmount += item.reinsurednetamountatrisk;
                }
                else
                {
                    dclRiderTotalReinsuredAmount += item.reinsurednetamountatrisk;
                }
            }

            ViewBag.Identifier = Identifier;
            ViewBag.FullName = strFullName;
            ViewBag.TotalPolicy = intPolicyNo;
            ViewBag.DateofBirth = strDob;
            ViewBag.TotalBasicSumReinsured = dclBasicTotalSumReinsured;
            ViewBag.TotalBasicReinsuredAmount = dclBasicTotalReinsuredAmount;
            ViewBag.TotalRiderNetAmount = dclRiderTotalReinsuredAmount;

            return View("ViewDetails", list);
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
            TempData["Identifier"] = Identifier;
            ViewBag.FullName = strFullName;
            return View(objInsuredList);
        }

        public IActionResult EditSession(int Id)
        {
            var objInsured = _db.dbLifeData.Find(Id);
            objInsured.dateuploaded = DateTime.Now.ToString("MM/dd/yyy");
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

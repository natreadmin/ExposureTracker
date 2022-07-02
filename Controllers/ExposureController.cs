using Microsoft.AspNetCore.Mvc;
using ExposureTracker.Data;
using ExposureTracker.Models;
using System.Net.Http.Headers;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;

namespace ExposureTracker.Controllers
{
    public class ExposureController :Controller
    {
        private readonly AppDbContext _db;
        IEnumerable<Insured> objInsuredList { get; set; }
        IEnumerable<TranslationTables> objTransTableList { get; set; }

        IEnumerable<Basic> objAccumulation { get; set; }

        public ExposureController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string searchKey)
        {
            if(!string.IsNullOrEmpty(searchKey))
            {
                objInsuredList = (from x in _db.dbLifeData where x.firstname.ToUpper().Contains(searchKey.ToUpper()) || (x.lastname.ToUpper().Contains(searchKey.ToUpper()) || (x.policyno.Contains(searchKey.Trim()) || (x.baserider.Contains(searchKey.Trim()) || (x.cedingcompany.ToUpper().Contains(searchKey.ToUpper()))))) select x).ToList();
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
                string userName = Environment.UserName;
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
                                        soaperiod = Convert.ToString(worksheet.Cells [row, 15].Value),
                                        certificate = Convert.ToString(worksheet.Cells [row, 16].Value).Trim(),
                                        plan = Convert.ToString(worksheet.Cells [row, 17].Value).Trim().ToUpper(),
                                        benefittype = Convert.ToString(worksheet.Cells [row, 18].Value).Trim().ToUpper(),
                                        baserider = Convert.ToString(worksheet.Cells [row, 19].Value).Trim().ToUpper(),
                                        currency = Convert.ToString(worksheet.Cells [row, 20].Value).Trim(),
                                        planeffectivedate = Convert.ToDateTime(worksheet.Cells [row, 21].Value).ToString("MM/dd/yyyy"),
                                        sumassured = Convert.ToDecimal(worksheet.Cells [row, 22].Value),
                                        reinsurednetamountatrisk = Convert.ToDecimal(worksheet.Cells [row, 23].Value),
                                        mortalityrating = Convert.ToString(worksheet.Cells [row, 24].Value),
                                        status = Convert.ToString(worksheet.Cells [row, 25].Value),
                                        dateuploaded = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"),
                                        uploadedby = userName,
                                    });

                                }
                            }

                            list.ForEach(x =>
                            {
                                string strTransInsuranceProd = string.Empty;

                                var query = _db.dbLifeData.FirstOrDefault(y => y.identifier == x.identifier && y.policyno == x.policyno && y.plan == x.plan); //check current row in list if it exists
                                var queryTransTable = _db.dbTranslationTable.FirstOrDefault(y => y.plan_code == x.plan); //get the benefit type based on the identifier from translation table as reference


                                if(query != null)
                                {
                                    int listQuarter = fn_getQuarter(x.soaperiod);
                                    int queryQuarter = fn_getQuarter(query.soaperiod);
                                    //if(objInsuredList.Count() > 0)
                                    //{
                                    if(query.bordereauxyear <= x.bordereauxyear && listQuarter <= queryQuarter && query.identifier == x.identifier && query.policyno == x.policyno && query.cedingcompany == x.cedingcompany && query.plan == x.plan)
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
                                            query.soaperiod = x.soaperiod;
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
                                        else //benefit type column null in excel
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
                                            query.soaperiod = x.soaperiod;
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
                                    else if(query.bordereauxyear <= x.bordereauxyear && listQuarter >= queryQuarter)
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
                                        query.cedantcode = queryTransTable.cedant_code;
                                        query.typeofbusiness = x.typeofbusiness;
                                        query.bordereauxfilename = x.bordereauxfilename;
                                        query.bordereauxyear = x.bordereauxyear;
                                        query.soaperiod = x.soaperiod;
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
                                    #endregion

                                    //}

                                }
                                else //current row in excel dont have record yet
                                {
                                    //var queryTransTable = _db.dbTranslationTable.FirstOrDefault(y => y.plan_code == x.plan); //get the benefit type based on the plan code as reference
                                    var newInsured = new Insured();
                                    if(query == null)
                                    {
                                        if(x.benefittype == string.Empty || x.cedantcode == string.Empty)
                                        {
                                            newInsured.identifier = x.identifier;
                                            newInsured.policyno = x.policyno;
                                            newInsured.firstname = x.firstname;
                                            newInsured.middlename = x.middlename;
                                            newInsured.lastname = x.lastname;
                                            newInsured.fullName = x.fullName;
                                            newInsured.gender = x.gender;
                                            newInsured.clientid = x.clientid;
                                            newInsured.dateofbirth = x.dateofbirth;
                                            newInsured.cedingcompany = x.cedingcompany;
                                            newInsured.cedantcode = queryTransTable.cedant_code;
                                            newInsured.typeofbusiness = x.typeofbusiness;
                                            newInsured.bordereauxfilename = x.bordereauxfilename;
                                            newInsured.bordereauxyear = x.bordereauxyear;
                                            newInsured.soaperiod = x.soaperiod;
                                            newInsured.certificate = x.certificate;
                                            newInsured.plan = queryTransTable.plan_code;
                                            newInsured.benefittype = queryTransTable.insured_prod;
                                            newInsured.baserider = fn_getBaseRider(queryTransTable.insured_prod);
                                            newInsured.currency = x.currency;
                                            newInsured.planeffectivedate = x.planeffectivedate;
                                            newInsured.sumassured = x.sumassured;
                                            newInsured.reinsurednetamountatrisk = x.reinsurednetamountatrisk;
                                            newInsured.mortalityrating = x.mortalityrating;
                                            newInsured.status = x.status;
                                        }

                                    }
                                    _db.dbLifeData.Add(newInsured);
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
                                        identifier = Convert.ToString(worksheet.Cells [row, 1].Value).Trim().ToUpper(),
                                        ceding_company = Convert.ToString(worksheet.Cells [row, 2].Value).Trim().ToUpper(),
                                        cedant_code = Convert.ToString(worksheet.Cells [row, 3].Value).Trim().ToUpper(),
                                        plan_code = Convert.ToString(worksheet.Cells [row, 4].Value).Trim().ToUpper(),
                                        benefit_cover = Convert.ToString(worksheet.Cells [row, 5].Value).Trim().ToUpper(),
                                        insured_prod = Convert.ToString(worksheet.Cells [row, 6].Value).Trim().ToUpper(),
                                        prod_description = Convert.ToString(worksheet.Cells [row, 7].Value).Trim().ToUpper(),
                                    });

                                }
                            }


                            foreach(var x in list)
                            {
                                var query = _db.dbTranslationTable.FirstOrDefault(y => y.identifier == x.identifier && y.ceding_company == x.ceding_company);
                                if(query != null)
                                {
                                    query.identifier = x.identifier;
                                    query.ceding_company = x.ceding_company;
                                    query.plan_code = x.plan_code;
                                    query.cedant_code = x.cedant_code;
                                    query.benefit_cover = x.benefit_cover;
                                    query.insured_prod = x.insured_prod;
                                    query.prod_description = x.prod_description;
                                    query.base_rider = fn_getBaseRider(x.insured_prod);
                                    _db.Entry(query).State = EntityState.Modified;

                                }
                                else
                                {

                                    var newRecord = new TranslationTables();
                                    if(string.IsNullOrEmpty(x.base_rider))
                                    {
                                        newRecord.identifier = x.identifier;
                                        newRecord.plan_code = x.plan_code;
                                        newRecord.base_rider = fn_getBaseRider(x.insured_prod);
                                        newRecord.insured_prod = x.insured_prod;
                                        newRecord.ceding_company = x.ceding_company;
                                        newRecord.prod_description = x.prod_description;
                                        newRecord.benefit_cover = x.benefit_cover;
                                        newRecord.cedant_code = x.cedant_code;
                                    }
                                    _db.dbTranslationTable.Add(newRecord);
                                    //_db.AddRange(listTable);
                                }

                            }
                            _db.SaveChanges();
                        }
                    }

                    #region BasicRider uploading
                    //else if(selectedDB == "BasicRider")
                    //{

                    //    var list = new List<BasicRiderProd>();
                    //    using(var stream = new MemoryStream())
                    //    {
                    //        await DBTemplate.CopyToAsync(stream);
                    //        using(var package = new ExcelPackage(stream))
                    //        {
                    //            ExcelWorksheet worksheet = package.Workbook.Worksheets ["Sheet1"];
                    //            var rowcount = worksheet.Dimension.Rows;
                    //            for(int row = 2; row <= rowcount; row++)
                    //            {

                    //                list.Add(new BasicRiderProd
                    //                {
                    //                    insuredprod_basic = Convert.ToString(worksheet.Cells [row, 1].Value).Trim().ToUpper(),
                    //                    insuredprod_rider = Convert.ToString(worksheet.Cells [row, 2].Value).Trim().ToUpper(),

                    //                });

                    //            }
                    //        }


                    //        foreach(var x in list)
                    //        {
                    //            var query = _db.dbBasicRider.FirstOrDefault(y => y.insuredprod_basic == x.insuredprod_basic);
                    //            if(query != null)
                    //            {
                    //                query.insuredprod_basic = x.insuredprod_basic;
                    //                query.insuredprod_rider = x.insuredprod_rider;
                    //                _db.Entry(query).State = EntityState.Modified;

                    //            }
                    //            else
                    //            {

                    //                 var newRecord = new BasicRiderProd();
                    //                 newRecord.insuredprod_basic = x.insuredprod_basic;
                    //                 newRecord.insuredprod_rider = x.insuredprod_rider;
                    //                _db.dbBasicRider.Add(newRecord);
                    //                //_db.AddRange(listTable);
                    //            }

                    //        }
                    //        _db.SaveChanges();
                    //    }
                    //}
                    #endregion
                    else
                    {
                        ViewBag.Message = "SELECT A DATABASE";
                        return View("Upload");
                    }
                    ViewBag.Message = selectedDB.ToUpper() + " has been Uploaded to Database";
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
                ViewBag.Message = ex;
                return View("Upload");
            }

        }



        public IActionResult ViewDetails(string Identifier)
        {

            //var ListPolicies = new Accumulation();
            var ListBasic = new List<Basic>()
            {
                new Basic{}
            };

            var ListRiderADB= new List<Rider_ADB>()
            {
                new Rider_ADB{}
            };

            var ListRiderSPLA = new List<Rider_SPLA>()
            {
                new Rider_SPLA{}
            };

            var Account = _db.dbLifeData.Where(y => y.identifier == Identifier);
            var Basic = _db.dbLifeData.Where(y => y.identifier == Identifier && y.baserider == "BASIC");
            var Rider = _db.dbLifeData.Where(y => y.identifier == Identifier && y.baserider == "RIDER");

            int intPolicyNo = Account.Count();
            var userDetails = Account.FirstOrDefault(x => x.identifier == Identifier);
            string strFullname = userDetails.fullName;
            string strDOB = userDetails.dateofbirth;

            foreach(var row in ListBasic)
            {
                
                foreach (var item in Basic)
                {
                    row.basic = "BASIC";
                    row.insuredprod = item.benefittype;
                    row.basictotalsumassured += item.sumassured;
                    row.basictotalNAR += item.reinsurednetamountatrisk;
                    
                }
            }

            foreach(var rider in ListRiderADB.ToList())
            {
                foreach(var item in Rider)
                {
                    if(item.benefittype == "ADB-IND")
                    {
                        rider.rider = "RIDER";
                        rider.insuredprod_adbind = item.benefittype;
                        rider.riderTotalsumassured_adbind += item.sumassured;
                        rider.riderTotal_NAR_adbind += item.reinsurednetamountatrisk;
                    }
                }

                //ListRiderADB.Add(rider);
            }

            foreach(var rider in ListRiderSPLA.ToList())
            {
                foreach(var item in Rider)
                {
                    if(item.benefittype == "SPLADBIND")
                    {
                        rider.rider = "RIDER";
                        rider.insuredprod_spladbind = item.benefittype;
                        rider.riderTotalsumassured_spladbind += item.sumassured;
                        rider.riderTotal_NAR_spladbind += item.reinsurednetamountatrisk;
                    }
                }

                //ListRiderSPLA.Add(rider);
            }
            #region exclude
            //else
            //{

            //    if (item.benefittype == "ADB-IND")
            //    {
            //row.rider = "RIDER";
            //row.insuredprodrider_adbind = item.benefittype;
            //row.riderTotalsumassured_adbind += item.sumassured;
            //row.riderTotal_NAR_adbind += item.reinsurednetamountatrisk;
            //    }
            //    else if(item.benefittype == "SPLADBIND")
            //    {
            //        row.rider = "RIDER";
            //        row.insuredprodrider_spladbind = item.benefittype;
            //        row.riderTotalsumassured_spladbind += item.sumassured;
            //        row.riderTotal_NAR_spladbind += item.reinsurednetamountatrisk;

            //    }

            //}
            #endregion

            ViewData ["BASIC"] = ListBasic.ToList();
            ViewData ["RIDER_ADB"] = ListRiderADB.ToList();
            ViewData ["RIDER_SPLA"] = ListRiderSPLA.ToList();


            ViewBag.Identifier = Identifier;
            ViewBag.FullName = strFullname.ToUpper();
            ViewBag.TotalPolicy = intPolicyNo;
            ViewBag.DateofBirth = strDOB;

            return View("ViewAccumulation");
        }


        public IActionResult ViewAccumulation()
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
                strFullName = item.fullName.ToUpper();
                if(strFullName != string.Empty)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            TempData ["Identifier"] = Identifier;
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



        //public string fn_getInsuranceProd(string valueInsuredProd, string valuePlan)
        //{
        //    var checkTable = _db.dbTranslationTable.Select(y => y.insured_prod == valueInsuredProd && y.plan_code == valuePlan);
        //    if(checkTable.Count() > 0)
        //    {

        //    }


        //}

        public int fn_getQuarter(string value)
        {
            string quarter = string.Empty;
            string quarter_ = string.Empty;
            int quarterNo = 0;
            var number = Regex.Matches(value, @"\d+");
            foreach(var no in number)
            {
                quarter += no;
            }
            quarter_ = quarter;
            quarterNo = int.Parse(quarter_);
            return quarterNo;
        }
        public string fn_getBaseRider(string valueInsuranceProd)
        {
            string [] InsuranceProd = { "VARIABLELIFE-RE", "TRADITIONALLIFE", "TERMLIFE-GRP", "CREDITLIFE" };

            foreach(var item in InsuranceProd)
            {
                if(item.ToUpper() == valueInsuranceProd.ToUpper())
                {
                    return "BASIC";
                    break;

                }
                else
                {
                    continue;

                }
            }
            return "RIDER";

        }

    }

}

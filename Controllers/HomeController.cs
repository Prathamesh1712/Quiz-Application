using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using Rotativa;
using PagedList.Mvc;
using PagedList;
using System.Security.Policy;
using System.Data.Entity;

namespace QuizApp.Controllers
{
    public class HomeController : Controller

    {

        quizdataEntities db =new quizdataEntities();
        public ActionResult Index()

        {
            if (Session["AD_ID"]!=null)
            {
                return RedirectToAction("Dashboard");
            }
                return View();
        }

        [HttpGet]
        public ActionResult SLOGIN()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SLOGIN(TBL_STUDENT s)
        {
            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_NAME == s.S_NAME && x.S_PASSWORD == s.S_PASSWORD).SingleOrDefault();
                if (std == null)
            {
                ViewBag.msg = "Invalid Email or Password";
            }
            else
            {
                Session["std_id"] = std.S_ID;
                  return RedirectToAction("StudentExam");
                
            }

            return View();
        }

        [HttpGet]
        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("SLOGIN");
            }
            else
            {
                List<TBL_CATEGORY> list = db.TBL_CATEGORY.ToList();
                ViewBag.list = new SelectList(list, "CAT_ID", "CAT_NAME");

                return View();
            }
        }
        [HttpPost]
        public ActionResult StudentExam(string room)
        {
            List<TBL_CATEGORY> list = db.TBL_CATEGORY.ToList();
            foreach (var item in list)
            {
                if (string.Equals(item.CAT_NAME, room, StringComparison.CurrentCultureIgnoreCase))
                {
         
                    List<TBL_QUESTIONS> li=db.TBL_QUESTIONS.Where(x =>x.Q_FK_CATID==item.CAT_ID).ToList();

                    Queue<TBL_QUESTIONS> queue=new Queue<TBL_QUESTIONS>();
                    foreach (TBL_QUESTIONS a in li)
                    { 
                    queue.Enqueue(a);
                    }
                    TempData["examid"] = item.CAT_ID;
                    TempData["question"]=queue;
                    TempData["score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Quiz For This Subject...";
                }
            }
            return View();
        }

        public ActionResult QuizStart()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("SLOGIN");
            }

            TBL_QUESTIONS q = null;
            if (TempData["question"] != null)
            {

                Queue<TBL_QUESTIONS> qlist = (Queue<TBL_QUESTIONS>)TempData["question"];
                if (qlist.Count > 0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["question"] = qlist;
                    TempData.Keep();


                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else 
            {
                return RedirectToAction("StudentExam");
            }

            return View(q); 
         
        }
        [HttpPost]
        public ActionResult QuizStart(TBL_QUESTIONS q)
        {
            string correctans=null;
            if (q.OPA != null)
            {
                correctans += "A";
            }
            else if(q.OPB != null)
            {
                correctans += "B";
            }
            else if (q.OPC != null)
            {
                correctans += "C";
            }
            else if (q.OPD != null)
            {
                correctans += "D";
            }
            if (correctans==null)
            {
                ViewBag.error = "Please Select an option...";
                return RedirectToAction("QuizStart");

            }
            else
                if (correctans.Equals(q.COP))
            {
                TempData["score"] = Convert.ToInt32(TempData["score"])+ 1;            
            }
            TempData.Keep();
                return RedirectToAction("QuizStart");

        }
        public ActionResult EndExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("SLOGIN");
            }

            
            TBL_SETEXAM ex = new TBL_SETEXAM();
            ex.EXAM_FK_CATID = Convert.ToInt32(TempData["examid"]);
            ex.EXAM_FK_STU = Convert.ToInt32(Session["std_id"]);
            ex.EXAM_DATE = System.DateTime.Now;

            ViewBag.count = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == ex.EXAM_FK_CATID).Count();

            int total = Convert.ToInt32(TempData["examid"]);
            // ex.EXAM_STD_SCORE = (int)Convert.ToInt32(TempData["score"].ToString()) * 100 / (int)Convert.ToInt32(total);
            ex.EXAM_STD_SCORE = (int)Convert.ToInt32(TempData["score"]);
            db.TBL_SETEXAM.Add(ex);
            db.SaveChanges();
            // Session.RemoveAll();
            //    TempData["score"] = ex.EXAM_STD_SCORE + " % ";

            return View();

        }

        [HttpGet]
        public ActionResult CardLayout(string room)
        {
            var list = db.TBL_CATEGORY.ToList();
            
            List<TBL_CATEGORY> list1 = db.TBL_CATEGORY.ToList();
            foreach (var item in list)
            {
                if (string.Equals(item.CAT_NAME, room, StringComparison.CurrentCultureIgnoreCase))
                {

                    List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == item.CAT_ID).ToList();

                    Queue<TBL_QUESTIONS> queue = new Queue<TBL_QUESTIONS>();
                    foreach (TBL_QUESTIONS a in li)
                    {
                        queue.Enqueue(a);
                    }
                    TempData["examid"] = item.CAT_ID;
                    TempData["question"] = queue;
                    TempData["score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }


            }
            return View(list);

        }
        [HttpPost]
        public ActionResult CardLayout()
        {
           
            return View();
        }


        [HttpGet]
        public ActionResult sregister(int id=0)
        {
            TBL_STUDENT st = new TBL_STUDENT();
            var lastStud = db.TBL_STUDENT.OrderByDescending(c => c.S_ID).FirstOrDefault();
            if (id != 0)
            {
                st = db.TBL_STUDENT.Where(x => x.S_ID == id).FirstOrDefault<TBL_STUDENT>();
            }
            else if (lastStud == null)
            {
                st.S_EMAIL = "STUD 001";
            }
            else
            {
                st.S_EMAIL = "STUD " + (Convert.ToInt32(lastStud.S_EMAIL.Substring(7, lastStud.S_EMAIL.Length - 7)) + 1).ToString("D3");
            }


            return View(st);

        }
        [HttpPost]
        public ActionResult sregister(TBL_STUDENT svw)
        {
            if (!ModelState.IsValid)
            {
                return View(svw);
            }
            try
            {
                TBL_STUDENT s = new TBL_STUDENT();
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_EMAIL = svw.S_EMAIL;
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
                return RedirectToAction("SLOGIN");
            }
            catch (Exception)
            {
                ViewBag.msg = "Email Id Already Exist....";
            }

            return View();
        }

        public ActionResult StudStatus(int? id, int? page)
        {
            if (Session["std_id"] == null)
            {

                return RedirectToAction("SLOGIN");
            }
            id =(int)Session["std_id"];
            ViewBag.id = id.Value;
            if (id == null)
            {
                return RedirectToAction("StudentExam");
            }
       
            var re = db.TBL_SETEXAM.ToList().Where(x=>x.EXAM_FK_STU==id);

            var email = db.TBL_STUDENT.Select(x => new { x.S_ID, x.S_NAME }).FirstOrDefault(x => x.S_ID == id)?.S_NAME;

            ViewBag.Name = email;

            var name  = db.TBL_STUDENT.Select(x => new { x.S_ID, x.S_EMAIL }).FirstOrDefault(x => x.S_ID == id)?.S_EMAIL;

            ViewBag.Email = name;

            return View(re);
        }




            public JsonResult IsEmailExist(String Email)
        {
            return Json(! db.TBL_STUDENT.Any(x => x.S_EMAIL == Email),JsonRequestBehavior.AllowGet);
        
        }




        [HttpGet]
        public ActionResult TLOGIN()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult TLOGIN(TBL_ADMIN a)
        {
            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_NAME == a.AD_NAME && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();
            if(ad!=null)
            {
                Session["AD_ID"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid Username and Password";
            }
            return View();
        }
        public ActionResult Dashboard()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("TLOGIN");
            }

            //  Session["AD_ID"] = 1;
            int adid = Convert.ToInt32(Session["AD_ID"].ToString());
             List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID ==adid).OrderByDescending(x => x.CAT_ID).ToList();
           // List<TBL_CATEGORY> li = db.TBL_CATEGORY.OrderByDescending(x => x.CAT_ID).ToList();
            ViewData["list"] =li;
            
            return View();
        }


        [HttpPost]
        public ActionResult AddCategory(TBL_CATEGORY cat, string name)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ck = db.TBL_CATEGORY.Where(x => x.CAT_NAME == cat.CAT_NAME).FirstOrDefault();
                    if (ck != null)
                    {
                        TempData["mgs"] = "Category Name Already Exist";
                    }
                    else
                    {
                        List<TBL_CATEGORY> li = db.TBL_CATEGORY.OrderByDescending(x => x.CAT_ID).ToList();
                        ViewData["list"] = li;
                        Random rd = new Random();
                        TBL_CATEGORY c = new TBL_CATEGORY();
                        c.CAT_NAME = cat.CAT_NAME;
                      //  c.CAT_ENCERYPT = Crypto.Encrypt(cat.CAT_NAME.Trim() + rd.Next().ToString(), true);
                        c.CAT_FK_ADID = Convert.ToInt32(Session["AD_ID"].ToString());


                        db.TBL_CATEGORY.Add(c);
                        db.SaveChanges();
                        TempData["mgss"] = "Subject Added";
                    }
                }
            }
            catch (Exception)
            {

                ViewBag.msg = "Please Enter Subject Name ....";

            }

            return RedirectToAction("AddCategory");

        }

        [HttpGet]
        public ActionResult AddQuestion()
        {
            if (Session["AD_ID"] == null)
            {
                return RedirectToAction("TLOGIN");
            }

            int sid = Convert.ToInt32(Session["AD_ID"]);
            List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID == sid).ToList();
            ViewBag.list =new SelectList(li, "CAT_ID", "CAT_NAME");
            return View();
        }
        [HttpPost]
        public ActionResult AddQuestion(TBL_QUESTIONS q, string qname)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var qt = db.TBL_QUESTIONS.Where(x => x.Q_TEXT == q.Q_TEXT).FirstOrDefault();
                    if (qt != null)
                    {
                        TempData["qmgs"] = "Question Already Exist";
                    }
                    else
                    {

                        int sid = Convert.ToInt32(Session["AD_ID"]);
                        List<TBL_CATEGORY> li = db.TBL_CATEGORY.Where(x => x.CAT_FK_ADID == sid).ToList();
                        ViewBag.list = new SelectList(li, "CAT_ID", "CAT_NAME");

                        TBL_QUESTIONS qa = new TBL_QUESTIONS();
                        qa.Q_TEXT = q.Q_TEXT;
                        qa.OPA = q.OPA;
                        qa.OPB = q.OPB;
                        qa.OPC = q.OPC;
                        qa.OPD = q.OPD;
                        qa.COP = q.COP;
                        qa.Q_FK_CATID = q.Q_FK_CATID;


                        db.TBL_QUESTIONS.Add(qa);
                        db.SaveChanges();
                        TempData["Msg"] = "Question Added ";
                        TempData.Keep();

                    }
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("AddQuestion");
        }
        public ActionResult ViewAllQuestions(int ?id, int? page)
        {
            if (Session["AD_ID"] == null)
            {

                return RedirectToAction("TLOGIN");
            }
            if (id == null)
            {
                return RedirectToAction("Dashboard");
            }

            int pagesize = 5, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TBL_QUESTIONS> pg = db.TBL_QUESTIONS.Where(x => x.Q_FK_CATID == id).ToList();
            IPagedList<TBL_QUESTIONS> st = pg.ToPagedList(pageindex, pagesize);



            return View(st);

        }



        public ActionResult Records()
        {
            //   if(Session["AD_ID"] == null)
            //  {
            //      return RedirectToAction("Index");
            //  }
            return View(from TBL_STUDENT in db.TBL_STUDENT select TBL_STUDENT);
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var dataa = db.TBL_QUESTIONS.Where(x => x.QUESTION_ID == id).FirstOrDefault();
                db.TBL_QUESTIONS.Remove(dataa);
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Msssg"] = "Quesion Deleted Sucessfully";
                TempData.Keep();

            }
            return RedirectToAction("AddCategory");
        }

        public ActionResult DeleteCat(int id)
        {
            try
            {
                var dat = db.TBL_CATEGORY.Where(x => x.CAT_ID == id).FirstOrDefault();
                db.TBL_CATEGORY.Remove(dat);
                db.SaveChanges();
                TempData["Mag"] = "Category Sucessfully Deleted";
                TempData.Keep();
            }
            catch (Exception)
            {

                TempData["Mcag"] = "Delete Questions First";
                TempData.Keep();

            }
            return RedirectToAction("AddCategory");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = db.TBL_QUESTIONS.Where(x => x.QUESTION_ID == id).FirstOrDefault();


            return View(data);
        }


        [HttpPost]
        public ActionResult Edit(TBL_QUESTIONS tbq)
        {
            var data = db.TBL_QUESTIONS.Where(x => x.QUESTION_ID == tbq.QUESTION_ID).FirstOrDefault();
            if (data != null)
            {
                data.Q_TEXT = tbq.Q_TEXT;
                data.OPA = tbq.OPA;
                data.OPB = tbq.OPB;
                data.OPC = tbq.OPC;
                data.OPD = tbq.OPD;
                data.COP = tbq.COP;
                data.Q_FK_CATID = tbq.Q_FK_CATID;
                db.SaveChanges();

            }
            TempData["Mssg"] = "Question Updated ";
            TempData.Keep();
            return RedirectToAction("Edit");


        }
        [HttpGet]
        public ActionResult EditCat(int id)
        {
            var da = db.TBL_CATEGORY.Where(x => x.CAT_ID == id).FirstOrDefault();
            return View(da);
        }

        [HttpPost]
        public ActionResult EditCat(TBL_CATEGORY tcg)
        {
            var da = db.TBL_CATEGORY.Where(x => x.CAT_ID == tcg.CAT_ID).FirstOrDefault();
            if (da != null)
            {
                da.CAT_NAME = tcg.CAT_NAME;
                db.SaveChanges();
            }
            return RedirectToAction("AddCategory");

        }




        public ActionResult About()
        {
            

            return View();
        }

        public ActionResult Contact()
        {
          

            return View();
        }

        public ActionResult UserList()
        {





            return View(from TBL_STUDENT in db.TBL_STUDENT select TBL_STUDENT);


        }
        public ActionResult result()
        {
            var re = db.TBL_SETEXAM.ToList();



            return View(re);
        }

        public ActionResult UserListData()
        {
            if (Session["AD_ID"] == null)
            {

                return RedirectToAction("TLOGIN");
            }
            return View();
        }

        public ActionResult ExportPDF()
        {
            return new Rotativa.ActionAsPdf("UserList");
        }

        public ActionResult ExportPDFScr()
        {
            return new Rotativa.ActionAsPdf("Records");
        }

    }
}
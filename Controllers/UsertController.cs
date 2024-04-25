using coffee_shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.IO;
using System.Web.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using ChangePassword = coffee_shop.Models.ChangePassword;

namespace coffee_shop.Controllers
{
    [RoutePrefix("api/user")]
    public class UsertController : ApiController
    {
        Coffee_shopEntities db = new Coffee_shopEntities();
        Response response = new Response();
        [System.Web.Http.HttpPost, System.Web.Http.Route("signup")]
        public HttpResponseMessage Signup([System.Web.Http.FromBody] User user)
        {
            try
            {
                User userObj = db.Users
                    .Where(u => u.email == user.email).FirstOrDefault();
                if (userObj == null)
                {
                    user.role = "user";
                    user.status = "false";
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Successfully Registered" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Email already exixts" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        [HttpPost, Route("login")]
        public HttpResponseMessage Login([FromBody] User user)
        {
                Console.WriteLine("user is trying to login");
            try
            {
                User userObj = db.Users
                    .Where(u => (u.email == user.email && u.password == user.password)).FirstOrDefault();
                Console.WriteLine("user found");
                if (userObj != null)
                {
                    if (userObj.status == "true")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManager.GenerateToken(userObj.email, userObj.role) });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "wait for admin approval" });

                    }
                }
                else
                {

                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Incorrect Username or Password" });
                }
            }
            catch(Exception e) { return Request.CreateResponse(HttpStatusCode.InternalServerError,e); }
        }


        [HttpGet, Route("checkToken")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage CheckToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new { message = "true" });
        }

        [HttpGet, Route("getAllUser")]
        // [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllUser()
        {
            try
            {
                // var token = Request.Headers.GetValues("authorization").First();
                // TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                /*if(tokenClaim.Role != "admin") 
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }*/
                var result = db.Users
                    .Select(u=> new { u.id, u.name, u.contactNumber, u.email, u.status, u.role })
                    .Where(x=> (x.role == "user"))
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);   

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("updateUserStatus")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateUserStatus(User user)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if(tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                User userObj = db.Users.Find(user.id);
                if(userObj == null)
                {
                    response.message = "User id does not found";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                userObj.status = user.status;
                db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "User status updated successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("changePassword")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage ChangePassword(ChangePassword changepassword)
        {
            try
            {
                var token = Request.Headers.GetValues("authentication").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                User userObj = (User)db.Users
                    .Where(u => u.email == tokenClaim.Email && u.password == changepassword.OldPassword);
                if(userObj != null)
                {
                    userObj.password = changepassword.NewPassword;
                    db.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    response.message = "Password updated successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.message = "Incorrect Old Password";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);                    
                }
            }catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private string createEmailBody(string email, string password)
        {
            try
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/Template/forgot-password.html")))
                {
                    body = reader.ReadToEnd();
                }
                body.Replace("{email}", email);
                body.Replace("{password}", password);
                body.Replace("{frontendUrl}", "https://localhost:4200/");
                return body;
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HttpPost, Route("forgotPassword")]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] User user)
        {
            User userObj= db.Users.Where(u=> u.email==user.email).FirstOrDefault();
            response.message = "Password sent successfully to your email";
            if(userObj == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.email));
            message.Subject = "Password by CoffeeShop";
            message.Body = createEmailBody(user.email, user.password);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
                await Task.FromResult(0);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
    }
}

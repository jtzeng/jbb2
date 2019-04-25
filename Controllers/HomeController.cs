using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using ServiceStack.DataAnnotations;

using System.Text;
using System.IO;

using Microsoft.AspNetCore.Http;

using JustinBB.Models;
using JustinBB;

using BCrypt.Net;

namespace JustinBB.Controllers
{
    public class HomeController : Controller
    {

        // Get the current instance of the User.
        // Returns null if the user is not logged in.
        private User getUser()
        {
            return JustinBB.Models.User.GetUser(HttpContext.Session.GetString("username"));
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(NewPost np)
        {
            User u = getUser();
            if (u == null)
            {
                ViewData["err"] = "You are not logged in.";
                return RedirectToAction("Index");
            }

            // Validate images.
            if (np.File != null)
            {
                var isValid = ImageHandler.ValidateImage(np.File);
                if (!isValid)
                {
                    ViewData["err"] = "Invalid image format.";
                    return RedirectToAction("Index");
                }
            }

            Console.WriteLine("Creating new post...");
            Console.WriteLine(np.TopicID);
            Console.WriteLine(np.Contents);
            Console.WriteLine(np.File != null ? np.File.Length.ToString() : "(no file)");

            Post p = new Post();

            p.TopicID = np.TopicID;
            p.UserID = u.ID;
            p.Contents = np.Contents;
            // TODO: Use GUID as filename.
            if (np.File != null) p.ImageID = Guid.NewGuid().ToString();
            // Must include flag to obtain last auto increment value.
            var postAutoID = Program.DB.Insert(p, true);

            await ImageHandler.SaveFile(np.File, p.ImageID);

            // Redirect to the topic.
            return Redirect($"/Home/Topic/{p.TopicID}");
        }

        [HttpPost]
        public async Task<IActionResult> NewTopic(NewTopic nt)
        {
            User u = getUser();
            if (u == null)
            {
                ViewData["err"] = "You are not logged in.";
                return RedirectToAction("Index");
            }

            // Validate images.
            if (nt.File != null)
            {
                var isValid = ImageHandler.ValidateImage(nt.File);
                if (!isValid)
                {
                    ViewData["err"] = "Invalid image format.";
                    return RedirectToAction("Index");
                }
            }

            if (nt.Title.Length < 1)
            {
                ViewData["err"] = "Please enter a title.";
                return RedirectToAction("Index");
            }

            Console.WriteLine("Creating new topic...");
            Console.WriteLine(nt.Title);
            Console.WriteLine(nt.Contents);
            Console.WriteLine(nt.File != null ? nt.File.Length.ToString() : "(no file)");

            Topic t = new Topic();
            Post p = new Post();

            t.UserID = u.ID;
            t.Title = nt.Title;
            var topicAutoID = Program.DB.Insert(t, true);

            p.TopicID = (int) topicAutoID;
            p.UserID = u.ID;
            p.Contents = nt.Contents;
            if (nt.File != null) p.ImageID = Guid.NewGuid().ToString();
            var postAutoID = Program.DB.Insert(p, true);

            await ImageHandler.SaveFile(nt.File, p.ImageID);

            // Redirect to the newly created topic.
            return Redirect($"/Home/Topic/{p.TopicID}");
        }

        public IActionResult Index()
        {
            ViewData["topics"] = JustinBB.Models.Topic.GetTopics();
            return View();
        }

        [HttpGet]
        public IActionResult Topic(int id)
        {
            Topic t = JustinBB.Models.Topic.GetTopic(id);
            if (t != null)
            {
                // Pass data to view.
                ViewData["topic"] = t;
                ViewData["posts"] = t.Posts;
                return View(t);
            }

            // Redirect to index if not found.
            Console.WriteLine($"Topic {id} not found!");

            ViewData["err"] = $"Topic {id} not found!";
            return RedirectToAction("Index");
        }

        [HttpGet("/Home/Vote/{postID}/{direction}")]
        public IActionResult Vote([FromRoute] int postID, [FromRoute] string direction)
        {
            User u = getUser();
            if (u == null)
            {
                ViewData["err"] = "You are not logged in.";
                return RedirectToAction("Index");
            }

            int dirNum = 0;

            if (direction == "up")
            {
                dirNum = 1;
            }
            else if (direction == "down")
            {
                dirNum = -1;
            }

            // Validate post.
            Post p = JustinBB.Models.Post.GetPost(postID);
            if (p == null)
            {
                ViewData["err"] = "Invalid post";
                return RedirectToAction("Index");
            }

            p.SetVote(u, dirNum);

            Console.WriteLine($"User {u.Username} voted for post {postID}.");

            // Return just a number, not a proper view, since this is parsed by AJAX.
            return Content(p.Score);
            // return new EmptyResult();
            // return Redirect($"/Home/Topic/{p.TopicID}");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User u)
        {
            // If the tickbox is checked, attempt to register the user.
            if (u.Register)
            {
                var rows = Program.DB.Select<User>(x => x.Username == u.Username);
                if (rows.Count > 0)
                {
                    ViewData["Err"] = "Sorry, but this user already exists.";
                    return View();
                }

                u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);

                Program.DB.Insert(u);

                Console.WriteLine($"Registered user {u.Username}:{new string('*', u.Password.Length)}!");

                TempData["status"] = "registered";
                HttpContext.Session.SetString("username", u.Username);
                return RedirectToAction("Index");
            }
            else
            {
                var rows = Program.DB.Select<User>(x => x.Username == u.Username);
                if (rows.Count > 0)
                {
                    var row = rows[0];

                    if (BCrypt.Net.BCrypt.Verify(u.Password, row.Password))
                    {
                        Console.WriteLine($"Logged in {u.Username}:{new string('*', u.Password.Length)}!");

                        TempData["status"] = "logged_in";
                        HttpContext.Session.SetString("username", u.Username);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewData["Err"] = "Invalid username or password.";
                        return View();
                    }
                }
                else
                {
                    ViewData["Err"] = "Invalid username.";
                    return View();
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

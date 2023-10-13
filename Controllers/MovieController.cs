using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;

        public MovieController(IMovieService movieService, IFileService fileService, IGenreService genreService)
        {
            _movieService = movieService;
            _fileService = fileService;
            _genreService = genreService;
        }
        public IActionResult Add()
        {
            var model = new Movie();
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text=a.GenreName, Value=a.Id.ToString()});

            return View(model);
        }
        [HttpPost]
        public IActionResult Add(Movie model)
        {
            model.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if(model.ImageFile != null)
            {
                var fileResult = this._fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File cloud not save";
                    return View(model);
                }
                var imageName = fileResult.Item2;
                model.MovieImage = imageName;
            }

            var result = _movieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Successfully Added";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View();
            }

        }

        public IActionResult Edit(int Id)
        {
            var model = _movieService.GetById(Id);
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(),"Id","GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList; 
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Movie model)
        {
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(), "Id", "GenreName", selectedGenres);
            model.MultiGenreList = multiGenreList;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ImageFile != null)
            {
                var fileResult = this._fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File cloud not save";
                    return View(model);
                }
                var imageName = fileResult.Item2;
                model.MovieImage = imageName;
            }

            var result = _movieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Successfully Updated";
                return RedirectToAction(nameof(MovieList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }
        }

        public IActionResult MovieList()
        {
            var data = this._movieService.List();
            return View(data);
        }

        public IActionResult Delete(int Id)
        {

            var result = _movieService.Delete(Id);

            return RedirectToAction(nameof(MovieList));
        }
    }
}

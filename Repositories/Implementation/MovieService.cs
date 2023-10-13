using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext _dbContext;

        public MovieService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Add(Movie model)
        {
            try
            {
                _dbContext.Movie.Add(model);
                _dbContext.SaveChanges();

                //to classify movie genre
                foreach (var genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };
                    _dbContext.MovieGenre.Add(movieGenre);
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                {
                    return false;
                }

                //remove this movie genre from MovieGenre table
                var movieGenres = _dbContext.MovieGenre.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    _dbContext.Remove(movieGenre);
                }

                //remove this movie from Movie table
                _dbContext.Movie.Remove(data);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            return _dbContext.Movie.Find(id);
        }

        public MovieListVm List(string term = "", bool paging = false, int currentPage = 0)
        {
            var data = new MovieListVm();
            var list = _dbContext.Movie.ToList();

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                list = list.Where(a => a.Title.ToLower().StartsWith(term)).ToList();
            }

            if (paging)
            {
                //apply paging here
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPage = TotalPages;
            }

            foreach (var movie in list)
            {
                var genres = (from genre in _dbContext.Genre
                              join mg in _dbContext.MovieGenre
                              on genre.Id equals mg.GenreId
                              where mg.MovieId == movie.Id
                              select genre.GenreName
                              ).ToList();
                var genreName = string.Join(',', genres);
                movie.GenreNames = genreName;
            }

            data.MovieList = list.AsQueryable();
            return data;
        }

        public bool Update(Movie model)
        {
            try
            {
                //remove old genre from moviegenre
                var genresToDeleted = _dbContext.MovieGenre
                                      .Where(a => a.MovieId == model.Id && !model.Genres
                                      .Contains(a.GenreId))
                                      .ToList();
                foreach (var movieGenre in genresToDeleted)
                {
                    _dbContext.MovieGenre.Remove(movieGenre);
                }

                _dbContext.Movie.Update(model);

                foreach (int genreId in model.Genres)
                {
                    var movieGenre = _dbContext.MovieGenre.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genreId);

                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genreId, MovieId = model.Id };
                        _dbContext.MovieGenre.Add(movieGenre);
                    }
                }

                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<int> GetGenreByMovieId(int movieId)
        {
            var genreIds = _dbContext.MovieGenre.Where(a => a.MovieId == movieId).Select(a => a.GenreId).ToList();

            return genreIds;
        }
    }
}

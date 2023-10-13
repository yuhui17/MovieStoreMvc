using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class GenreService : IGenreService
    {
        private readonly DatabaseContext _dbContext;

        public GenreService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Add(Genre model)
        {
            try
            {
                _dbContext.Genre.Add(model);
                _dbContext.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false; 
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if(data == null)
                {
                    return false;
                }

                _dbContext.Genre.Remove(data);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Genre GetById(int id)
        {
            return _dbContext.Genre.Find(id);
        }

        public IQueryable<Genre> List()
        {
            var data = _dbContext.Genre.AsQueryable();
            return data;
        }

        public bool Update(Genre model)
        {
            try
            {
                _dbContext.Genre.Update(model);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

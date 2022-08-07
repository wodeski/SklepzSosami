using Serwis.Models.Domains;

namespace Serwis.Converter
{
    public static class UserConverter
    {
        public static ApplicationUser ConvertToApplicationUser(this Register model)
        {
            return new ApplicationUser
            {
                UserName = model.UserName,
                Password = model.Password,
                Email = model.Email,
                CreatedDate = model.CreatedDate
            };
        }

    }
}

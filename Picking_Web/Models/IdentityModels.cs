using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Picking_Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool Ativo { get; set; }
        public bool Licenciado { get; set; }
        public bool Operador { get; set; }
        public int UsuarioSAPId { get; set; }
        public int EmpresaId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Impressora> Impressoras { get; set; }
        public DbSet<TipoImpressora> TipoImpressoras { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<GestaoImpressoes> GestaoImpressoes { get; set; }
        public DbSet<GestaoEtiquetas> GestaoEtiquetas { get; set; }
        public DbSet<Licenca> Licenca { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
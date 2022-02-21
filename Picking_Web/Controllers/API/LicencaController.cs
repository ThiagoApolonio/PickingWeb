using Picking_Web.Helpers;
using Picking_Web.Models;
using System.Linq;
using System.Web.Http;

namespace Picking_Web.Controllers.API
{
    public class LicencaController : ApiController
    {

        private ApplicationDbContext _context;

        public LicencaController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetLicenca()
        {
            var licenca = _context.Licenca.ToList();

            licenca[0].QuantidadeLicencas = EncryptionHelper.Base64Decode(licenca[0].QuantidadeLicencas);

            return Ok(licenca);
        }
    }
}

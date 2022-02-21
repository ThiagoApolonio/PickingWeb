using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Picking_Web.DTOs;
using Picking_Web.Models;
using System.Data.Entity;

namespace Picking_Web.Controllers.API
{
    public class ImpressorasController : ApiController
    {
        private ApplicationDbContext _context;

        public ImpressorasController()
        {
            _context = new ApplicationDbContext();
        }
        
        // GET /api/impressoras
        public IHttpActionResult GetImpressoras(string query = null)
        {
            var customersQuery = _context.Impressoras.Include(i => i.TipoImpressora);
            
            if (!String.IsNullOrWhiteSpace(query))
                customersQuery = customersQuery.Where(i => i.Descricao.Contains(query));

            var customerDtos = customersQuery
                .ToList()
                .Select(Mapper.Map<Impressora, ImpressoraDto>);

            return Ok(customerDtos);
        }

        // GET /api/impressoras/1
        public IHttpActionResult GetImpressora(int id)
        {
            var impressora = _context.Impressoras.SingleOrDefault(c => c.Id == id);

            if (impressora == null)
            {
                NotFound();
            }

            return Ok(Mapper.Map<Impressora, ImpressoraDto>(impressora));
        }

        // POST /api/impressoras
        [HttpPost]
        public IHttpActionResult CriarImpressora(ImpressoraDto impressoraDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var impressora = Mapper.Map<ImpressoraDto, Impressora>(impressoraDto);
            _context.Impressoras.Add(impressora);
            _context.SaveChanges();

            impressoraDto.Id = impressora.Id;
            

            return Created(new Uri(Request.RequestUri + "/" + impressora.Id), impressoraDto );
        }

        // PUT /api/impressoras/1
        [HttpPut]
        public IHttpActionResult AtualizarImpressora(int id, ImpressoraDto impressoraDto)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var impressoraInDb = _context.Impressoras.Single(i => i.Id == id);

            if (impressoraInDb == null)
            {
                return NotFound();
            }

            Mapper.Map(impressoraDto, impressoraInDb);

            _context.SaveChanges();

            return Ok();
        }

        // DELETE /api/impressoras/1
        [HttpDelete]
        public IHttpActionResult DeletarImpressora(int id)
        {
            var impressoraInDb = _context.Impressoras.Single(i => i.Id == id);

            if (impressoraInDb == null)
            {
                return NotFound();
            }

            _context.Impressoras.Remove(impressoraInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}

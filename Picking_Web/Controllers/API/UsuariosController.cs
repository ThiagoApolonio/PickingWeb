using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Picking_Web.Models;
using AutoMapper;
using Picking_Web.DTOs;
using Picking_Web.Helpers;
using Microsoft.AspNet.Identity;

namespace Picking_Web.Controllers.API
{
    public class UsuariosController : ApiController
    {

        private ApplicationDbContext _context;

        public UsuariosController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/usuarios
        public IHttpActionResult GetUsuarios(string query = null)
        {
            List<UsuariosDto> usuariosDtos = _context.Users.ToList().Select(Mapper.Map<ApplicationUser, UsuariosDto>).ToList();

            if (User.Identity.GetUserId() != Privilegios.AdminUserID && User.Identity.GetUserId() != Privilegios.ConvidadoUserID)
            {
                var userAdmin = usuariosDtos.Find(u => u.Id == Privilegios.AdminUserID);
                var userConv = usuariosDtos.Find(u => u.Id == Privilegios.ConvidadoUserID);

                usuariosDtos.Remove(userAdmin);
                usuariosDtos.Remove(userConv);
            }

            return Ok(usuariosDtos);
        }

        // GET /api/usuarios
        public IHttpActionResult GetUsuariosSAP(int empresa_id)
        {
            try
            {
                return Ok(SAPHelper.GetUsuariosSap(_context, empresa_id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // DELETE /api/usuarios/1
        [HttpDelete]
        public IHttpActionResult DeletarUsuario(string id)
        {
            var usuarioInDb = _context.Users.Single(u => u.Id == id);

            if (usuarioInDb == null)
            {
                return NotFound();
            }

            _context.Users.Remove(usuarioInDb);
            _context.SaveChanges();

            return Ok();
        }
    }
}

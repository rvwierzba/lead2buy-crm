using Lead2Buy.API.Data;
using Lead2Buy.API.DTOs.Network;
using Lead2Buy.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lead2Buy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NetworkController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NetworkController(AppDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetNetworkContacts()
        {
            var userId = GetUserId();
            var contacts = await _context.NetworkContacts
                .Where(nc => nc.UserId == userId)
                .OrderBy(nc => nc.Name)
                .Select(nc => new NetworkContactDto
                {
                    Id = nc.Id,
                    Name = nc.Name,
                    PhoneNumber = nc.PhoneNumber,
                    Email = nc.Email,
                    LinkedInUrl = nc.LinkedInUrl,
                    InstagramUrl = nc.InstagramUrl,
                    OtherSocialUrl = nc.OtherSocialUrl,
                    Notes = nc.Notes,
                    FacebookUrl = nc.FacebookUrl,
                    WhatsAppNumber = nc.WhatsAppNumber,
                    YouTubeChannelUrl = nc.YouTubeChannelUrl
                })
                .ToListAsync();
            
            return Ok(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNetworkContact([FromBody] CreateNetworkContactDto dto)
        {
            var userId = GetUserId();
            var newContact = new NetworkContact
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                LinkedInUrl = dto.LinkedInUrl,
                InstagramUrl = dto.InstagramUrl,
                OtherSocialUrl = dto.OtherSocialUrl,
                Notes = dto.Notes,
                UserId = userId,
                FacebookUrl = dto.FacebookUrl,
                WhatsAppNumber = dto.WhatsAppNumber,
                YouTubeChannelUrl = dto.YouTubeChannelUrl
            };

            _context.NetworkContacts.Add(newContact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNetworkContacts), new { id = newContact.Id }, newContact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNetworkContact(Guid id, [FromBody] CreateNetworkContactDto dto)
        {
            var userId = GetUserId();
            var contact = await _context.NetworkContacts.FirstOrDefaultAsync(nc => nc.Id == id && nc.UserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            contact.Name = dto.Name;
            contact.PhoneNumber = dto.PhoneNumber;
            contact.Email = dto.Email;
            contact.LinkedInUrl = dto.LinkedInUrl;
            contact.InstagramUrl = dto.InstagramUrl;
            contact.OtherSocialUrl = dto.OtherSocialUrl;
            contact.Notes = dto.Notes;
            contact.FacebookUrl = dto.FacebookUrl;
            contact.WhatsAppNumber = dto.WhatsAppNumber;
            contact.YouTubeChannelUrl = dto.YouTubeChannelUrl;


            _context.NetworkContacts.Update(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNetworkContact(Guid id)
        {
            var userId = GetUserId();
            var contact = await _context.NetworkContacts.FirstOrDefaultAsync(nc => nc.Id == id && nc.UserId == userId);

            if (contact == null)
            {
                return NotFound();
            }

            _context.NetworkContacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
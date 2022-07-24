using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorRoman.Models;

namespace BlazorRoman.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly RomanDBContext _context;
        public DoctorsController(RomanDBContext context)
        {
            _context = context;
        }
        // GET: api/Doctors or api/Doctors?sortOrder="param"
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadDoctors>>> GetDoctors(string? sortOrder = null)
        {
            if (_context.Doctors == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors.Include(c => c.CabinetNavigation).Include(s => s.SpecializationNavigation).Include(r => r.RegionNavigation).ToListAsync();
            doctors = sortOrder switch
            {
                "FullName" => doctors.OrderBy(s => s.FullName).ToList(),
                "Specialization" => doctors.OrderBy(s => s.SpecializationNavigation.Title).ToList(),
                "Cabinet" => doctors.OrderBy(s => s.CabinetNavigation.Number).ToList(),
                "Region" => doctors.OrderBy(s => s.RegionNavigation.Number).ToList(),
                _ => doctors.OrderBy(s => s.Id).ToList(),
            };
            return FillingDoctor(doctors);            
        }
        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ReadDoctors>>> GetDoctors(int id)
        {
            if (_context.Doctors == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors.FindAsync(id);
            if (doctors == null)
            {
                return NotFound();
            }
            return FillingDoctor(await _context.Doctors
                .Where(d => d == doctors)
                .Include(c => c.CabinetNavigation)
                .Include(s => s.SpecializationNavigation)
                .Include(r => r.RegionNavigation)
                .ToListAsync());            
        }
        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctors(int id,[FromBody]ReadDoctors doctors)
        {
            var dbDoctor = await _context.Doctors.Where(d => d.Id == id).Include(c => c.CabinetNavigation).Include(s => s.SpecializationNavigation).Include(r => r.RegionNavigation).FirstOrDefaultAsync();
           
            if (dbDoctor?.FullName != doctors.FullName || dbDoctor == null)
            {
                return NotFound();
            }
            dbDoctor =  await Task.Run(()=> Update(dbDoctor, doctors));            
            if(dbDoctor == null)
            {
                return BadRequest();
            }
            _context.Entry(dbDoctor).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Redirect("/.Error");
                }
            }
            return Ok();
        }
        // POST: api/Doctors
        [HttpPost]
        public async Task<IActionResult> PostDoctors([FromBody]ReadDoctors doctors)
        {
          if (_context.Doctors == null)
          {
              return Problem("Entity set 'RomanDBContext.Doctors'  is null.");
          }
          if(!await AttributeExistAsync(doctors))
          {
                return Problem("Entity set 'Attribute Table'  is null.");
          }
            Doctors newdoctor = await FillingDoctorAsync(doctors);
            _context.Doctors.Add(newdoctor);
            await _context.SaveChangesAsync();
            return Created("", FillingDoctor(new List<Doctors>() { newdoctor }));
              
            //  CreatedAtAction("GetDoctors", newdoctor);
        }
        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctors(int id)
        {
            if (_context.Doctors == null)
            {
                return NotFound();
            }
            var doctors = await _context.Doctors.FindAsync(id);
            if (doctors == null)
            {
                return NotFound();
            }
            _context.Doctors.Remove(doctors);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DoctorsExists(int id)
        {
            return (_context.Doctors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private async Task<bool> AttributeExistAsync(ReadDoctors doctors)
        {
            try
            {
                if (doctors.Region != null)
                {
                    var region = _context.Region
                        .Where(c => c.Number == doctors.Region)
                        .FirstOrDefault() == null ? (_context.Region.Add(new Region { Number = (int)doctors.Region })) : null;
                }
                var cabinet = _context.Cabinets
                    .Where(c => c.Number == doctors.Cabinet)
                    .FirstOrDefault() == null ? (_context.Cabinets.Add(new Cabinets { Number = (int)doctors.Cabinet })) : null;

                var specialization = _context.Specialization
                    .Where(c => c.Title == doctors.Specialization)
                    .FirstOrDefault() == null ? (_context.Specialization.Add(new Specialization { Title = doctors.Specialization })) : null;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<Doctors> FillingDoctorAsync(ReadDoctors doctors)
        {
            var cabinet = await _context.Cabinets
                .Where(c => c.Number == doctors.Cabinet)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            var region = await _context.Region
                 .Where(c => c.Number == doctors.Region)
                 .Select(c => c.Id)
                 .FirstOrDefaultAsync();
            var specialization = await _context.Specialization
                .Where(c => c.Title == doctors.Specialization)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            return new Doctors
            {
                FullName = doctors.FullName,
                Cabinet = cabinet,
                Region = region == 0 ? (int?)null : region,
                Specialization = specialization
            };
        }
        private static List<ReadDoctors> FillingDoctor(List<Doctors> doctors)
        {
            List<ReadDoctors> doctorsAll = new List<ReadDoctors>();
            foreach (var doc in doctors)
            {
                doctorsAll.Add(new ReadDoctors
                {
                    FullName = doc.FullName,
                    Cabinet = (doc.Cabinet == null) ? (int?)null : doc.CabinetNavigation.Number,
                    Specialization = (doc.Specialization == null) ? (string?)null : doc.SpecializationNavigation.Title,
                    Region = (doc.Region == null) ? (int?)null : doc.RegionNavigation.Number,
                });
            }
            return doctorsAll;
        }
        private static Doctors Update(Doctors dbDoctor, ReadDoctors doctors )
        {           
                dbDoctor.FullName = doctors.FullName;
                dbDoctor.RegionNavigation = new Region { Number = (int)doctors.Region };
                dbDoctor.CabinetNavigation = new Cabinets { Number = (int)doctors.Cabinet };
                dbDoctor.SpecializationNavigation = new Specialization { Title =doctors.Specialization };
          
            
            return dbDoctor;
        }
    }
    
}

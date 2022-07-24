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
    public class PatientsController : ControllerBase
    {
        private readonly RomanDBContext _context;

        public PatientsController(RomanDBContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadPatients>>> GetPatients(string? sortOrder = null)
        {
          if (_context.Patients == null)
          {
              return NotFound();
          }
          var patients = await _context.Patients.Include(r=>r.RegionNavigation).ToListAsync();

            patients = sortOrder switch
            {
                "Surname" => patients.OrderBy(s => s.Surname).ToList(),
                "Name" => patients.OrderBy(s => s.Name).ToList(),
                "MiddleName" => patients.OrderBy(s => s.MiddleName).ToList(),
                "Address"=>patients.OrderBy(s=>s.Address).ToList(),
                "DateOfBirth" => patients.OrderBy(s => s.DateOfBirth).ToList(),
                "Floor" => patients.OrderBy(s => s.Floor).ToList(),
                "Region" => patients.OrderBy(s => s.RegionNavigation.Number).ToList(),
                _ => patients.OrderBy(s => s.Id).ToList(),
            };
            return FillingPatients(patients);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ReadPatients>>> GetPatients(int id)
        {
          if (_context.Patients == null)
          {
              return NotFound();
          }
            var patients = await _context.Patients.FindAsync(id);

            if (patients == null)
            {
                return NotFound();
            }

            return FillingPatients( await _context.Patients
                .Where(p => p == patients)
                .Include(p =>p.RegionNavigation)
                .ToListAsync());
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatients(int id, [FromBody]ReadPatients patients)
        {
            var dbPatients = await _context.Patients.Where(d => d.Id == id).Include(r => r.RegionNavigation).FirstOrDefaultAsync();
            if (dbPatients == null)
            {
                return NotFound();
            }
            dbPatients = await Task.Run(() => Update(dbPatients, patients));
            _context.Entry(dbPatients).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientsExists(id))
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

        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patients>> PostPatients([FromBody]ReadPatients patients)
        {
          if (_context.Patients == null)
          {
              return Problem("Entity set 'RomanDBContext.Patients'  is null.");
          }
            if (!await AttributeExistAsync(patients))
            {
                return Problem("Entity set 'Attribute Table'  is null.");
            }
            Patients newpatient = await FillingPatientAsync(patients);
            _context.Patients.Add(newpatient);
            await _context.SaveChangesAsync();

            return Created("", FillingPatients(new List<Patients>() { newpatient}));
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatients(int id)
        {
            if (_context.Patients == null)
            {
                return NotFound();
            }
            var patients = await _context.Patients.FindAsync(id);
            if (patients == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patients);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientsExists(int id)
        {
            return (_context.Patients?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private async Task<bool> AttributeExistAsync(ReadPatients patients)
        {
            try
            {
                if (patients.Region != null)
                {
                    var region = _context.Region
                        .Where(c => c.Number == patients.Region)
                        .FirstOrDefault() == null ? (_context.Region.Add(new Region { Number = (int)patients.Region })) : null;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static List<ReadPatients> FillingPatients(List<Patients> patients)
        {
            List<ReadPatients> patientsAll = new List<ReadPatients>();
            foreach(var pat in patients)
            {
                patientsAll.Add(new ReadPatients
                {
                    Surname = pat.Surname,
                    Name = pat.Name,
                    MiddleName = pat.MiddleName,
                    Address = pat.Address,
                    DateOfBirth = pat.DateOfBirth.Date,
                    Floor = pat.Floor,
                    Region = (pat.Region == null) ? (int?)null : pat.RegionNavigation.Number
                });
            }
            return patientsAll;
        }
        private async Task<Patients>FillingPatientAsync(ReadPatients patients)
        {
             var region = await _context.Region
                 .Where(c => c.Number == patients.Region)
                 .Select(c => c.Id)
                 .FirstOrDefaultAsync();
            return new Patients
            {
                Surname = patients.Surname,
                Name = patients.Name,
                MiddleName = patients.MiddleName,
                Address = patients.Address,
                DateOfBirth =(DateTime)patients.DateOfBirth,
                Floor = patients.Floor,
                Region =region == 0? (int?) null : region
            };
        }       
        private static Patients Update(Patients dbPatient, ReadPatients patient)
        {
            dbPatient.Surname = patient.Surname;
            dbPatient.Name = patient.Name;
            dbPatient.MiddleName = patient.MiddleName;
            dbPatient.Address = patient.Address;
            dbPatient.DateOfBirth =(DateTime)patient.DateOfBirth;
            dbPatient.Floor = patient.Floor;
            dbPatient.RegionNavigation = new Region { Number = (int)patient.Region };
            return dbPatient;
        }
    }
}

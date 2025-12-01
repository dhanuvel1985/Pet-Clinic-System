using AppointmentService.Application.DTOs;
using AppointmentService.Application.Interfaces;
using AppointmentService.Domain.Entities;
using AppointmentService.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentService.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentDbContext _db;

        public AppointmentRepository(AppointmentDbContext db)
        {
            _db = db;
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _db.Appointments
                .Where(a => a.Status == AppointmentStatus.Pending)
                .ToListAsync();
        }

        public async Task<List<AppointmentDto>> GetAppointmentsAsync(int page, int pageSize)
        {
            return await _db.Appointments
                        .AsNoTracking()
                        .Select(p => new AppointmentDto
                        {
                            Id = p.Id,
                            UserId = p.UserId,
                            PetId = p.PetId,
                            PreferredDate = p.PreferredDate,
                            Reason = p.Reason
                        })
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
        }
    }
}

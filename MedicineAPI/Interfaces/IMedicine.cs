﻿using GoodAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoodAPI.Interfaces
{
    public interface IMedicine
    {
        Task<IEnumerable<Medicine>> GetMedicines();
        Task<Medicine> GetMedicineById(int MedicineId);
        Task<IActionResult> CreateUpdateMedicine(Medicine MedicineDto);
        Task<bool> DeleteMedicine(int MedicineId);
    }
}

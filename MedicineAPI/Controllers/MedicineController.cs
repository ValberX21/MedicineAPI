﻿using GoodAPI.Data;
using GoodAPI.Dto;
using GoodAPI.Erro;
using GoodAPI.Interfaces;
using GoodAPI.Mensage;
using GoodAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GoodAPI.Controllers
{
    [Route("api/medicines")]
    public class MedicineController : ControllerBase
    {
        protected ResponseDto _response;
        private IMedicine _medicineRepository;
        private readonly RabbitMQService _rabbitMQService;

        public MedicineController(IMedicine medicineRepository, ApplicationDbContext db, RabbitMQService rabbitMQService)
        {
            _medicineRepository = medicineRepository;
            this._response = new ResponseDto();
            _rabbitMQService = rabbitMQService;

        }

        [HttpPost("addUpdateMedicine")]
        public async Task<IActionResult> InsertMedicine([FromBody] Medicine Medicine)
        {
            try
            {
                object result = await _medicineRepository.CreateUpdateMedicine(Medicine);               

                _response.Result = result;

                var statusRetorned = result.GetType().GetProperty("StatusCode")?.GetValue(result);

                if(statusRetorned.ToString() == "500")
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }
                else
                {
                    _rabbitMQService.SendMessage("New medicine Added - " + Medicine.GenericName + " - " + Medicine.Name, "medicinesAdd");
                    return StatusCode(StatusCodes.Status201Created, _response);
                }
            }
            catch (Exception ex)
            {
                ErroRegister erroRegister = new ErroRegister();

                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                };

                erroRegister.register(errorLog);

                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);

            }
        }

        [HttpGet("getAllMedicines")]
        public async Task<IActionResult> getMedicines()
        {
            int codeStatus = 0;
            try
            {
                object result = await _medicineRepository.GetMedicines();

                _response.Result = result;

                codeStatus = 200;

            }
            catch (Exception ex)
            {
                ErroRegister erroRegister = new ErroRegister();

                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                };

                erroRegister.register(errorLog);

                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };

                codeStatus = 500;
            }

            return StatusCode(codeStatus, _response);
        }

        [HttpPut("updateMedicine")]
        public async Task<IActionResult> updateMedicine([FromBody] Medicine Medicine)
        {
            try
            {
                object result = await _medicineRepository.CreateUpdateMedicine(Medicine);

                var statusRetorned = result.GetType().GetProperty("StatusCode")?.GetValue(result);

                if (statusRetorned.ToString() != "200")
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }
                else
                {
                    _rabbitMQService.SendMessage("New medicine Added - " + Medicine.GenericName + " - " + Medicine.Name, "medicinesAdd");
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                ErroRegister erroRegister = new ErroRegister();

                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                };

                erroRegister.register(errorLog);

                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("deleteMedicine")]
        public async Task<IActionResult> deleteMedicine(int medicineID)
        {
            try
            {
                bool result = await _medicineRepository.DeleteMedicine(medicineID);

                if (!result)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                ErroRegister erroRegister = new ErroRegister();

                var errorLog = new ErrorLog
                {
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                };

                erroRegister.register(errorLog);

                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };

                return StatusCode(StatusCodes.Status500InternalServerError, _response);

            }
        }
    }
}
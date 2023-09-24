using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Dto.RequestDTO;
using MagicVilla_VillaAPI.Dto.ResponseDTO;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/villaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbvillaRepository;

        public VillaAPIController(ILogger<VillaAPIController> logger, IMapper mapper, IVillaRepository villaRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _dbvillaRepository = villaRepository;
            this._response = new();
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villaList = await _dbvillaRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>()
                {
                    e.ToString()
                };

                return _response;
            }
            
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>>  GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error invalid id " + id);
                    return BadRequest();
                }
                var villa = await _dbvillaRepository.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
            
                return Ok(_response);
            } catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>()
                {
                    e.ToString()
                };
                return _response;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _dbvillaRepository.GetAsync(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already exists");
                    return BadRequest();
                }
                if (villaCreateDTO == null)
                {
                    return BadRequest(villaCreateDTO);
                }

                Villa villa = _mapper.Map<Villa>(villaCreateDTO);

                await _dbvillaRepository.CreateAsync(villa);
            
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
            
                return CreatedAtRoute("GetVilla",new {id = villa.Id} ,villa);
            }catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>()
                {
                    e.ToString()
                };
                return _response;
            }
            
            
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbvillaRepository.GetAsync(u => u.Id == id);
                if(villa == null)
                {
                    return NotFound();
                }
                await _dbvillaRepository.RemoveAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
            
                return Ok(_response);
            } catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>()
                {
                    e.ToString()
                };
                return _response;
            }
            
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody]VillaUpdateDTO villaUpdateDTO)
        {
            if(villaUpdateDTO == null || id != villaUpdateDTO.Id)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;
            Villa villa = _mapper.Map<Villa>(villaUpdateDTO);

            await _dbvillaRepository.UpdateAsync(villa);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;

            return Ok(_response);
        }

        //Por ahora sin hacer
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbvillaRepository.GetAsync(u => u.Id == id, tracked: false);

            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            await _dbvillaRepository.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();
        }
        

    }
}

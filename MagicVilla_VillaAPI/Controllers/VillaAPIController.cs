using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
        private readonly ILogging logger;

        public VillaAPIController(ILogging _logger) 
        {
            logger = _logger;
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            logger.Log("Gettin all villas","");
            return Ok(VillaStore.villaList);

        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        //[ProducesResponseType(200,Type=typeof(VillaDTO))]
        //[ProducesResponseType(404)]      
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id==0)
            {
                logger.Log("Get villa error with Id"+id,"error");
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(villaDTO);
            //}

            if (VillaStore.villaList.FirstOrDefault(x=>x.Name.ToLower()==villaDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }
            if (villaDTO==null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id=VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id+1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla",new {id=villaDTO.Id},villaDTO);
        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id==0)
            {
                return BadRequest();
            }
            var villa=VillaStore.villaList.FirstOrDefault(x=> x.Id==id);
            if (villa==null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO==null||id!=villaDTO.Id)
            {
                return BadRequest();
            }
            var villa=VillaStore.villaList.FirstOrDefault(x=> x.Id==id);
            villa.Name=villaDTO.Name;
            villa.Sqft=villaDTO.Sqft;
            villa.Occupency=villaDTO.Occupency;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO==null||id==0)
            {
                return BadRequest();
            }
            var villa =VillaStore.villaList.FirstOrDefault(x=> x.Id==id);
            if (villa==null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa,ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}

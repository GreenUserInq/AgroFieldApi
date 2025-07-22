using Microsoft.AspNetCore.Mvc;
using AgroFieldApi.Models;
using AgroFieldApi.Services;

namespace AgroFieldApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly FieldService _fieldService;

        public FieldsController(FieldService fieldService)
        {
            _fieldService = fieldService;
        }

        /// <summary>
        /// Получение всех полей с координатами центра и контура.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllFields()
        {
            var fields = _fieldService.GetAllFields();
            return Ok(fields.Select(f => new
            {
                f.Id,
                f.Name,
                f.Size,
                Locations = f.Location
            }));
        }

        /// <summary>
        /// Получение площади поля по идентификатору.
        /// </summary>
        [HttpGet("{id}/size")]
        public IActionResult GetFieldSize(string id)
        {
            var size = _fieldService.GetFieldSize(id);
            return size == null ? NotFound() : Ok(size);
        }

        /// <summary>
        /// Получение расстояния от центра поля до точки.
        /// </summary>
        [HttpGet("{id}/distance")]
        public ActionResult<double> GetDistanceToPoint(string id, [FromQuery] double lat, [FromQuery] double lng)
        {
            try
            {
                var distance = _fieldService.GetDistanceToPoint(id, new GeoPoint(lat, lng));
                return Ok(distance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message); // 404 если поле не найдено
            }
        }

        /// <summary>
        /// Проверка, принадлежит ли точка одному из полей.
        /// </summary>
        [HttpGet("contains")]
        public IActionResult CheckPointInField([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = _fieldService.FindFieldContainingPoint(new GeoPoint(lat, lng));

            if (result.found)
                return Ok(new { result.id, result.name });
            else
                return Ok(false);
        }
    }
}

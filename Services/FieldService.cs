using AgroFieldApi.Models;
using AgroFieldApi.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgroFieldApi.Services
{
    public class FieldService
    {
        private readonly List<FieldModel> _fields;

        public FieldService(IOptions<KmlPathSettings> options)
        {
            var fieldsPath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.Fields);
            var centroidsPath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.Centroids);

            var centroidParser = new KmlCentroidParser();
            var fieldParser = new KmlFieldParser();

            var centroids = centroidParser.Parse(centroidsPath);
            var fields = fieldParser.Parse(fieldsPath);

            _fields = fields.Select(f =>
            {
                centroids.TryGetValue(f.Id, out GeoPoint center);
                var area = GeoUtils.CalculatePolygonArea(f.Polygon);

                return new FieldModel(f.Id, f.Name, center, f.Polygon, area);
            }).ToList();
        }

        public List<FieldModel> GetAllFields() => _fields;

        public FieldModel GetFieldById(string id) =>
            _fields.FirstOrDefault(f => f.Id == id);

        public double? GetFieldSize(string id) =>
            _fields.FirstOrDefault(f => f.Id == id)?.Size;

        public double GetDistanceToPoint(string id, GeoPoint point)
        {
            var field = _fields.FirstOrDefault(f => f.Id == id);

            if (field == null || field.Center == null)
                throw new InvalidOperationException("Field not found or center not calculated.");

            return GeoUtils.GetDistanceMeters(field.Center, point);
        }

        public (bool found, string id, string name) FindFieldContainingPoint(GeoPoint point)
        {
            foreach (var field in _fields)
            {
                if (GeoUtils.IsPointInsidePolygon(point, field.Polygon))
                    return (true, field.Id, field.Name);
            }
            return (false, null, null);
        }
    }
}

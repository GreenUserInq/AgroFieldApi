using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgroFieldApi.Models;

namespace AgroFieldApi.Services
{
    public class FieldService
    {
        private readonly List<FieldModel> _fields;

        public FieldService(string fieldsPath, string centroidsPath)
        {
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

        public double? GetDistanceToPoint(string id, GeoPoint point)
        {
            var field = GetFieldById(id);
            return field == null ? null : GeoUtils.GetDistanceMeters(field.Center, point);
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

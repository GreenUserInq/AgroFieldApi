using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using AgroFieldApi.Models;

namespace AgroFieldApi.Services
{
    public static class GeoUtils
    {
        /// <summary>
        /// Вычисляет расстояние в метрах между двумя точками 
        /// </summary>
        public static double GetDistanceMeters(GeoPoint p1, GeoPoint p2)
        {
            var coord1 = new GeoCoordinate(p1.Lat, p1.Lng);
            var coord2 = new GeoCoordinate(p2.Lat, p2.Lng);
            return coord1.GetDistanceTo(coord2); 
        }

        /// <summary>
        /// Проверяет, находится ли точка внутри полигона 
        /// </summary>
        public static bool IsPointInsidePolygon(GeoPoint point, List<GeoPoint> polygon)
        {
            int count = polygon.Count;
            bool inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if ((polygon[i].Lat > point.Lat) != (polygon[j].Lat > point.Lat) &&
                    (point.Lng < (polygon[j].Lng - polygon[i].Lng) * (point.Lat - polygon[i].Lat) /
                     (polygon[j].Lat - polygon[i].Lat) + polygon[i].Lng))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        /// <summary>
        /// Вычисляет площадь полигона в квадратных метрах (с использованием сферической геометрии).
        /// </summary>
        public static double CalculatePolygonArea(List<GeoPoint> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return 0;

            const double R = 6378137; // Радиус Земли в метрах
            double area = 0;

            for (int i = 0; i < polygon.Count; i++)
            {
                var p1 = polygon[i];
                var p2 = polygon[(i + 1) % polygon.Count];

                double lat1 = DegreesToRadians(p1.Lat);
                double lng1 = DegreesToRadians(p1.Lng);
                double lat2 = DegreesToRadians(p2.Lat);
                double lng2 = DegreesToRadians(p2.Lng);

                area += (lng2 - lng1) * (2 + Math.Sin(lat1) + Math.Sin(lat2));
            }

            area = area * R * R / 2.0;
            return Math.Abs(area); 
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}

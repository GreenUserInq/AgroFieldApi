using System.Xml;
using AgroFieldApi.Models;

namespace AgroFieldApi.Services
{
    /// <summary>
    /// Парсер KML-файлов для извлечения координат центроидов полей.
    /// </summary>
    public class KmlCentroidParser
    {
        /// <summary>
        /// Загружает и разбирает KML-файл, возвращая словарь центроидов полей.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        /// Словарь, где ключ — это <c>id</c> поля из KML, а значение — координаты центроида в виде <see cref="GeoPoint"/>.
        /// </returns>
        public Dictionary<string, GeoPoint> Parse(string filePath)
        {
            var centroids = new Dictionary<string, GeoPoint>();

            var xml = new XmlDocument();
            xml.Load(filePath);

            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

            var placemarks = xml.SelectNodes("//kml:Placemark", nsmgr);
            foreach (XmlNode placemark in placemarks)
            {
                var id = placemark.Attributes["id"]?.InnerText;
                if (id == null) continue;

                var coordinatesNode = placemark.SelectSingleNode(".//kml:coordinates", nsmgr);
                if (coordinatesNode == null) continue;

                var parts = coordinatesNode.InnerText.Trim().Split(',');
                var point = new GeoPoint(
                    double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture)
                );

                centroids[id] = point;
            }

            return centroids;
        }
    }
}

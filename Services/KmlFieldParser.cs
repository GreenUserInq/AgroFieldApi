using System.Xml;
using AgroFieldApi.Models;

namespace AgroFieldApi.Services
{
    /// <summary>
    /// Парсит KML-файл и извлекает список сельскохозяйственных полей
    /// </summary>
    public class KmlFieldParser
    {
        /// <summary>
        /// Загружает и разбирает KML-файл по указанному пути, формируя список моделей полей.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Список объектов <see cref="FieldModel"/>, представляющих найденные поля.</returns>
        public List<FieldModel> Parse(string filePath)
        {
            var fields = new List<FieldModel>();

            var xml = new XmlDocument();
            xml.Load(filePath);

            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

            var placemarks = xml.SelectNodes("//kml:Placemark", nsmgr);
            foreach (XmlNode placemark in placemarks)
            {
                var name = placemark.SelectSingleNode("kml:name", nsmgr)?.InnerText;
                var id = placemark.Attributes["id"]?.InnerText ?? Guid.NewGuid().ToString();

                var coordinatesNode = placemark.SelectSingleNode(".//kml:coordinates", nsmgr);
                if (coordinatesNode == null) continue;

                var coordsText = coordinatesNode.InnerText.Trim();
                var polygon = coordsText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        var parts = s.Split(',');
                        return new GeoPoint(
                            double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture)
                        );
                    }).ToList();

                fields.Add(new FieldModel(id, name, center: null, polygon, size: 0.0));
            }

            return fields;
        }
    }
}
